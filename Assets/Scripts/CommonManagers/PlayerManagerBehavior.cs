using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManagerBehavior : MonoBehaviour, IManager
{
	public PlayerData humanPlayer;
	public PlayerData enemyPlayer;


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
		humanPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Fighter, humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human));
		humanPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Digger, humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human));
		humanPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Listener, humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human));
		humanPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Fighter, humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human));
		humanPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Digger, humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human));
		humanPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Listener, humanPlayer.spawnTiles [Random.Range (0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human));

		// Enemy Units
		enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Fighter, enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy));
		enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Digger, enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy));
		enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Listener, enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy));
		enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Fighter, enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy));
		enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Digger, enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy));
		enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (UnitData.UnitType.Listener, enemyPlayer.spawnTiles [Random.Range (0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy));
	}
		
}
