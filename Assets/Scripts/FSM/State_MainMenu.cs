using UnityEngine;
using System.Collections;

public class State_MainMenu : BaseState
{
	#region SUBFSM

	public enum SubState
	{
		mainMenu,
		characterSelection,
		controls,
		credits,
		quitConfirm
	}

	private SubState _currentState;

	public SubState currentState {
		get{ return _currentState; }
		set { 
			//When setting a state, always disble the previous HUD and enable the next one.
			//Main Menu is the only exception

			switch (_currentState) {
			case SubState.mainMenu:
				//Do a lot of nothingies ! Actually, disable the main menu buttons for the time being.
				GameMasterScript.instance.UIMB.MM_LockMainMenuButtons (false);
				break;
			case SubState.characterSelection:
				//hide characterselection HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.characterSelection, false);
				//Close individual slots too
				for (int i = 0; i < GameMasterScript.instance.UIMB.CS_selectionFiles.Length; i++)
					GameMasterScript.instance.UIMB.CS_DisplayCharacterFile (i, false);
				break;
			case SubState.controls:
				//hide controls HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.controls, false);
				break;
			case SubState.credits:
				//hide credits HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.credits, false);
				break;
			case SubState.quitConfirm:
				//hide quit HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.quitConfirm, false);
				break;
			}

			//update the current state
			_currentState = value;

			Debug.Log ("menu substate: " + _currentState.ToString ());

			//Enable current HUD
			switch (_currentState) {
			case SubState.mainMenu:
				//Enable main menu buttons
				GameMasterScript.instance.UIMB.MM_LockMainMenuButtons (true);
				break;
			case SubState.characterSelection:
				//display characterselection HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.characterSelection, true);
				//Reset character selection data
				GameMasterScript.instance.playerCharacters = new UnitData[GameMasterScript.instance.playerCharacters.Length];
				CS_currentIndex = 0;
				break;
			case SubState.controls:
				//display controls HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.controls, true);
				break;
			case SubState.credits:
				//display credits HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.credits, true);
				break;
			case SubState.quitConfirm:
				//display quit HUD
				GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.quitConfirm, true);
				break;
			}
		}
	}

	#endregion

	//internal variables
	int CS_currentIndex;

	public State_MainMenu ()
	{
		//Will be run when state is created (During assembly compiling)
	}

	public override void OnStateEnter ()
	{
		//Will be run every time entering this state

		//Reset data
		currentState = SubState.mainMenu;

		//Enable the main menu HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.mainMenu, true);
	}

	/// <summary>
	/// This method is responsible for handling all the common UI inputs (on main menu)
	/// </summary>
	/// <param name="newButton">New button.</param>
	public void ButtonPressed (UserInterfaceManagerBehavior.MM_ButtonType newButton)
	{
		switch (newButton) {
		case UserInterfaceManagerBehavior.MM_ButtonType.startGame:
			//If the start game button has been pressed, GET THE GAME STARTED
			break;
		case UserInterfaceManagerBehavior.MM_ButtonType.showCharacterSelection:
			//go to character selection
			currentState = SubState.characterSelection;
			break;
		case UserInterfaceManagerBehavior.MM_ButtonType.showControls:
			//go to display controls
			currentState = SubState.controls;
			break;
		case UserInterfaceManagerBehavior.MM_ButtonType.showCredits:
			//go to display credits
			currentState = SubState.credits;
			break;
		case UserInterfaceManagerBehavior.MM_ButtonType.showQuit:
			//go to display quit button
			currentState = SubState.quitConfirm;
			break;
		case UserInterfaceManagerBehavior.MM_ButtonType.confirmQuit:
			//QUIT THE GAAAAME
			QuitGame ();
			break;
		case UserInterfaceManagerBehavior.MM_ButtonType.goToMainMenu:
			//Go backies to main menu
			currentState = SubState.mainMenu;
			break;
		}
	}


	/// <summary>
	/// This method is responsible for handling the inputs for the Character Selection
	/// </summary>
	public void CS_ButtonPressed (UserInterfaceManagerBehavior.CS_ButtonType newButton, int index = 0)
	{
		switch (newButton) {
		case UserInterfaceManagerBehavior.CS_ButtonType.addCharacter:
			//Check if the player already has enough characters
			if (CS_currentIndex >= GameMasterScript.instance.playerCharacters.Length) {
				Debug.LogWarning ("The player got enough characters already !!!");
				return;
			}

			//Display it too!
			GameMasterScript.instance.UIMB.CS_FillCharacterInfo (GameMasterScript.instance.USMB.unitTypes [index], CS_currentIndex);

			//If not, then add it!
			GameMasterScript.instance.playerCharacters [CS_currentIndex++] = GameMasterScript.instance.USMB.unitTypes [index];

			break;
		case UserInterfaceManagerBehavior.CS_ButtonType.showInfo:
			//Display the correct class info according to the given index
			break;
		case UserInterfaceManagerBehavior.CS_ButtonType.confirm:
			//Check if the player has selected all characters
			for (int i = 0; i < GameMasterScript.instance.playerCharacters.Length; i++) {
				if (GameMasterScript.instance.playerCharacters [i] == null) {
					Debug.LogWarning ("Player needs to select more characters to continue !!");
					return;
				}
			}

			//If so, then start the game !
			StartGame ();
			break;
		}
	}


	public override void OnStateExit ()
	{
		//Will be run every time leaving this state

		//Reset the state
		currentState = SubState.mainMenu;

		//Disable the main menu HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.mainMenu, false);

	}

	void StartGame ()
	{
		GameMasterScript.ChangeState<State_Game> ();
	}

	void QuitGame ()
	{
		Application.Quit ();
	}


}
