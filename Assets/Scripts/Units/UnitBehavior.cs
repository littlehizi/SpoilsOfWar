using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBehavior : MonoBehaviour, ISelection, IVision
{
	//References
	UnitMovementBehavior UMB;
	DigBehavior DB;
	CombatBehavior CB;
	BombBehavior BB;

	[HideInInspector]public SpriteRenderer unitSR;

	//Internal vars
	GroundBehavior _currentTile;

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
				OnStaminaRestored ();
			}

			//Check for oxygen change
			if (stamina < unitData.stamina && !canPathfindAir ()) {
				Debug.Log ("wee?");
				oxygen -= GameMasterScript.instance.oxygenLossPerTile;
			}
		}
	}

	//Stats
	public UnitData unitData;
	public int currentBombsHeld;
	public bool canBeSelected;
	public bool isExhausted;
	public const float exhaustionMalus = 2.5f;

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
		health = unitData.health;
		stamina = unitData.stamina;

		//Set current tile
		currentTile = newTile;

		//Victory
		OnTileEnterEvent += GameMasterScript.instance.VMB.VictoryCheck;

		//Selection
		canBeSelected = true;
		isExhausted = false;
	}

	public void OnSelect ()
	{
		Debug.Log ("Unit Selected: " + this.gameObject.name);
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
		if (tilesToDig == null || tilesToDig.Length < 1)
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
		CB.StartCombat (enemyTile, false);
	}

	public void OnDeathEnter ()
	{
		//Stop all actions
		UMB.StopWalking ();
		DB.StopDigging ();
		if (CB.isFighting)
			CB.StopCombat ();

		//Deselect unit (in case it was)
		if (GameMasterScript.instance.SMB.unitSelected.Contains (this))
			GameMasterScript.instance.SMB.DeselectUnit (this);

		//VISION
		DeleteIVisionEntry ();

		//Victory
		OnTileEnterEvent -= GameMasterScript.instance.VMB.VictoryCheck;

		//Remove itself from the player unitstorage
		switch (alignment) {
		case PlayerData.TypeOfPlayer.human:
			GameMasterScript.instance.PLMB.humanPlayer.storedUnits.Remove (this);
			break;
		case PlayerData.TypeOfPlayer.enemy:
			GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Remove (this);
			break;
		}

		//Destroy unit
		Destroy (this.gameObject);
	}

	#endregion

	#region oxygen

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

	#endregion

	#region bomb

	/// <summary>
	/// Plants a bomb.
	/// </summary>
	public bool PlantBomb ()
	{
		if (currentBombsHeld > 0) {
			if (BB.BombCurrentTile ()) {
				currentBombsHeld--;
				Debug.Log ("a bomb has been planted");
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

	/// <summary>
	/// Returns true if the unit is currently standing on its spawn tiles.
	/// </summary>
	/// <returns><c>true</c>, if on spawn tiles was ised, <c>false</c> otherwise.</returns>
	bool isOnSpawnTiles (PlayerData.TypeOfPlayer playerType)
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

	public void OnStaminaRestored ()
	{
		stamina = unitData.stamina;
		isExhausted = false;
		currentBombsHeld = unitData.amountOfBombs;
	}

	public void OnDeselect ()
	{
		Debug.Log ("Unit Deselected: " + this.gameObject.name);
	}
}
