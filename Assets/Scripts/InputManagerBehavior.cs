using UnityEngine;
using System.Collections;

/// <summary>
/// Input manager behavior.
/// Its job is to give order to other scrips according to mouse state.
/// </summary>
public class InputManagerBehavior : MonoBehaviour, IManager
{
	public enum InputState
	{
		disabled,
		idle,
		unitSelected,
		selectingDigPath
	}

	//References
	public SelectionManagerBehavior SMB;
	public LayerMask tileLayer;

	//Input state var
	InputState _currentState;

	public InputState currentState {
		get { return _currentState; } 
		set {
			//Stuff to do when leaving a state
			switch (_currentState) {
			case InputState.disabled:
				break;
			case InputState.idle:
				break;
			case InputState.unitSelected:
				break;
			case InputState.selectingDigPath:
				break;
			}

			_currentState = value;

			//Stuff to do when entering a state
			switch (_currentState) {
			case InputState.disabled:
				break;
			case InputState.idle:
				break;
			case InputState.unitSelected:
				break;
			case InputState.selectingDigPath:
				break;
			}
		}
	}

	//START METHOD
	public void OnGameStart ()
	{
	}

	void Update ()
	{
		//Call the righty method according to the current state
		switch (currentState) {
		case InputState.idle:
			MouseIdleState ();
			break;
		case InputState.unitSelected:
			MouseUnitSelectedState ();
			break;
		case InputState.selectingDigPath:
			break;
		}
	}

	#region Idle State

	/// <summary>
	/// This method will be run when the current state is IDLE.
	/// </summary>
	void MouseIdleState ()
	{
		//In Idle, left click will select a unit
		if (Input.GetMouseButtonDown (0)) {
			//Raycast, and if a tile is hit, look if a player is current stepping on it.

			GroundBehavior tileHitLC = null;

			if (RaycastOnGroundTile (out tileHitLC)) {

				if (tileHitLC.unitsOnTile.Count > 0) {
					//Add all the units to the current selection
					for (int i = 0; i < tileHitLC.unitsOnTile.Count; i++)
						SMB.SelectNewUnitsOnTile (tileHitLC.unitsOnTile [i]);

					//Switch to UNITSELECTED input state
					currentState = InputState.unitSelected;
				}
			}
		}

		//In Idle, right click doesn't do anythingies

		//In Idle, mouseScroll zooms out the map
	}

	#endregion

	#region UnitSelected State

	void MouseUnitSelectedState ()
	{
		//In UnitSelected, left click will deselect all selected units
		SMB.DeselectUnits ();

		//In UnitSelected, right click will either order a unit to move, either to start digging.

		//In UnitSelected, mouseRoll will either change the selected unit on a same time, either zooms out the map

	}

	#endregion

	#region handy methods

	bool RaycastOnGroundTile (out GroundBehavior tileHit)
	{
		Ray clickRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit stuffHit;

		if (Physics.Raycast (clickRay, out stuffHit, Mathf.Infinity, tileLayer)) {
			tileHit = stuffHit.transform.GetComponent<GroundBehavior> ();
			return true;
		}

		tileHit = null;
		return false;
	}

	#endregion
}
