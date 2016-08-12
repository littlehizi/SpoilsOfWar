using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

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
		characterSelection,
		game,
		pauseMenu,
		endGame,
		controls,
		credits,
		quitConfirm
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
		goToMainMenu,
		showCharacterSelection,
		showControls,
		showCredits,
		showQuit,
		confirmQuit
	}

	public enum CS_ButtonType
	{
		addCharacter,
		showInfo,
		closeInfo,
		confirm
	}

	[System.Serializable]
	public class SelectionFile
	{
		public GameObject openFile, closedFile;
		public Text className;
		public Image medal;
	}


	//Internal variables
	public Button[] MM_mainMenuButtons;
	public SelectionFile[] CS_selectionFiles;

	public void MM_ButtonPressed (int buttonID)
	{
		((State_MainMenu)GameMasterScript.currentState).ButtonPressed ((MM_ButtonType)buttonID);
	}

	public void MM_LockMainMenuButtons (bool isLocked)
	{
		for (int i = 0; i < MM_mainMenuButtons.Length; i++)
			MM_mainMenuButtons [i].interactable = isLocked;
	}

	//Character Selection

	public void CS_AddCharacter (int index)
	{
		((State_MainMenu)GameMasterScript.currentState).CS_ButtonPressed (CS_ButtonType.addCharacter, index);
	}

	public void CS_DisplayCharacterFile (int index, bool canDisplay)
	{
		CS_selectionFiles [index].openFile.SetActive (canDisplay);
		CS_selectionFiles [index].closedFile.SetActive (!canDisplay);
	}

	public void CS_FillCharacterInfo (UnitData unitData, int index)
	{
		CS_DisplayCharacterFile (index, true);
		CS_selectionFiles [index].className.text = unitData.name;
		CS_selectionFiles [index].medal.sprite = unitData.medal;
	}

	public void CS_ShowInfo (int index)
	{
		((State_MainMenu)GameMasterScript.currentState).CS_ButtonPressed (CS_ButtonType.showInfo, index);
	}

	public void CS_Confirm ()
	{
		((State_MainMenu)GameMasterScript.currentState).CS_ButtonPressed (CS_ButtonType.confirm);
	}

	#endregion

	#region Game

	public enum G_ButtonType
	{
		restart,
		goToMainMenu,
		unpause,
		controls
	}

	[System.Serializable]
	public class CharacterFile
	{
		public GameObject openFile, closedFile;
		public Slider healthSlider, staminaSlider, oxygenSlider;
		public Image picture;
		public Image medal;
	}

	//Internal variables
	public Text G_playerResources;
	public Text G_enemyResources;
	public CharacterFile[] G_characterFiles;
	public RectTransform clockHandleBig;
	public RectTransform clockHandleSmall;
	public Image[] daysLeft;
	public Sprite[] dayNumbers;

	public void G_ButtonPressed (int buttonID)
	{
		((State_Game)GameMasterScript.currentState).ButtonPressed ((G_ButtonType)buttonID);
	}

	public void G_UpdateResourcesUI ()
	{
		G_playerResources.text = GameMasterScript.instance.PLMB.humanPlayer.resources.ToString ("D4");
		G_enemyResources.text = GameMasterScript.instance.PLMB.enemyPlayer.resources.ToString ("D4");
	}

	public void G_SetupCharacterFile (CharacterFile newFile, UnitData unitData)
	{
		newFile.medal.sprite = unitData.medal;
		newFile.picture.sprite = unitData.filePicture;
	}

	public void G_DisplayCharacterFile (int index, bool canDisplay)
	{
		G_characterFiles [index].openFile.SetActive (canDisplay);
		G_characterFiles [index].closedFile.SetActive (!canDisplay);
	}

	public void G_DisplayCharacterFile (CharacterFile newFile, bool canDisplay)
	{
		newFile.openFile.SetActive (canDisplay);
		newFile.closedFile.SetActive (!canDisplay);
	}

	bool[] G_isDisplaying = new bool[6]{ false, false, false, false, false, false };

	public void G_DisplayCharacterFile (int file)
	{
		G_DisplayCharacterFile (G_characterFiles [file], !G_isDisplaying [file]);
		bool check = G_isDisplaying [file];
		G_isDisplaying [file] = !G_isDisplaying [file];

		if (check != G_isDisplaying [file])
			((State_Game)GameMasterScript.currentState).SelectNewUnitByClickingOnFile (file, G_isDisplaying [file]);

	}

	public void G_UpdateSlider (Slider newSlider, float newValue)
	{
		newSlider.value = newValue;
	}

	public void G_UpdateTime (float hours, float mins, int days)
	{
		//Rotate
		clockHandleBig.transform.rotation = Quaternion.Euler (-Vector3.forward * 15 * hours);
		clockHandleSmall.transform.rotation = Quaternion.Euler (-Vector3.forward * 15 * mins);

		//Time
		int firstDiggit = Mathf.FloorToInt (((float)days) / 10.0f);
		int secondDiggit = days % 10;

		daysLeft [0].sprite = dayNumbers [firstDiggit];
		daysLeft [1].sprite = dayNumbers [secondDiggit];
	}

	#endregion
}
