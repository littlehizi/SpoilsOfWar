using UnityEngine;
using System.Collections;

public class State_MainMenu : BaseState
{

	public State_MainMenu ()
	{
		//Will be run when state is created (During assembly compiling)
	}

	public override void OnStateEnter ()
	{
		//Will be run every time entering this state

		//Enable the main menu HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.mainMenu, true);
	}

	public void ButtonPressed (UserInterfaceManagerBehavior.MM_ButtonType newButton)
	{
		switch (newButton) {
		case UserInterfaceManagerBehavior.MM_ButtonType.startGame:
			//If the start game button has been pressed, GET THE GAME STARTED
			GameMasterScript.ChangeState<State_Game> ();
			break;
		}
	}


	public override void OnStateExit ()
	{
		//Will be run every time leaving this state

		//Disable the main menu HUD
		GameMasterScript.instance.UIMB.DisplayHUD (UserInterfaceManagerBehavior.TypeOfHUD.mainMenu, false);

	}

}
