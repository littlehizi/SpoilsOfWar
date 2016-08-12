using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Runtime.InteropServices;

[RequireComponent (typeof(UnitBehavior))]
public class AIBrainBehavior : MonoBehaviour
{
	#region FSM

	public enum AIState
	{
		idle,
		walking,
		digging,
		fighting,
		goingHome,
		bombing
	}

	private AIState _currentState;

	public AIState currentState {
		get{ return _currentState; }
		set { 
			switch (_currentState) {
			case AIState.idle:
				break;
			case AIState.walking:
				currentUnit.StopWalking ();
				break;
			case AIState.digging:
				//Stop digging
				currentUnit.DB.StopDigging ();
				break;
			case AIState.fighting:
				break;
			case AIState.goingHome:
				break;
			case AIState.bombing:
				break;
			}

			_currentState = value;

			switch (_currentState) {
			case AIState.idle:
				break;
			case AIState.walking:
				break;
			case AIState.digging:
				break;
			case AIState.fighting:
				break;
			case AIState.goingHome:
				GoHome ();
				break;
			case AIState.bombing:
				break;
			}
		}
	}

	#endregion

	//References
	UnitBehavior currentUnit;
	UnitData.UnitType currentType;
	GroundBehavior[] pathChosen;
	public float inactivityDelay = 5;
	float inactivityTimer;

	//Internal variables
	public int pointsToPick = 5;
	public static List<GroundBehavior> dugTiles;

	//Digger specific
	public static GroundBehavior selectedBombsite;
	public int preferedDiggerDepth = 4;
	public bool isWaitingForResources;
	GroundBehavior[] currentPath;

	//listener specific
	public int listenerID;

	//Fighter specific
	private GroundBehavior _currentTileTarget;

	public GroundBehavior currentTileTarget {
		get{ return _currentTileTarget; }
		set {
			//Set the value and start walking to it.
			GroundBehavior previousTarget = _currentTileTarget;

			_currentTileTarget = value;

			if (_currentTileTarget != null && (previousTarget == null || _currentTileTarget.ID != previousTarget.ID))
				//currentUnit.WalkToTile (_currentTileTarget);
				currentUnit.StartDiggingProcess (DigFromHereUntilTile (currentTileTarget));
			
		}
	}

