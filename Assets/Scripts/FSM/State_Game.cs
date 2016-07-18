using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State_Game : BaseState
{

	public State_Game ()
	{
		//Will be run when state is created (During assembly compiling)
	}

	public override void OnStateEnter ()
	{
		//Will be run every time entering this state
	
		//Display Game HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.game, true);

		//Start the game
		GameMasterScript.instance.StartGame ();

		//Allow player inputs
		GameMasterScript.instance.IMB.currentState = InputManagerBehavior.InputState.idle;
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
		}
	}

	public void GameOver (PlayerData.TypeOfPlayer newWinner)
	{
		Debug.Log ("Game over ! " + newWinner + " is the winner!");

		//Stop the game
		GameMasterScript.instance.IMB.currentState = InputManagerBehavior.InputState.disabled;

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

		//Hide game HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.game, false);
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.endGame, false);


	}
}
