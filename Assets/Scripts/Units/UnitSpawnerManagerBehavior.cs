using UnityEngine;
using System.Collections;

public class UnitSpawnerManagerBehavior : MonoBehaviour, IManager
{
	public GameObject UnitPrefab;

	public UnitData[] unitTypes;

	public void OnGameStart ()
	{
	}

	/// <summary>
	/// Spawns the unit based on a UnitData.UnitType and a GroundBehavior tile.
	/// </summary>
	/// <returns>The unit.</returns>
	/// <param name="newType">New type.</param>
	/// <param name="newTile">New tile.</param>
	public static UnitBehavior SpawnUnit (UnitData.UnitType newType, GroundBehavior newTile)
	{
		//Prepare spawn point
		Vector3 spawnPos = new Vector3 (newTile.tilePos.x, -newTile.tilePos.y, -2);

		//Spawn
		UnitData correctData = GameMasterScript.instance.USMB.unitTypes [0];

		for (int i = 0; i < GameMasterScript.instance.USMB.unitTypes.Length; i++) {
			if (GameMasterScript.instance.USMB.unitTypes [i].typeOfUnit == newType)
				correctData = GameMasterScript.instance.USMB.unitTypes [i];
		}

		//Spawn unit
		UnitBehavior newUnit = ((GameObject)Instantiate (GameMasterScript.instance.USMB.UnitPrefab, spawnPos, Quaternion.identity)).GetComponent<UnitBehavior> ();
		newUnit.gameObject.name = newType.ToString () + " unit";

		//Give data to unit. UNIT IS FREEEEEEE
		newUnit.unitData = correctData;

		//Run unit setup
		newUnit.SetupUnit (newTile);

		return newUnit;
	}
}
