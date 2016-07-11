using UnityEngine;
using System.Collections;

public class TestUnit : MonoBehaviour
{

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.K)) {
			GroundBehavior summonTile = GameMasterScript.instance.GMB.currentGrid.tiles [0, 10]; 

			summonTile.isDug = true;

			UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Fighter, summonTile);

			foreach (GroundBehavior GB in GameMasterScript.instance.GMB.currentGrid.tiles)
				GB.isDug = true;
		}
	}
}
