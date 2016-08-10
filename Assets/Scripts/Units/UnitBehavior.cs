using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBehavior : MonoBehaviour, ISelection, IVision
{
	//References
	[HideInInspector] public UnitMovementBehavior UMB;
	[HideInInspector] public DigBehavior DB;
	[HideInInspector] public CombatBehavior CB;
	[HideInInspector] public BombBehavior BB;

	[HideInInspector] public SpriteRenderer unitSR;

	//ON TILE ENTER EVENT
	public delegate void OnTileEnter ();

	public OnTileEnter OnTileEnterEvent;

	public GroundBehavior currentTile {
		get { return _currentTile; }
		set {
			//Remove the unit from the current tile
			if (_currentTile != null && _currentTile.unitsOnTile.Contains (this))
				_currentTile.unitsOnTile.Remove (this);

			//Set the new tile
			_currentTile = value;

			//Tell the new tile about this unit
			_currentTile.unitsOnTile.Add (this);

			//Run the OnTileEnter Event
			if (OnTileEnterEvent != null)
				OnTileEnterEvent ();

			//Check if was exhausted and running home
			if (isOnSpawnTiles (alignment)) {
				//Restore stamina
				OnStaminaRestored ();
				//Restore bombs
				currentBombsHeld = unitData.amountOfBombs;
			}

//			//Check for oxygen change
//			if (stamina < unitData.stamina && !canPathfindAir ()) {
//				Debug.Log ("Unit is gonna die from oxygen lost !");
//				isLosingOxygen = true;
//				SubscribeToOxygenLossTick (isLosingOxygen);
//
//			} else if (isLosingOxygen && canPathfindAir ()) {
//				isLosingOxygen = false;
//				SubscribeToOxygenLossTick (isLosingOxygen);
//			}
//
//
		}
	}

	//Stats
	public UnitData unitData;
	public int currentBombsHeld;
	public bool canBeSelected;
	public bool isExhausted;
	GroundBehavior _currentTile;

	UserInterfaceManagerBehavior.CharacterFile _currentCharacterFile;

	UserInterfaceManagerBehavior.CharacterFile currentCharacterFile {
		get {
			if (_currentCharacterFile == null) {
				//GetCharacterFile
				int unitIndex = GameMasterScript.instance.PLMB.humanPlayer.storedUnits.FindIndex (i => i.GetHashCode () == this.GetHashCode ());
				_currentCharacterFile = GameMasterScript.instance.UIMB.G_characterFiles [unitIndex];

				//Set it up
				GameMasterScript.instance.UIMB.G_SetupCharacterFile (_currentCharacterFile, unitData);
			}
			return _currentCharacterFile;
		}
	}

	public bool isOnEnemyTrench { 
		get { 
			if (alignment == PlayerData.TypeOfPlayer.human)
				return isOnSpawnTiles (PlayerData.TypeOfPlayer.enemy);
			else
				return isOnSpawnTiles (PlayerData.TypeOfPlayer.human);
		} 
	}

	int _health;

	public int health {
		get { return _health; }
		set {
			_health = value;

			//Update slider
			if (alignment == PlayerData.TypeOfPlayer.human)
				GameMasterScript.instance.UIMB.G_UpdateSlider (currentCharacterFile.healthSlider, (float)_health / (float)unitData.health);

			//If the unit's health falls below zero, it dies to death, but like, deadly deathful death kind of death
			if (_health <= 0)
				OnDeathEnter ();
		}
	}

	int _stamina;

	public int stamina {
		get { return _stamina; }
		set {
			_stamina = value;

			//Update slider
			if (alignment == PlayerData.TypeOfPlayer.human)
				GameMasterScript.instance.UIMB.G_UpdateSlider (currentCharacterFile.staminaSlider, (float)_stamina / (float)unitData.stamina);

			//If the unit's stamina falls below zero, the unit goes backies to the spawn
			if (_stamina <= 0)
				isExhausted = true;
		}
	}

	public int _oxygen;

	public int oxygen {
		get { return _oxygen; }
		set {
			_oxygen = value;
			//if the unit has no more oxygen, the unit dies.

			//Update slider
			if (alignment == PlayerData.TypeOfPlayer.human)
				GameMasterScript.instance.UIMB.G_UpdateSlider (currentCharacterFile.oxygenSlider, (float)_oxygen / 100f);
			
			if (_oxygen <= 0)
				OnDeathEnter ();
		}
	}

	GroundBehavior runAwayTile;

	public PlayerData.TypeOfPlayer alignment;

	public void SetupUnit (GroundBehavior newTile)
	{
		//Get references
		UMB = this.GetComponent<UnitMovementBehavior> ();
		DB = this.GetComponent<DigBehavior> ();
		CB = this.GetComponent<CombatBehavior> ();
		unitSR = this.GetComponent<SpriteRenderer> ();
		BB = this.GetComponent<BombBehavior> ();

		unitSR.sprite = unitData.sprite;
		unitSR.color = unitData.tmpColor;

		//Set temporary data
		_health = unitData.health;
		_stamina = unitData.stamina - 1;
		_oxygen = 100;
		currentBombsHeld = unitData.amountOfBombs;
		SubscribeToOxygenLossTick (true);

		//Set current tile
		_currentTile = newTile;
		_currentTile.unitsOnTile.Add (this);

		//Selection
		canBeSelected = true;
		isExhausted = false;

		if (alignment == PlayerData.TypeOfPlayer.enemy)
			this.GetComponent<AIBrainBehavior> ().SetupAI ();
	}

	public void OnSelect ()
	{
		Debug.Log ("Unit Selected: " + this.gameObject.name);

		//Display the correct file if player)
		if (alignment == PlayerData.TypeOfPlayer.human) {
			GameMasterScript.instance.UIMB.G_DisplayCharacterFile (currentCharacterFile, true);
		}
	}

	#region FSM

	public enum UnitState
	{
		idle,
		walking,
		digging,
		listening,
		fighting,
		dead
	}

	private UnitState _currentState;

	public UnitState currentState {
		get { return _currentState; }
		set { 
			//LEAVE STATE
			switch (_currentState) {
			case UnitState.idle:
				break;
			case UnitState.walking:
				//When leaving the walking state, just make sure the character is done walking
				StopWalking ();
				break;
			case UnitState.digging:
				break;
			case UnitState.listening:
				break;
			case UnitState.fighting:
				break;
			case UnitState.dead:
				break;
			}

			//SET STATE
			_currentState = value;

			//ENTER STATE
			switch (_currentState) {
			case UnitState.idle:
				break;
			case UnitState.walking:
				break;
			case UnitState.digging:
				break;
			case UnitState.listening:
				break;
			case UnitState.fighting:
				break;
			case UnitState.dead:
				break;
			}
		}
	}

	#endregion

	#region Walk

	/// <summary>
	/// This method will communicate with the walk component, as well as setting the unit's state.
	/// </summary>
	/// <param name="destination">Destination.</param>
	public void WalkToTile (GroundBehavior destination)
	{
		currentState = UnitState.walking;

		UMB.MoveUnitToTarget (destination);
	}

	/// <summary>
	/// Stops the walking process.
	/// </summary>
	public void StopWalking ()
	{
		UMB.StopWalking ();
	}

	public void ReachedDestination ()
	{
		currentState = UnitState.idle;
	}

	#endregion

	#region Dig

	public void StartDiggingProcess (GroundBehavior[] tilesToDig)
	{
		//Check if it's not an empty list
		if (tilesToDig == null || tilesToDig.Length == 0)
			return;
		

		StartCoroutine ("DiggingProcess", tilesToDig);
	}

	IEnumerator DiggingProcess (GroundBehavior[] tilesToDig)
	{
		//Check if the first tile is in range. If not, then walk to it.
		if (!PathfindingManagerBehavior.ListContainsGroundTile (PathfindingManagerBehavior.GetTileNeighbor (tilesToDig [0]), currentTile)) {
			//Find the nearest available tile next to the first tile to dig
			//////FOR NOW, JUST HAVE IT WORK, DO NOT CARE ABOUT COMPLEX PATHS
			List<GroundBehavior> neighbors = PathfindingManagerBehavior.GetTileNeighbor (tilesToDig [0]);

			bool hasATileToTalkTo = false;

			for (int i = 0; i < neighbors.Count; i++) {
				if (neighbors [i].isDug) {
					WalkToTile (neighbors [i]);
					hasATileToTalkTo = true;
					break;
				}
			}

			//If none, return without any results (and yell at the player)
			if (!hasATileToTalkTo)
				return false;

			while (currentState == UnitState.walking)
				yield return null;

		}

		currentState = UnitState.digging;

		DB.StartDigging (tilesToDig);
	}

	#endregion

	#region IVision

	public int visionStrength { get { return unitData.vision; } }

	public GroundBehavior visionStartTile { get { return currentTile; } }

	//Register to the FOW vision system
	public void RegisterToIVisionStorage ()
	{
		GameMasterScript.instance.FOWMB.visionBeacons.Add (this);
	}

	//Units are dynamic, so delete entry on death
	public void DeleteIVisionEntry ()
	{
		GameMasterScript.instance.FOWMB.visionBeacons.Remove (this);
	}

	#endregion

	#region Combat

	public void EngageCombat (GroundBehavior enemyTile)
	{
		Debug.Log (this + " engaged combat with enemy!");	
		CB.StartCombat (enemyTile, true);
	}

	public void IsEngagedInCombat (GroundBehavior enemyTile)
	{
		Debug.Log ("An enemy has engaged combat with " + this);
		canBeSelected = false;
		//Stop the unit from doing whatever
		StopEverythingies ();

		CB.StartCombat (enemyTile, false);
	}

	public void OnDeathEnter ()
	{
		//Stop all actions
		UMB.StopWalking ();
		DB.StopDigging ();
		if (CB.isFighting)
			CB.StopCombat ();

		UMB.StopAllCoroutines ();
		DB.StopAllCoroutines ();

		//Reset dig path
		DigSelectionManagerBehavior.ResetDigPath ();

		//Desubscribe to oxygen loss event just in case
		SubscribeToOxygenLossTick (false);

		//Deselect unit (in case it was)
		if (GameMasterScript.instance.SMB.unitSelected.Contains (this))
			GameMasterScript.instance.SMB.DeselectUnit (this);

		//Remove unit from unit on tile storage
		currentTile.unitsOnTile.Remove (this);

		//VISION
		DeleteIVisionEntry ();

		//Victory
		//OnTileEnterEvent -= GameMasterScript.instance.VMB.VictoryCheck;

		//Remove itself from the player unitstorage
		switch (alignment) {
		case PlayerData.TypeOfPlayer.human:
			GameMasterScript.instance.PLMB.humanPlayer.storedUnits.Remove (this);
			break;
		case PlayerData.TypeOfPlayer.enemy:
			GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Remove (this);
			break;
		}

		//Tell the AI if it's an enemy unit
		if (alignment == PlayerData.TypeOfPlayer.enemy)
			this.GetComponent<AIBrainBehavior> ().AIDeathEnter ();

		//Destroy unit
		Destroy (this.gameObject);
	}

	#endregion

	#region Oxygen

	bool canPathfindAir ()
	{
		GroundBehavior spawnTile = null;

		if (alignment == PlayerData.TypeOfPlayer.human) {
			spawnTile = GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [0];
		} else {
			spawnTile = GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [0];
		}

		GroundBehavior[] path = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, currentTile, spawnTile, true);

		return path != null;
	}

	void SubscribeToOxygenLossTick (bool state)
	{
		if (state)
			GameMasterScript.instance.TMB.OnNewTickE += OnOxygenLoss;
		else
			GameMasterScript.instance.TMB.OnNewTickE -= OnOxygenLoss;
	}

	//Lose oxygen every tick !
	void OnOxygenLoss ()
	{
		//Check for oxygen change
		if (!canPathfindAir ()) {
			Debug.Log ("Unit is gonna die from oxygen lost !");
			oxygen -= GameMasterScript.instance.oxygenLossPerTile;

		} 
	}

	#endregion

	#region Bomb

	/// <summary>
	/// Plants a bomb.
	/// </summary>
	public bool PlantBomb ()
	{
		if (currentBombsHeld > 0) {
			if (BB.BombCurrentTile ()) {
				currentBombsHeld--;
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Detonnates a bomb.
	/// </summary>
	public void DetonnateBomb ()
	{
		BB.DetonnateBomb ();
	}

	#endregion

	#region Resources

	/// <summary>
	/// Uses the resources according it the unit's alignment.
	/// The default value is one resource per usage.
	/// This method will return false if no resource is currently available
	/// </summary>
	/// <returns><c>true</c>, if resource was used, <c>false</c> otherwise.</returns>
	/// <param name="amount">Amount.</param>
	public bool UseResource (int amount = 1)
	{
		//If the player has resources, use them, and refresh the UI
		switch (alignment) {
		case PlayerData.TypeOfPlayer.human:
			if (GameMasterScript.instance.PLMB.humanPlayer.resources >= amount) {
				GameMasterScript.instance.PLMB.humanPlayer.resources -= amount;
				GameMasterScript.instance.UIMB.G_UpdateResourcesUI ();
				return true;
			}
			break;
		case PlayerData.TypeOfPlayer.enemy:
			if (GameMasterScript.instance.PLMB.enemyPlayer.resources >= amount) {
				GameMasterScript.instance.PLMB.enemyPlayer.resources -= amount;
				GameMasterScript.instance.UIMB.G_UpdateResourcesUI ();
				return true;
			}
			break;
		}

		return false;
	}

	#endregion

	/// <summary>
	/// Returns true if the unit is currently standing on its spawn tiles.
	/// </summary>
	/// <returns><c>true</c>, if on spawn tiles was ised, <c>false</c> otherwise.</returns>
	public bool isOnSpawnTiles (PlayerData.TypeOfPlayer playerType)
	{
		if (playerType == PlayerData.TypeOfPlayer.human) {
			for (int i = 0; i < GameMasterScript.instance.PLMB.humanPlayer.spawnTiles.Length; i++) {
				if (currentTile.ID == GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [i].ID)
					return true;
			}
		} else if (playerType == PlayerData.TypeOfPlayer.enemy) {
			for (int i = 0; i < GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles.Length; i++) {
				if (currentTile.ID == GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [i].ID)
					return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the current until is below the enemy HQ.
	/// This method is used to plant the endgame bomb.
	/// </summary>
	/// <returns><c>true</c>, if below enemy H was ised, <c>false</c> otherwise.</returns>
	public bool isBelowEnemyHQ ()
	{
		switch (alignment) {
		case PlayerData.TypeOfPlayer.human:
			if (currentTile.tilePos.x >= GameMasterScript.instance.gridWidth - GridManagerBehavior.STATIC_WIDTH)
				return true;
			break;
		case PlayerData.TypeOfPlayer.enemy:
			if (currentTile.tilePos.x <= GridManagerBehavior.STATIC_WIDTH - 1)
				return true;
			break;
		}

		return false;
	}

	public void OnStaminaRestored ()
	{
		stamina = unitData.stamina;
		isExhausted = false;
		currentBombsHeld = unitData.amountOfBombs;
	}

	public void OnDeselect ()
	{
		Debug.Log ("Unit Deselected: " + this.gameObject.name);

		//Hide the correct file if player)
		if (alignment == PlayerData.TypeOfPlayer.human) {
			GameMasterScript.instance.UIMB.G_DisplayCharacterFile (currentCharacterFile, false);
		}
	}

	public void StopEverythingies ()
	{
		//Stop all actions
		UMB.StopWalking ();
		DB.StopDigging ();
		if (CB.isFighting)
			CB.StopCombat ();

		UMB.StopAllCoroutines ();
		DB.StopAllCoroutines ();

	}
}
