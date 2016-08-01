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

	private bool _isFortified;

	public bool isFortified {
		get{ return _isFortified; }
		set {
			_isFortified = value;
			if (_isFortified)
				FortifyTile ();
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
		isFortified = false;

		//Get stats
		hp = groundData.hp;
		digRes = groundData.digRes;
		sprite = groundData.sprite;
		moveCost = groundData.moveCost;

		//Temporary stuff
		colorBackup = tileSR.color = tileSR.color;
	}

	public void ApplyDugColor ()
	{
		if (isDug)
			tileSR.color -= dugColorOffset;
		else
			tileSR.color += dugColorOffset;
	}

	void FortifyTile ()
	{
		Transform fortifyTile = new GameObject ("Tile Fortify Sprite").transform;
		fortifyTile.SetParent (this.transform);
		fortifyTile.position = this.transform.position;
		SpriteRenderer fortifySR = fortifyTile.gameObject.AddComponent<SpriteRenderer> ();
		fortifySR.sprite = GameMasterScript.instance.GMB.fortifiedSprite;
		fortifySR.sortingLayerName = "BackgroundOverlay";
	}
}
