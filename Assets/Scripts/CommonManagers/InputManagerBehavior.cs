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
	}

	//References
	public SelectionManagerBehavior SMB;
	public LayerMask tileLayer;

	//Input state var
	InputState _currentState;

	//Internal Vars
	GroundBehavior digSelectionTmp;


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
			}

			_currentState = value;

			Debug.Log ("New Input State: " + _currentState.ToString ());

			//Stuff to do when entering a state
			switch (_currentState) {
			case InputState.disabled:
				break;
			case InputState.idle:
				break;
			case InputState.unitSelected:
				break;
			}
		}
	}

	//START METHOD
	public void OnGameStart ()
	{
		digSelectionTmp = null;
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

					if (tileHitLC.unitsOnTile.Count > 0) {
						bool alignmentCheck = (tileHitLC.unitsOnTile [0].alignment == PlayerData.TypeOfPlayer.human);

						//If units are cooperating with humans, select them, if not, cancel the selection
						if (alignmentCheck) {
							for (int i = 0; i < tileHitLC.unitsOnTile.Count; i++) {
								if (tileHitLC.unitsOnTile [i].canBeSelected)
									SMB.SelectNewUnitsOnTile (tileHitLC.unitsOnTile [i]);
							}
						} else
							return;
					}
				
					//Switch to UNITSELECTED input state
					currentState = InputState.unitSelected;
                    
				}
			}
		}

		//In Idle, right click doesn't do anythingies

		//In Idle, mouseScroll zooms out the map
		if (!Mathf.Approximately (Input.GetAxis ("Mouse ScrollWheel"), 0.0f))
			Camera.main.GetComponent<WorldCamera> ().GetDesiredZoom ();
	}

	#endregion

	#region UnitSelected State


	void MouseUnitSelectedState ()
	{
		//Check if the selection manager has no unit selected, just in case
		if (SMB.unitSelected.Count == 0) {
			currentState = InputState.idle;
			return;
		}


		//In UnitSelected, left click will deselect all selected units
		if (Input.GetMouseButtonDown (0)) {
			SMB.DeselectUnits ();
			//Also, empty the selection of tiles
			DigSelectionManagerBehavior.ResetDigPath ();

			//If left clicking on a tile with other friendly units, stay in this state
			GroundBehavior tileHitLC = null;

			if (RaycastOnGroundTile (out tileHitLC)) {
				if (tileHitLC.unitsOnTile.Count > 0) {
					bool alignmentCheck = (tileHitLC.unitsOnTile [0].alignment == PlayerData.TypeOfPlayer.human);

					if (alignmentCheck) {
						for (int i = 0; i < tileHitLC.unitsOnTile.Count; i++)
							SMB.SelectNewUnitsOnTile (tileHitLC.unitsOnTile [i]);

						return;
					}
				}
			}

			//Go backies to IDLE mouse state
			currentState = InputState.idle;
		}

		//In UnitSelected, right click will either order a unit to move, either to start digging.
		if (Input.GetMouseButtonDown (1)) {
			GroundBehavior tileHitRC = null;
			if (RaycastOnGroundTile (out tileHitRC)) {				
				//If the tile is dug, then move. if not, then start the digging logic
				if (tileHitRC.isDug) {
					((UnitBehavior)SMB.unitSelected [0]).WalkToTile (tileHitRC);
				} else {
					//If the same tile is clicked twice, then the selecting is over. if not, the player can extend the path.
					if (digSelectionTmp == null || tileHitRC.ID != digSelectionTmp.ID) {
						//If it's the first time clickin on a tile or it's not a double tap, continue gathering tiles
						digSelectionTmp = tileHitRC;

						//Highlight the tile (TEMPORARILY)
						tileHitRC.tileSR.color = Color.yellow;

						//The method will handle the storing and creating path
						DigSelectionManagerBehavior.GetGroundTilesToDig (tileHitRC);
					} else if (digSelectionTmp != null && tileHitRC.ID == digSelectionTmp.ID) {
						//If double tap, give the order to the currently character to dig (or move to closest tile and dig)
						((UnitBehavior)SMB.unitSelected [0]).StartDiggingProcess (DigSelectionManagerBehavior.OutputTilesToDig (SMB.unitSelected [0].currentTile));

						//Reset flag
						digSelectionTmp = null;
					} 
				}
			}
		}

		//In UnitSelected, mouseRoll will either change the selected unit on a same time, either zooms out the map
		if (!Mathf.Approximately (Input.GetAxis ("Mouse ScrollWheel"), 0.0f)) {
				
			if (Input.GetAxis ("Mouse ScrollWheel") > 0.05f)
				SMB.ChangeUnitOrder (SelectionManagerBehavior.ChangeOrder.moveDown);
			else if (Input.GetAxis ("Mouse ScrollWheel") < -0.05f)
				SMB.ChangeUnitOrder (SelectionManagerBehavior.ChangeOrder.moveUp);
		}
	}

	#endregion

	#region handy methods

	bool RaycastOnGroundTile (out GroundBehavior tileHit)
	{
		Ray clickRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit2D stuffHit = Physics2D.Raycast (clickRay.origin, clickRay.direction, Mathf.Infinity, tileLayer);

		if (stuffHit) {
			tileHit = stuffHit.transform.GetComponent<GroundBehavior> ();
			return true;
		}

		tileHit = null;
		return false;
	}

	#endregion
}
