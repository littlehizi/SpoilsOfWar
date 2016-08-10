using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is responsible for storing all data related to the ground tiles.
/// It also carries methods affecting the tiles.
/// This class is attached to every tile in the grid, and stored in GridManagerBehavior.
/// </summary>
public class GroundBehavior : MonoBehaviour
{
	public enum GroundType
	{
		mud,
		hardDirt,
		compactedDirt,
		compactedDirtClay,
		clay,
		hardClay,
		bedRock
	}

	public enum EnvGroundType
	{
		building,
		buildingBG,
		sky,
		grass
	}

	//Main Info
	public BaseGroundData groundData;
	public GroundType typeOfGround;
	public Vector2 tilePos;
	public int ID;

	GroundBehavior[] _tileNeighbors;

	GroundBehavior[] tileNeighbors {
		get{ return _tileNeighbors; }
		set { 
			_tileNeighbors = PathfindingManagerBehavior.GetTileNeighbor (this).ToArray ();
		}
	}


	//Handy stuff (FOW)
	public Vector2 trueTilePos { get { return new Vector2 (tilePos.x, -tilePos.y); } }

	//Stats
	private int _hp;

	public int hp {
		get{ return _hp; }
		set {
			_hp = value;
			if (_hp <= 0)
				isDug = true;
		}
	}

	public float digRes;
	public Sprite sprite;
	public int moveCost;
	int collapseHP;
	bool isCollapsing;
	SpriteRenderer subTileSR;

	private bool _isFortified;

	public bool isFortified {
		get{ return _isFortified; }
		set {
			_isFortified = value;

			FortifyTile (_isFortified);

			//Fortification or collapse !
			SubscribeToCollapseTick (!_isFortified);

		}
	}

	private bool _isAnObstacle;

	public bool isAnObstacle {
		get { return _isAnObstacle; }
		set { 
			_isAnObstacle = value;

			//Change the sprite 
			if (_isAnObstacle) {
				GameObject obstacleSpriteT = new GameObject ("obstacle sprite");
				obstacleSpriteT.transform.position = trueTilePos;
				obstacleSpriteT.transform.parent = this.transform;

				SpriteRenderer obstacleSR = obstacleSpriteT.AddComponent<SpriteRenderer> ();
				obstacleSR.sprite = GameMasterScript.instance.GMB.obstacleSprite;
				obstacleSR.sortingLayerID = tileSR.sortingLayerID;
				obstacleSR.sortingOrder = tileSR.sortingOrder + 1;
		
				//Set digres and HP to unbreakable values 
				digRes = 99999;
				hp = 99999;
			} else {
				digRes = groundData.digRes;
				hp = groundData.hp;
				Destroy (this.transform.FindChild ("obstacle sprite").gameObject);
			}
		}
	}

	//Colors
	private SpriteRenderer _tileSR;

	public SpriteRenderer tileSR {
		get {
			if (_tileSR == null)
				_tileSR = this.GetComponent<SpriteRenderer> ();
			return _tileSR;
		}
	}

	public Color colorBackup;

	Color dugColorOffset = new Color (0.2f, 0.2f, 0.2f, 0.0f);


	//Vars
	public List<UnitBehavior> unitsOnTile;

	//Flags
	bool _isDug;

	public bool isDug {
		get { return _isDug; }
		set {
			bool prevValue = _isDug;

			//Change the flag and set the tile color to darker
			_isDug = value;

			if (_isDug && !prevValue)
				ApplyDugColor ();
			else if (!_isDug && prevValue)
				ApplyDugColor ();

			//Collapse ! or not
			SubscribeToCollapseTick (isDug);

			if (!_isDug) {
				for (int i = 0; i < unitsOnTile.Count; i++)
					unitsOnTile [i].OnDeathEnter ();
			}
		}
	}

	public bool isOccupied { get { return unitsOnTile.Count > 0; } }


	//PathFinding
	[HideInInspector]public GroundBehavior parent;

	//Extract the data from given GroundData
	public void SetupGround ()
	{
		//Initialize lists
		unitsOnTile = new List<UnitBehavior> ();
		_isFortified = false;

		//Get stats
		hp = groundData.hp;
		digRes = groundData.digRes;
		sprite = groundData.sprite;
		moveCost = groundData.moveCost;
		collapseHP = groundData.collapseHP;
		isCollapsing = false;
		isDug = false;

		StartCoroutine (SetTileNeighbor ());

		//Set subtile
		subTileSR = new GameObject ("subTileSR").AddComponent<SpriteRenderer> ();
		subTileSR.transform.SetParent (this.transform);
		subTileSR.transform.localPosition = Vector3.zero;
		subTileSR.sprite = groundData.sprite;
		subTileSR.color = Color.white;

		//Temporary stuff
		colorBackup = tileSR.color;
		tileSR.sprite = groundData.sprite;
	}