	public void SetupAI ()
	{
		//return;

		//Initalize
		currentUnit = this.GetComponent<UnitBehavior> ();
		AIBrainBehavior.dugTiles = new List<GroundBehavior> ();

		//init
		currentType = currentUnit.unitData.typeOfUnit;

		//Pick a random bombsite on the uper half of the depth, under the player camp.
		do {
			AIBrainBehavior.selectedBombsite = GameMasterScript.instance.GMB.currentGrid.tiles [Random.Range (0, GridManagerBehavior.STATIC_WIDTH - 1), Random.Range (GridManagerBehavior.STATIC_HEIGHT + 3, GameMasterScript.instance.gridHeight / 2)];
		} while(AIBrainBehavior.selectedBombsite == null || AIBrainBehavior.selectedBombsite.isAnObstacle);

		//Get base action / goal
		switch (currentType) {
		case UnitData.UnitType.Digger:
			//Diggers find a path towards the common bombsite.

			//Pick 5 points on the path to bomb in a snake-like path
			//Pick start tile, down from its current tile
			GroundBehavior firstTile = currentUnit.currentTile;
			do {
				Vector2 tilePosBelow = firstTile.tilePos;
				tilePosBelow.y += 1;
				firstTile = GameMasterScript.instance.GMB.TileposToTile (tilePosBelow);
			} while(firstTile == null || firstTile.isDug);

			DigSelectionManagerBehavior.GetGroundTilesToDig (firstTile);

			bool isVertical = true;

			Vector2 pre_chosenTile = Vector2.zero;
			Vector2 chosenTile = currentUnit.currentTile.tilePos;

			for (int i = 0; i < pointsToPick; i++) {

				do {
					//Backup
					if (!GameMasterScript.instance.GMB.TileposToTile (chosenTile).isAnObstacle)
						pre_chosenTile = chosenTile;
					//else
					//	pointsToPick++;

					if (isVertical)
						chosenTile = new Vector2 (pre_chosenTile.x, 
							Mathf.Clamp (selectedBombsite.tilePos.y - Random.Range (-GameMasterScript.instance.gridHeight / preferedDiggerDepth, GameMasterScript.instance.gridHeight / 10), GridManagerBehavior.STATIC_HEIGHT + 2, GameMasterScript.instance.gridHeight - 1));
					else
						chosenTile = new Vector2 (Mathf.Clamp (Mathf.Abs (selectedBombsite.tilePos.x - currentUnit.currentTile.tilePos.x) / (i) + Random.Range (-2, 2), 0, GameMasterScript.instance.gridWidth - 2), 
							pre_chosenTile.y);
					
					isVertical = !isVertical;
				} while(GameMasterScript.instance.GMB.TileposToTile (chosenTile).isAnObstacle);
					
				DigSelectionManagerBehavior.GetGroundTilesToDig (GameMasterScript.instance.GMB.TileposToTile (chosenTile), true);
			}

			DigSelectionManagerBehavior.GetGroundTilesToDig (selectedBombsite, true);

			pathChosen = DigSelectionManagerBehavior.OutputTilesToDig (currentUnit.currentTile);

			currentPath = pathChosen;
			//DIG !
			currentUnit.StartDiggingProcess (pathChosen);

			//Subscribe to the inactivity timer
			inactivityTimer = inactivityDelay;
			currentUnit.OnTileEnterEvent += CheckForInactivity;

			//Subscribe to bombing event
			currentUnit.OnTileEnterEvent += CheckIfOnBombsite;

			//Subscribe to add new dug tile event
			currentUnit.OnTileEnterEvent += AddNewDugTile;

			break;
		case UnitData.UnitType.Fighter:
			//Fighters do nothingies during the first seconds. They wait for the diggers to dig a bit until they become inactive.
			currentTileTarget = currentUnit.currentTile;

			//Once inactive, they'll start moving
			inactivityTimer = inactivityDelay;
			currentUnit.OnTileEnterEvent += CheckForInactivity;

			//Check for closeby units
			currentUnit.OnTileEnterEvent += LookForUnitsInSight;
			break;
		case UnitData.UnitType.Listener:
			//Get which listener is the current one.
			List<UnitBehavior> storedUnits = GameMasterScript.instance.PLMB.enemyPlayer.storedUnits;
			int otherListeners = 0;
			for (int i = 0; i < storedUnits.Count; i++) {
				if (storedUnits [i].unitData.typeOfUnit == UnitData.UnitType.Listener)
					otherListeners++;
				
				if (storedUnits [i] == currentUnit) {
					listenerID = otherListeners;
					break;
				}
			}

			GroundBehavior targetTile = null;
			//Position the listeners according to their ID.
			switch (listenerID) {
			case 0:
				//Get a point near home, slightly below.
				targetTile = GameMasterScript.instance.GMB.TileposToTile (new Vector2 (GameMasterScript.instance.gridWidth - GridManagerBehavior.STATIC_WIDTH - 3, GridManagerBehavior.STATIC_HEIGHT + 2));
				break;
			case 1:
				//Get a point towards the middle
				targetTile = GameMasterScript.instance.GMB.TileposToTile (new Vector2 (GameMasterScript.instance.gridWidth / 3 * 2, GridManagerBehavior.STATIC_HEIGHT + 2));
				break;
			case 2:
				//Get a point below but close to home
				targetTile = GameMasterScript.instance.GMB.TileposToTile (new Vector2 (GameMasterScript.instance.gridWidth - GridManagerBehavior.STATIC_WIDTH - 3, GridManagerBehavior.STATIC_HEIGHT + GameMasterScript.instance.gridHeight / 2));
				break;
			case 3:
				//I don't know... Why would you wanna have more than 3 listeners..? that's kinda retarded
				break;
			case 4:
				break;
			case 5:
				break;
			}


			firstTile = currentUnit.currentTile;

			do {
				Vector2 tilePosBelow = firstTile.tilePos;
				tilePosBelow.y += 1;
				firstTile = GameMasterScript.instance.GMB.TileposToTile (tilePosBelow);
			} while(firstTile == null || firstTile.isDug);


			Debug.Log (firstTile + " " + targetTile);

			currentUnit.StartDiggingProcess (PathfindingManagerBehavior.FindWithAStarNoObstacles (firstTile, targetTile));


			//Check for closeby units
			currentUnit.OnTileEnterEvent += LookForUnitsInSight;

			break;
		}
	}

