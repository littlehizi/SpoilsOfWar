using UnityEngine;
using System.Collections;

/// <summary>
/// UI manager behavior.
/// This class is responsible for handling the UI's actions, and send the results to the current state
/// This manager will not be initlialized during StartGame, since it can be used on Mainmenu
/// THIS CLASS IS A COMMUNICATION MEDIUM, DO NOT ABUSE IT TOO MUCH (although you can)
/// </summary>
public class UserInterfaceManagerBehavior : MonoBehaviour, IManager
{

	public enum TypeOfHUD
	{
		mainMenu,
		game,
		endGame
	}

	public GameObject[] HUDs;

	public void OnGameStart ()
	{
		
	}

	public void DisplayHUD (TypeOfHUD newType, bool wantToDisplay)
	{
		HUDs [(int)newType].SetActive (wantToDisplay);
	}

	#region Main Menu

	public enum MM_ButtonType
	{
		startGame,
		nothing,
		moreNothing,
		nothingAgain
	}

	public void MM_ButtonPressed (int buttonID)
	{
		((State_MainMenu)GameMasterScript.currentState).ButtonPressed ((MM_ButtonType)buttonID);
	}

	#endregion

	#region Game

	public enum G_ButtonType
	{
		restart,
		goToMainMenu
	}

	public void G_ButtonPressed (int buttonID)
	{
		((State_Game)GameMasterScript.currentState).ButtonPressed ((G_ButtonType)buttonID);
	}

	#endregion
}