	IEnumerator SetTileNeighbor ()
	{
		yield return new WaitForSeconds (0.3f);
		tileNeighbors = new GroundBehavior[4];
	}

	public void ApplyDugColor ()
	{
		if (isDug)
			tileSR.color += dugColorOffset;
		else
			tileSR.color -= dugColorOffset;
	}

	/// <summary>
	/// Fortifies the tile visually.
	/// This method is in charge of displaying the state of visibility of the fortification of the tile.
	/// Wua. That sounded smart!
	/// </summary>
	/// <param name="state">If set to <c>true</c> state.</param>
	void FortifyTile (bool state)
	{
		if (state) {
			Transform fortifyTile = new GameObject ("Tile Fortify Sprite").transform;
			fortifyTile.SetParent (this.transform);
			fortifyTile.position = this.transform.position;
			SpriteRenderer fortifySR = fortifyTile.gameObject.AddComponent<SpriteRenderer> ();
			fortifySR.sprite = GameMasterScript.instance.GMB.fortifiedSprite;
			fortifySR.sortingLayerName = "BackgroundOverlay";
		} else {
			Transform fortifiedTile = this.transform.FindChild ("Tile Fortify Sprite");

			if (fortifiedTile != null)
				Destroy (fortifiedTile.gameObject);
		}
	}

	void SubscribeToCollapseTick (bool state)
	{
		if (!groundData.canCollapse)
			return;
		
		if (state)
			GameMasterScript.instance.TMB.OnNewTickE += WaitToCollapse;
		else
			GameMasterScript.instance.TMB.OnNewTickE -= WaitToCollapse;

	}

	void WaitToCollapse ()
	{
		if (isCollapsing)
			return;
					
		//Because it ended up being necessary..
		if (isFortified) {
			SubscribeToCollapseTick (false);
			return;
		}

		collapseHP--;

		for (int i = 0; i < tileNeighbors.Length; i++) {
			if (tileNeighbors [i].isDug && !tileNeighbors [i].isFortified)
				collapseHP--;
		}

		Debug.Log ("Tile slowly collapsing.. " + collapseHP.ToString () + " hp left");

		if (collapseHP <= 0) {
			Collapse ();
			SubscribeToCollapseTick (false);
		}
	}

	void Collapse ()
	{
		//Flag so i won't loop !
		if (isCollapsing)
			return;

		isCollapsing = true;

		//Find the nearest tile above that's not dug !
		GroundBehavior tileAbove = this;
		while (true) {
			tileAbove = GameMasterScript.instance.GMB.currentGrid.tiles [(int)tileAbove.tilePos.x, (int)tileAbove.tilePos.y - 1];
			if (!tileAbove.isDug)
				break;

			//Make sure you don't go out the map for some reason..
			if (tileAbove.tilePos.y <= 3)
				return;
		}


		//duplicate that tile over all the undug tiles below
		GroundBehavior tileBelow = GameMasterScript.instance.GMB.currentGrid.tiles [(int)tileAbove.tilePos.x, (int)tileAbove.tilePos.y + 1];

		do {

			if (!tileBelow.isDug)
				break;

			//Give it current groundData and reset tile ! But first, check if the tile above is undestructible. If so, then keep the currentData
			if (tileAbove.groundData.hp < 1000)
				tileBelow.groundData = tileAbove.groundData;


			//Unfortify it if needed
			if (tileBelow.isFortified)
				tileBelow.FortifyTile (false);

			//If any player, kill them
			if (tileBelow.unitsOnTile.Count > 0) {
				for (int i = 0; i < tileBelow.unitsOnTile.Count; i++)
					tileBelow.unitsOnTile [i].OnDeathEnter ();
			}

			//Effect
			tileBelow.CollapseEffect ();

			//Setup
			tileBelow.SetupGround ();
		

			//Make sure you don't go out the map for some reason..
			if (tileBelow.tilePos.y >= GameMasterScript.instance.gridHeight - 3)
				return;

			//PROGRESS !
			tileBelow = GameMasterScript.instance.GMB.currentGrid.tiles [(int)tileBelow.tilePos.x, (int)tileBelow.tilePos.y + 1];


		} while (tileBelow.isDug);
	}

	public void CollapseEffect ()
	{
		//Do some funky stuff !
	}

}
