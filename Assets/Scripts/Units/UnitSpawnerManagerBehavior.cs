using UnityEngine;
using System.Collections;

public class UnitSpawnerManagerBehavior : MonoBehaviour, IManager
{
	public GameObject UnitPrefab;

	public void OnGameStart ()
	{
	}

	public static UnitBehavior SpawnUnit (UnitData.UnitType newType, GroundBehavior newTile)
	{
		//Prepare spawn point
		Vector3 spawnPos = new Vector3 (newTile.tilePos.x, -newTile.tilePos.y, -2);

		//Spawn
		UnitBehavior newUnit = ((GameObject)Instantiate (GameMasterScript.instance.USMB.UnitPrefab, spawnPos, Quaternion.identity)).GetComponent<UnitBehavior> ();

		//Run unit setup
		newUnit.SetupUnit (newTile);

		return newUnit;
	}
}
