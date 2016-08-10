using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
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
	public static GroundBehavior selectedBombsite;
	GroundBehavior[] pathChosen;

	//Internal variables
	public int stubbornness;
	public int pointsToPick = 5;
	public int preferedDiggerDepth = 4;
	public bool isWaitingForResources;
	GroundBehavior[] currentPath;
	public float inactivityDelay = 5;
	float inactivityTimer;


	public void SetupAI ()
	{
		//return;

		//Initalize
		currentUnit = this.GetComponent<UnitBehavior> ();

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

			break;
		case UnitData.UnitType.Fighter:
			break;
		case UnitData.UnitType.Listener:
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
				Debug.Log ("Inactive unit spotted !");

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
			break;
		case UnitData.UnitType.Listener:
			break;
		}
	}

	#region Location Utilities

	void GoHome ()
	{
		Debug.Log ("AI going home~");
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

	#endregion

	#region Death

	public void AIDeathEnter ()
	{
		//Desubscribe to stuff
		currentUnit.OnTileEnterEvent -= CheckForInactivity;
		currentUnit.OnTileEnterEvent -= CheckIfOnBombsite;

		//Summon a new unit of same type !
		GameMasterScript.instance.PLMB.enemyPlayer.storedUnits.Add (UnitSpawnerManagerBehavior.SpawnUnit (currentType, GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [Random.Range (0, 6)], PlayerData.TypeOfPlayer.enemy));
	}

	//	void OnDestroy ()
	//	{
	//		//Desubscribe to stuff
	//		currentUnit.OnTileEnterEvent -= CheckForInactivity;
	//		currentUnit.OnTileEnterEvent -= CheckIfOnBombsite;
	//	}

	#endregion

	//	void OnDrawGizmos ()
	//	{
	//		if (pathChosen == null)
	//			return;
	//
	//		Gizmos.color = Color.cyan;
	//		for (int i = 0; i < pathChosen.Length; i++)
	//			Gizmos.DrawWireCube (pathChosen [i].trueTilePos, Vector3.one);
	//
	//		Gizmos.color = Color.magenta;
	//
	//		Gizmos.DrawWireCube (selectedBombsite.trueTilePos, Vector3.one);
	//
	//		Gizmos.DrawWireCube (pathChosen [0].trueTilePos, Vector3.one);
	//	}
}
