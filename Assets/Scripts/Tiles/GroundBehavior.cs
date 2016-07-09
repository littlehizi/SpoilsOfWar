using UnityEngine;
using System.Collections;

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

	//Main Info
	public BaseGroundData groundData;
	public GroundType typeOfGround;
	public Vector2 tilePos;
	public int ID;

	//Stats
	public int hp;
	public float digRes;
	public Sprite sprite;
	public int moveCost;

	//Flags
	public bool isDug;
	public bool isOccupied;

	//PathFinding
	[HideInInspector]public GroundBehavior parent;

	//Extract the data from given GroundData
	public void SetupGround ()
	{
		hp = groundData.hp;
		digRes = groundData.digRes;
		sprite = groundData.sprite;
		moveCost = groundData.moveCost;
	}
}