	void Update ()
	{
		switch (currentType) {
		case UnitData.UnitType.Digger:
			//Update inactivity
			if (inactivityTimer < 0) {
				//UNIT HAS BEEN INACTIVE !Restart digging from scratch
				currentPath = pathChosen;
				currentUnit.StartDiggingProcess (currentPath);
				inactivityTimer = inactivityDelay;
				//Debug.Log ("Inactive unit spotted !");

			} else if (inactivityTimer > 0 && !isWaitingForResources) {
				inactivityTimer -= Time.deltaTime;
			}

			//if the enemy doesn't have enough stamina, have it go home
			if (currentUnit.stamina < 10 && currentState != AIState.goingHome) {
				//Check if unit is close from bombsite. If so, unit will try to make a run for it
				if (Vector3.Distance (currentUnit.currentTile.trueTilePos, selectedBombsite.trueTilePos) > 5) {
					currentState = AIState.goingHome;
					return;
				}
			} else if (currentState == AIState.goingHome)
				return;

			//If there's not enough resources, wait, don't dig.
			if (GameMasterScript.instance.PLMB.enemyPlayer.resources < 10 && !isWaitingForResources) {
				isWaitingForResources = true;
				currentUnit.DB.StopDigging ();
			} else if (GameMasterScript.instance.PLMB.enemyPlayer.resources > 20 && isWaitingForResources) {
				//if there's enough resources, restart digging from where it left off!
				currentPath = UpdateDiggingPath ();
				currentUnit.StartDiggingProcess (currentPath);
				isWaitingForResources = false;
			}

			break;
		case UnitData.UnitType.Fighter:
			//Check for flag change
			if (currentUnit.CB.isFighting)
				currentState = AIState.fighting;
			else if (currentState == AIState.fighting && !currentUnit.CB.isFighting)
				currentState = AIState.idle;

			//If the fighter is inactive, pick a new tile to dig. If the unit is fighting, wait for it to be done at least..
			if (inactivityTimer < 0 && currentState != AIState.fighting) {
				//Debug.Log ("Inactive unit spotted !");
				if (currentTileTarget.ID == currentUnit.currentTile.ID) {
					if (AIBrainBehavior.dugTiles.Count == 0)
						return;
					currentTileTarget = SetFighterPath ();
					//currentUnit.StartDiggingProcess (PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, GameMasterScript.instance.GMB.TileposToTile (new Vector2 (currentUnit.currentTile.tilePos.x, currentUnit.currentTile.tilePos.y + 1)), currentTileTarget, true));
					//Reset timer
				} 
				inactivityTimer = inactivityDelay;
			} else if (inactivityTimer < 0 && currentState == AIState.fighting) {
				//Check for units on surrounding tiles
				GroundBehavior[] neighbors = PathfindingManagerBehavior.GetTileNeighbor (currentUnit.currentTile).ToArray ();

				bool check = false;
				for (int i = 0; i < neighbors.Length; i++) {
					if (neighbors [i].unitsOnTile.Count > 0 && neighbors [i].unitsOnTile [0].alignment == PlayerData.TypeOfPlayer.human)
						check = true;
				}

				if (!check)
					currentState = AIState.idle;
				
			} else if (inactivityTimer > 0) {
				inactivityTimer -= Time.deltaTime;
			}

			//IF the unit doesn't have enough stamina and is not fighting, have it go home
			if (currentUnit.stamina < 10 && currentState != AIState.fighting)
				currentState = AIState.goingHome;
			break;
		case UnitData.UnitType.Listener:
			break;
		}
	}

	#region Location Utilities

	void GoHome ()
	{
		//Debug.Log ("AI going home~");
		//Dig the way home
		GroundBehavior[] pathToHome = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, currentUnit.currentTile, GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [Random.Range (0, 6)]);
		//currentUnit.WalkToTile (GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [Random.Range (0, 6)]);
		currentUnit.StartDiggingProcess (pathToHome);

