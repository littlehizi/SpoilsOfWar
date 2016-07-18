using UnityEngine;
using System.Collections;

public class VictoryManagerBehavior : MonoBehaviour, IManager
{

	public void OnGameStart ()
	{
		//Find all units and add VictoryCheck to their event
	}

	/// <summary>
	/// This method is attached to the OnTileEnter event on each unit.
	/// </summary>
	public void VictoryCheck ()
	{
		//Check for human victory
		for (int i = 0; i < GameMasterScript.instance.PLMB.humanPlayer.storedUnits.Count; i++) {
			for (int k = 0; k < GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles.Length; k++) {
				if (GameMasterScript.instance.PLMB.humanPlayer.storedUnits [i].currentTile.ID == GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [k].ID)
					((State_Game)GameMasterScript.currentState).GameOver (PlayerData.TypeOfPlayer.human);
			}
		}

		//Check for enemy victory
		for (int i = 0; i < GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Count; i++) {
			for (int k = 0; k < GameMasterScript.instance.PLMB.humanPlayer.spawnTiles.Length; k++) {
				if (GameMasterScript.instance.PLMB.enemyPlayer.storedUnits [i].currentTile.ID == GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [k].ID)
					((State_Game)GameMasterScript.currentState).GameOver (PlayerData.TypeOfPlayer.enemy);
			}
		}
	}
}
