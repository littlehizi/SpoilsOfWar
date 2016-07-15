using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBehavior : MonoBehaviour, ISelection
{
	//References
	UnitMovementBehavior UMB;
	DigBehavior DB;

	//Internal vars
	GroundBehavior _currentTile;

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
		}
	}

	//Stats
	public UnitData unitData;
	public int health;
	public int stamina;
    public PlayerData.TypeOfPlayer alignment;

	public void SetupUnit (GroundBehavior newTile)
	{
		//Get references
		UMB = this.GetComponent<UnitMovementBehavior> ();
		DB = this.GetComponent<DigBehavior> ();

		//Set current tile
		currentTile = newTile;

		//Set temporary data
		health = unitData.health;
		stamina = unitData.stamina;
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

	public void OnDeselect ()
	{
		Debug.Log ("Unit Deselected: " + this.gameObject.name);
	}
}
