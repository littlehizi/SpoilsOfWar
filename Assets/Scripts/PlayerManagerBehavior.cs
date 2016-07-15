using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManagerBehavior : MonoBehaviour, IManager
{
    public PlayerData humanPlayer;
    public PlayerData enemyPlayer;


    public void OnGameStart()
    {
        humanPlayer.spawnTiles = new GroundBehavior[6];
        enemyPlayer.spawnTiles = new GroundBehavior[6];

        humanPlayer.storedUnits = new List<UnitBehavior>();
        enemyPlayer.storedUnits = new List<UnitBehavior>();
    }

    public void SpawnBaseUnit()
    {
        // Human Units
        UnitSpawnerManagerBehavior.SpawnUnit(UnitData.UnitType.Fighter, humanPlayer.spawnTiles[Random.Range(0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human);
        UnitSpawnerManagerBehavior.SpawnUnit(UnitData.UnitType.Digger, humanPlayer.spawnTiles[Random.Range(0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human);
        UnitSpawnerManagerBehavior.SpawnUnit(UnitData.UnitType.Listener, humanPlayer.spawnTiles[Random.Range(0, humanPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.human);

        // Enemy Units
        UnitSpawnerManagerBehavior.SpawnUnit(UnitData.UnitType.Fighter, enemyPlayer.spawnTiles[Random.Range(0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy);
        UnitSpawnerManagerBehavior.SpawnUnit(UnitData.UnitType.Digger, enemyPlayer.spawnTiles[Random.Range(0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy);
        UnitSpawnerManagerBehavior.SpawnUnit(UnitData.UnitType.Listener, enemyPlayer.spawnTiles[Random.Range(0, enemyPlayer.spawnTiles.Length)], PlayerData.TypeOfPlayer.enemy);
    }

}