		currentUnit.OnTileEnterEvent += WaitToBeHome;
	}

	/// <summary>
	/// Method called by the OnTile event, and is responsible for checking if the AI is on a spawn tile.
	/// This method gets subscribed only when the GoHome method is called.
	/// </summary>
	void WaitToBeHome ()
	{
		//Debug
		//currentUnit.DB.StopAllCoroutines ();

		if (currentUnit.isOnSpawnTiles (PlayerData.TypeOfPlayer.enemy)) {
			//if the player is at home, unsubscribe and have it and proceed with its activity
			currentUnit.OnTileEnterEvent -= WaitToBeHome;

			switch (currentType) {
			case UnitData.UnitType.Digger:
				//If digger, RESUME DIGGING !
				currentPath = UpdateDiggingPath ();
				currentUnit.StartDiggingProcess (currentPath);
				break;
			case UnitData.UnitType.Fighter:
				//Once home, just pick a normal path
				currentTileTarget = SetFighterPath ();
				//currentUnit.StartDiggingProcess (DigFromHereUntilTile(currentTileTarget));
				break;
			case UnitData.UnitType.Listener:
				break;
			}

		}
	}

	void CheckForInactivity ()
	{
		//Reset timer !
		inactivityTimer = inactivityDelay;
	}

	#endregion

	#region Path Utilities

	/// <summary>
	/// Returns a new digging path!
	/// </summary>
	/// <returns>The digging path.</returns>
	GroundBehavior[] UpdateDiggingPath ()
	{
		int currentIndex = 0;

		for (int i = 0; i < pathChosen.Length; i++) {
			if (pathChosen [i].ID == currentUnit.currentTile.ID) {
				currentIndex = i;
				break;
			}
		}

		List<GroundBehavior> outputList = new List<GroundBehavior> ();

		for (int i = 0; i < pathChosen.Length; i++)
			outputList.Add (pathChosen [i]);

		for (int i = 0; i < currentIndex; i++)
			outputList.RemoveAt (0);

		GroundBehavior[] output = outputList.ToArray ();

		Debug.Log (output [output.Length - 1]);
		return output;
	}

	void CheckIfOnBombsite ()
	{
		//Check if enemy is on bomb site. If So, plant all the bombs and suicide-detonnate
		if ((currentUnit.currentTile.ID == selectedBombsite.ID) || currentUnit.isBelowEnemyHQ ()) {
			Debug.Log ("GAME'S ABOUT TO EEEEND");
			//Set the state
			currentState = AIState.bombing;

			//Plant one
			currentUnit.PlantBomb ();
			//Now plant as many as necessary
			VictoryManagerBehavior.BombSite currentBombSite = null;

			for (int i = 0; i < GameMasterScript.instance.VMB.bombSites.Count; i++) {
				if (GameMasterScript.instance.VMB.bombSites [i].owners.Contains (currentUnit))
					currentBombSite = GameMasterScript.instance.VMB.bombSites [i];
			}

			if (currentBombSite == null)
				Debug.LogError ("SHEET GONE WRONG !");

			for (int i = 0; i < currentBombSite.totalAmountOfBombsNeeded; i++) {
				if (!currentUnit.PlantBomb ()) {
					currentState = AIState.goingHome;
					Debug.Log ("Not enough bombs ! Abort !");
					return;
				}

				//if you already can detonnate, do it !
				if (currentUnit.BB.canDetonnateABombsite) {
					//Add a last bomb, 'cause it's slightly retarded
					if (!currentUnit.PlantBomb ())
						return;
					//Kaboom
					currentUnit.DB.StopAllCoroutines ();
					currentUnit.DetonnateBomb ();
					Debug.LogWarning ("GGWP");
					return;
				}
			}

			//TEchnically, the game is won if it gets to this point
			Debug.LogWarning ("No GGWP");
		}
	}

	GroundBehavior SetFighterPath ()
	{
		if (AIBrainBehavior.dugTiles.Count > 0)
			return AIBrainBehavior.dugTiles [Random.Range (0, AIBrainBehavior.dugTiles.Count)];
		else
			return currentUnit.currentTile;
	}

	/// <summary>
	/// Adds the current tile to the path of dug tiles
	/// This gets called everytime a digger gets on a new tile
	/// </summary>
	void AddNewDugTile ()
	{
		if (AIBrainBehavior.dugTiles.Count == 0 || !AIBrainBehavior.dugTiles.Contains (currentUnit.currentTile))
			AIBrainBehavior.dugTiles.Add (currentUnit.currentTile);
	}

	GroundBehavior[] DigFromHereUntilTile (GroundBehavior tile)
	{
		GroundBehavior[] tmp = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, currentUnit.currentTile, tile);

		if (tmp == null || tmp.Length == 1) {
			return tmp;
		}
		GroundBehavior[] output = new GroundBehavior[tmp.Length - 1];

		for (int i = 1; i < tmp.Length; i++)
			output [i - 1] = tmp [i];

		return output;
	}

	#endregion

	#region Combat Utilities

	void LookForUnitsInSight ()
	{
		RaycastHit2D[] tileTargets = Physics2D.CircleCastAll (currentUnit.currentTile.trueTilePos, (float)currentUnit.unitData.vision, Vector2.zero);

		List<GroundBehavior> targetedUnits = new List<GroundBehavior> ();

		//Get all the tiles
		for (int i = 0; i < tileTargets.Length; i++) {
			GroundBehavior tmpTile = GameMasterScript.instance.GMB.TileposToTile (tileTargets [i].transform.position);

			if (tmpTile.unitsOnTile.Count > 0) {
				if (tmpTile.unitsOnTile [0].alignment == PlayerData.TypeOfPlayer.human) {
					//If the units on that tile are enemies, then you've got a new target !
					targetedUnits.Add (tmpTile);
				}
			}
		}

		switch (currentType) {
		case UnitData.UnitType.Fighter:
			//Pick a random target
			if (targetedUnits.Count > 0)
				currentTileTarget = targetedUnits [Random.Range (0, targetedUnits.Count)];
			break;
		case UnitData.UnitType.Listener:
			//Find a fighter, and tell him to handle it if the enemy is far. If the enemy is close, self-detonnate self.
			for (int i = 0; i < targetedUnits.Count; i++) {
				if (Vector2.Distance (currentUnit.currentTile.tilePos, targetedUnits [i].tilePos) < 1.4f) {
					//Self detonnate !
					if (currentUnit.PlantBomb ()) {
						currentUnit.DetonnateBomb ();
						return;
					}
				}
			}

			List<UnitBehavior> fighterList = new List<UnitBehavior> ();

			for (int i = 0; i < GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Count; i++) {
				if (GameMasterScript.instance.PLMB.enemyPlayer.storedUnits [i].unitData.typeOfUnit == UnitData.UnitType.Fighter)
					fighterList.Add (GameMasterScript.instance.PLMB.enemyPlayer.storedUnits [i]);
			}

			if (fighterList.Count > 0 && targetedUnits.Count > 0) {
				UnitBehavior chosenFighter = fighterList [Random.Range (0, fighterList.Count)];

				chosenFighter.GetComponent<AIBrainBehavior> ().currentTileTarget = targetedUnits [Random.Range (0, targetedUnits.Count)];
			}
			break;
		}

	}

	#endregion

	#region Death

	public void AIDeathEnter ()
	{
		//Desubscribe to stuff
		currentUnit.OnTileEnterEvent -= CheckForInactivity;
		currentUnit.OnTileEnterEvent -= CheckIfOnBombsite;
		currentUnit.OnTileEnterEvent -= AddNewDugTile;
		currentUnit.OnTileEnterEvent -= LookForUnitsInSight;

		//Summon a new unit of same type !
		if (!((State_Game)GameMasterScript.currentState).isGameOver)
			GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (currentType, GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [Random.Range (0, 6)], PlayerData.TypeOfPlayer.enemy));
	}

	//	void OnDestroy ()
	//	{
	//		//Desubscribe to stuff
	//		currentUnit.OnTileEnterEvent -= CheckForInactivity;
	//		currentUnit.OnTileEnterEvent -= CheckIfOnBombsite;
	//		currentUnit.OnTileEnterEvent -= AddNewDugTile;
	//	}

	#endregion

}
