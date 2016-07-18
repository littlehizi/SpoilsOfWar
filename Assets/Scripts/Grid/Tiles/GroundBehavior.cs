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
	public int hp;
	public float digRes;
	public Sprite sprite;
	public int moveCost;

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
}
