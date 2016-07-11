using UnityEngine;
using System.Collections;

public class UnitBehavior : MonoBehaviour, ISelection
{
	//References
	UnitMovementBehavior UMB;

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

	public void SetupUnit (GroundBehavior newTile)
	{
		//Get references
		UMB = this.GetComponent<UnitMovementBehavior> ();

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

	public void WalkToTile (GroundBehavior destination)
	{
		currentState = UnitState.walking;

		UMB.MoveUnitToTarget (destination);
	}

	public void StopWalking ()
	{
		UMB.StopWalking ();
	}

	public void ReachedDestination ()
	{
		currentState = UnitState.idle;
	}

	#endregion


	public void OnDeselect ()
	{
		Debug.Log ("Unit Deselected: " + this.gameObject.name);
	}
}
