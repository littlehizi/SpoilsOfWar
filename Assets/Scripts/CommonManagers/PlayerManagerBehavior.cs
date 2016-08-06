using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManagerBehavior : MonoBehaviour, IManager
{
	public PlayerData humanPlayer;
	public PlayerData enemyPlayer;

	public enum EnemyBaseUnitType
	{
		twoOfEach,
		random,
		diggerFighter,
		fighterListener
	}

	public EnemyBaseUnitType enemyTeamComposition;

	void Awake ()
	{
		humanPlayer.storedUnits = new List<UnitBehavior> ();
		enemyPlayer.storedUnits = new List<UnitBehavior> ();
	}

	public void OnGameStart ()
	{
		//Clear lists
		humanPlayer.storedUnits.Clear ();
		enemyPlayer.storedUnits.Clear ();

		humanPlayer.spawnTiles = new GroundBehavior[6];
		enemyPlayer.spawnTiles = new GroundBehavior[6];
	}

	public void SpawnBaseUnit ()
	{
		//Initalize the spawn tiles
		GameMasterScript.instance.GMB.InitalizeSpawnTiles ();

		// Human Units
		for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length; i++)
			humanPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.playerCharacters [i].typeOfUnit, 
					humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.human));


		switch (enemyTeamComposition) {
		case EnemyBaseUnitType.random:
		// Enemy Units. Summon randomly !
			for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length; i++)
				enemyPlayer.storedUnits.Add (
					UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [Random.Range (0, GameMasterScript.instance.USMB.unitTypes.Length)].typeOfUnit, 
						enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
						PlayerData.TypeOfPlayer.enemy));
			break;
		case EnemyBaseUnitType.twoOfEach:
			enemyPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Digger].typeOfUnit, 
					enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.enemy));
			enemyPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Digger].typeOfUnit, 
					enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.enemy));

			enemyPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Fighter].typeOfUnit, 
					enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.enemy));
			enemyPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Fighter].typeOfUnit, 
					enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.enemy));

			enemyPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Listener].typeOfUnit, 
					enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.enemy));
			enemyPlayer.storedUnits.Add (
				UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Listener].typeOfUnit, 
					enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
					PlayerData.TypeOfPlayer.enemy));
			break;
		case EnemyBaseUnitType.diggerFighter:
			for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length / 2; i++)
				enemyPlayer.storedUnits.Add (
					UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Digger].typeOfUnit, 
						enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
						PlayerData.TypeOfPlayer.enemy));

			for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length / 2; i++)
				enemyPlayer.storedUnits.Add (
					UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Fighter].typeOfUnit, 
						enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
						PlayerData.TypeOfPlayer.enemy));
			break;
		case EnemyBaseUnitType.fighterListener:
			for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length / 2; i++)
				enemyPlayer.storedUnits.Add (
					UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Fighter].typeOfUnit, 
						enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
						PlayerData.TypeOfPlayer.enemy));

			for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length / 2; i++)
				enemyPlayer.storedUnits.Add (
					UnitSpawnerManagerBehavior.SpawnUnit (GameMasterScript.instance.USMB.unitTypes [(int)UnitData.UnitType.Listener].typeOfUnit, 
						enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], 
						PlayerData.TypeOfPlayer.enemy));
			break;
		}
	}
}
