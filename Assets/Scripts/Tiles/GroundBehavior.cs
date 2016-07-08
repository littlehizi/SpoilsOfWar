using UnityEngine;
using System.Collections;

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

	//Stats
	public int hp;
	public float digRes;
	public Sprite sprite;
	public int moveCost;

	//Flags
	public bool isDug;
	public bool isOccupied;

	//Extract the data from given GroundData
	public void SetupGround ()
	{
		hp = groundData.hp;
		digRes = groundData.digRes;
		sprite = groundData.sprite;
		moveCost = groundData.moveCost;
	}
}
