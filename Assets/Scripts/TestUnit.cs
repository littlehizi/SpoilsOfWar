using UnityEngine;
using System.Collections;

public class TestUnit : MonoBehaviour
{

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.K)) {
			GroundBehavior summonTile = GameMasterScript.instance.GMB.currentGrid.tiles [0, 5]; 

			summonTile.isDug = true;

			UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Digger, summonTile);

			foreach (GroundBehavior GB in GameMasterScript.instance.GMB.currentGrid.tiles)
				if (GB.tilePos.y < 10)
					GB.isDug = true;
		}
	}
}
