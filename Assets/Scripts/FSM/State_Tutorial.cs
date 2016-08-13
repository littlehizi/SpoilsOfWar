using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State_Tutorial : BaseState
{

	#region FSM

	InputManagerBehavior.InputState backupState;

	private bool _isPaused;

	public bool isPaused {
		get{ return _isPaused; }
		set { 
			_isPaused = value;

			//STOP TIME ! time powers are awesome ! Also, set inputs !
			if (isPaused) {
				Time.timeScale = 0.0f;
				backupState = GameMasterScript.instance.IMB.currentState;
				GameMasterScript.instance.IMB.currentState = InputManagerBehavior.InputState.disabled;
			} else {
				Time.timeScale = 1;
				GameMasterScript.instance.IMB.currentState = backupState;
			}

			//Display pause menu
			GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.pauseMenu, _isPaused);

		}
	}

	#endregion

	//Internal variables
	public bool isGameOver;

	//Backup
	int gridWidth;
	int gridHeight;

	//Initialization
	public override void OnStateEnter ()
	{
		stateID = State.tutorial;

		isPaused = false;
		isGameOver = false;

		//Show HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.game, true);

		//SPAWN FIELD
		gridWidth = GameMasterScript.instance.gridWidth;
		gridHeight = GameMasterScript.instance.gridHeight;
		GameMasterScript.instance.gridWidth = GameMasterScript.instance.tutorialGridWidth;
		GameMasterScript.instance.gridHeight = GameMasterScript.instance.tutorialGridHeight;

		GameMasterScript.instance.SetManagers ();
		GameMasterScript.instance.GMB.InitalizeSpawnTiles ();

		//Set both player's resources and allow them to gather resources 
		GameMasterScript.instance.RMB.SetBaseResources ();
		GameMasterScript.instance.RMB.canGetResources = true;
		GameMasterScript.instance.UIMB.G_UpdateResourcesUI ();
	}

	/// <summary>
	/// In here happen all the scritabled events of the tutorial level
	/// </summary>
	IEnumerator Start ()
	{
		//Allow player inputs
		GameMasterScript.instance.IMB.currentState = InputManagerBehavior.InputState.idle;

		//Spawn ally digger
		UnitBehavior allyDigger = GameMasterScript.instance.PLMB.SpawnSingleUnit (UnitData.UnitType.Digger, PlayerData.TypeOfPlayer.human, GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [Random.Range (0, 6)]);

		//Spawn enemy listener
		GroundBehavior listenerSpawnTile = GameMasterScript.instance.GMB.TileposToTile (new Vector2 (GameMasterScript.instance.tutorialGridWidth / 2 + 2, GameMasterScript.instance.tutorialGridHeight / 3 * 2));
		listenerSpawnTile.hp = 0;
		listenerSpawnTile.isFortified = true;

		UnitBehavior enemyListener = GameMasterScript.instance.PLMB.SpawnSingleUnit (UnitData.UnitType.Listener, PlayerData.TypeOfPlayer.enemy, listenerSpawnTile);
		enemyListener.oxygen = 99999999;
		enemyListener.health = 99999999;

		//Remove obstacles around the listener
		Collider2D[] tilesHit = Physics2D.OverlapCircleAll (listenerSpawnTile.trueTilePos, 5, LayerMask.NameToLayer ("GroundTiles"));

		for (int i = 0; i < tilesHit.Length; i++) {
			tilesHit [i].GetComponent<GroundBehavior> ().isAnObstacle = false;
		}


		//Add a yield return between each audio event? dunno how you guys wanna do it
		yield return new WaitForSeconds (0.5f);

		//Wait for the player to reach 1/3 of the map of the length of the map. He'll find a listener.
		while (GameMasterScript.instance.PLMB.humanPlayer.storedUnits [0].currentTile.tilePos.x < GameMasterScript.instance.tutorialGridWidth / 3) {
			yield return null;
		}
			
		//Once the player reached one third of the map, 1 warrior will join in. wait until the player select it.
		UnitBehavior allyFighter = GameMasterScript.instance.PLMB.SpawnSingleUnit (UnitData.UnitType.Fighter, PlayerData.TypeOfPlayer.human, GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [Random.Range (0, 6)]);

		while (GameMasterScript.instance.SMB.unitSelected.Count == 0 || ((UnitBehavior)GameMasterScript.instance.SMB.unitSelected [0]).unitData.typeOfUnit != UnitData.UnitType.Fighter) {
			yield return null;
		}

		//Once the fighter unit has been selected, wait for the player to select the Listener unit
		UnitBehavior allyListener = GameMasterScript.instance.PLMB.SpawnSingleUnit (UnitData.UnitType.Listener, PlayerData.TypeOfPlayer.human, GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [Random.Range (0, 6)]);

		while (GameMasterScript.instance.SMB.unitSelected.Count == 0 || ((UnitBehavior)GameMasterScript.instance.SMB.unitSelected [0]).unitData.typeOfUnit != UnitData.UnitType.Listener) {
			yield return null;
		}

		UnitBehavior[] playerUnits = new UnitBehavior[3];

		if (allyDigger != null)
			playerUnits [0] = allyDigger;

		if (allyFighter != null)
			playerUnits [1] = allyFighter;

		if (allyListener != null)
			playerUnits [2] = allyListener;

		//Wait until the player sees the listener
		bool check = false;
		while (!check) {
			for (int i = 0; i < playerUnits.Length; i++) {
				if (playerUnits [i] == null)
					continue;

				if (Vector2.Distance (playerUnits [i].currentTile.tilePos, listenerSpawnTile.tilePos) < playerUnits [i].visionStrength) {
					check = true;
					break;
				}
			}

			yield return null;
		}

		//Spawn fighters behind the listener
		UnitBehavior[] enemyFighters = new UnitBehavior[5];
		AIBrainBehavior[] enemyBrains = new AIBrainBehavior[5];
		for (int i = 0; i < enemyFighters.Length; i++) {
			enemyFighters [i] = GameMasterScript.instance.PLMB.SpawnSingleUnit (UnitData.UnitType.Fighter, PlayerData.TypeOfPlayer.enemy, listenerSpawnTile);
			enemyFighters [i].health = 999999;
			enemyFighters [i].stamina = 100;
			enemyBrains [i] = enemyFighters [i].GetComponent<AIBrainBehavior> ();
		}

		//Undig the 4 tiles nearby
		List<GroundBehavior> neighbors = PathfindingManagerBehavior.GetTileNeighbor (listenerSpawnTile);

		for (int i = 0; i < neighbors.Count; i++)
			neighbors [i].hp = 0;

		while (!isGameOver) {
			for (int i = 0; i < enemyBrains.Length; i++) {
				UnitBehavior target = null;
				do {
					target = playerUnits [Random.Range (0, playerUnits.Length)];
				} while(target == null);

				enemyBrains [i].currentTileTarget = target.currentTile;
			}

			yield return null;
		}

		Debug.LogWarning ("Tutorial over!");

		//Redirect to main menu?
		GameMasterScript.ChangeState<State_MainMenu> ();

	}

	//Destruction
	public override void OnStateExit ()
	{
		//RESTORE BACKUP
		GameMasterScript.instance.gridWidth = gridWidth;
		GameMasterScript.instance.gridHeight = gridHeight;

		//hide HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.game, false);
	}
}
