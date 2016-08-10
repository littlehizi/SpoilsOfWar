using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State_Game : BaseState
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

	public State_Game ()
	{
		//Will be run when state is created (During assembly compiling)
	}

	public override void OnStateEnter ()
	{
		//Will be run every time entering this state
		isPaused = false;

		//Display Game HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.game, true);

		//Start the game
		GameMasterScript.instance.StartGame ();

		//Allow player inputs
		GameMasterScript.instance.IMB.currentState = InputManagerBehavior.InputState.idle;

		//Set both player's resources and allow them to gather resources 
		GameMasterScript.instance.RMB.SetBaseResources ();
		GameMasterScript.instance.RMB.canGetResources = true;
		GameMasterScript.instance.UIMB.G_UpdateResourcesUI ();
	}

	public void Update ()
	{
		//Just check for escape to pause the game. Only that. Kinda disappointing, isn't it?
		if (Input.GetKeyDown (KeyCode.Escape))
			isPaused = !isPaused;
	}

	public void ButtonPressed (UserInterfaceManagerBehavior.G_ButtonType newButton)
	{
		switch (newButton) {
		case UserInterfaceManagerBehavior.G_ButtonType.restart:
			GameMasterScript.ChangeState<State_Game> ();
			break;
		case UserInterfaceManagerBehavior.G_ButtonType.goToMainMenu:
			//GO TO MAIN MENU
			GameMasterScript.ChangeState<State_MainMenu> ();
			break;
		case UserInterfaceManagerBehavior.G_ButtonType.unpause:
			isPaused = false;
			break;
		case UserInterfaceManagerBehavior.G_ButtonType.controls:
			break;
		}
	}

	public void GameOver (PlayerData.TypeOfPlayer newWinner)
	{
		Debug.Log ("Game over ! " + newWinner + " is the winner!");

		//Stop the game
		GameMasterScript.instance.IMB.currentState = InputManagerBehavior.InputState.disabled;

		//Stop all units from doing whatever
		for (int i = 0; i < GameMasterScript.instance.PLMB.humanPlayer.storedUnits.Count; i++)
			GameMasterScript.instance.PLMB.humanPlayer.storedUnits [i].StopEverythingies ();

		for (int i = 0; i < GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Count; i++)
			GameMasterScript.instance.PLMB.enemyPlayer.storedUnits [i].StopEverythingies ();


		//Display endgame screen
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.endGame, true);
	}

	public override void OnStateExit ()
	{
		//Will be run every time leaving this state

		//Destroy all units
		List<UnitBehavior> tmpUnitsToKill = new List<UnitBehavior> ();

		for (int i = 0; i < GameMasterScript.instance.PLMB.humanPlayer.storedUnits.Count; i++) {
			tmpUnitsToKill.Add (GameMasterScript.instance.PLMB.humanPlayer.storedUnits [i]);
		}
		for (int i = 0; i < GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Count; i++) {
			tmpUnitsToKill.Add (GameMasterScript.instance.PLMB.enemyPlayer.storedUnits [i]);
		}

		for (int i = 0; i < tmpUnitsToKill.Count; i++)
			tmpUnitsToKill [i].OnDeathEnter ();

		//Destroy all Grids and tiles
		foreach (GroundBehavior tmpTile in GameMasterScript.instance.GMB.currentGrid.tiles)
			Destroy (tmpTile.gameObject);

		foreach (FOWTileBehavior tmpTile in GameMasterScript.instance.FOWMB.fowSRStorage)
			Destroy (tmpTile.gameObject);

		GameMasterScript.instance.FOWMB.visionBeacons.Clear ();
		GameMasterScript.instance.FOWMB.tmpTileScanStorage.Clear ();

		Destroy (GameObject.Find ("Grid"));
		Destroy (GameObject.Find ("FogOfWar Holder"));

		//Stop the tick, time and resource from working
		GameMasterScript.instance.RMB.StopAllCoroutines ();
		GameMasterScript.instance.TMB.canTick = false;
		GameMasterScript.instance.TIMB.isTimeRunning = false;

		//Hide game HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.game, false);
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.endGame, false);

		//Reset pause
		Time.timeScale = 1;
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.pauseMenu, false);

	}
}
