using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Selection manager behavior.
/// This class is responsible for storing and giving order to selected tiles.
/// </summary>
public class SelectionManagerBehavior : MonoBehaviour, IManager
{
	/// <summary>
	/// Change order.
	/// Used in ChangeUnitOrder method only.
	/// </summary>
	public enum ChangeOrder
	{
		moveUp,
		moveDown
	}

	///All units stored will be stored in this list. Note that the [0] item is the currently selected unit.
	public List<ISelection> unitSelected;

	//ScrollTimer
	public float selectionScrollTimer;
	float selectionScrollDelay;


	//START METHOD
	public void OnGameStart ()
	{
		unitSelected = new List<ISelection> ();

		//Set variables
		selectionScrollDelay = 0.0f;
	}

	/// <summary>
	/// Selects the new units on tile and call its OnSelect method.
	/// </summary>
	/// <param name="newUnit">New unit.</param>
	public bool SelectNewUnitsOnTile (UnitBehavior newUnit)
	{
		//if the unit cannot be selected, deny the selection. Gotta respect privacy, you know.
		if (!newUnit.canBeSelected)
			return false;
		
		if (((UnitBehavior)newUnit).alignment == PlayerData.TypeOfPlayer.human) {
			unitSelected.Add ((ISelection)newUnit);
			newUnit.OnSelect ();

			//if it's the first unit, get the event registered
			if (unitSelected [0] == newUnit)
				ApplyFirstSelectedUnitChange (newUnit, true);

			//Change the order layer 
			for (int i = 0; i < unitSelected.Count; i++) {
				if (unitSelected [i] == newUnit)
					newUnit.unitSR.sortingOrder = 10 - i;
			}
			return true;
		}

		return false;
	}

	/// <summary>
	/// Deselects all selected units, and call the OnDeselect method.
	/// </summary>
	public void DeselectUnits ()
	{
		for (int i = 0; i < unitSelected.Count; i++) {
			//if it's the first unit, get the event deregistered
			if (i == 0)
				ApplyFirstSelectedUnitChange ((UnitBehavior)unitSelected [0], false);
			unitSelected [i].OnDeselect ();

			//Change the order layer 
			((UnitBehavior)unitSelected [i]).unitSR.sortingOrder = 0;
		}
		
		unitSelected.Clear ();
	}

	public void DeselectUnit (ISelection newUnit)
	{
		//if it's the first unit, get the event deregistered
		if (unitSelected [0] == newUnit)
			ApplyFirstSelectedUnitChange ((UnitBehavior)newUnit, false);

		//Change the order layer 
		((UnitBehavior)newUnit).unitSR.sortingOrder = 0;

		unitSelected.Remove (newUnit);
		newUnit.OnDeselect ();

	}

	void Update ()
	{
		if (selectionScrollDelay >= 0.0f)
			selectionScrollDelay -= Time.deltaTime;
	}



	/// <summary>
	/// Sorts the list according to the chosen type of change
	/// </summary>
	/// <param name="typeOfChange">Type of change.</param>
	public void ChangeUnitOrder (ChangeOrder typeOfChange)
	{
		//If the player already scrolled, wait for the timer to end, and return. If not, set timer and scroll
		if (selectionScrollDelay >= 0.01f)
			return;
		else
			selectionScrollDelay = selectionScrollTimer;

		if (unitSelected.Count > 0) {
			if (typeOfChange == ChangeOrder.moveDown) {
				ISelection tmp = unitSelected [0];
				int tmpSortingIndex = ((UnitBehavior)tmp).unitSR.sortingOrder;
				unitSelected.RemoveAt (0);
				unitSelected.Add (tmp);
				((UnitBehavior)tmp).unitSR.sortingOrder = ((UnitBehavior)unitSelected [0]).unitSR.sortingOrder;
				((UnitBehavior)unitSelected [0]).unitSR.sortingOrder = tmpSortingIndex;


			} else if (typeOfChange == ChangeOrder.moveUp) {
				ISelection tmp = unitSelected [unitSelected.Count - 1];
				int tmpSortingIndex = ((UnitBehavior)tmp).unitSR.sortingOrder;
				unitSelected.RemoveAt (unitSelected.Count - 1);
				unitSelected.Insert (0, tmp);
				((UnitBehavior)tmp).unitSR.sortingOrder = ((UnitBehavior)unitSelected [unitSelected.Count - 1]).unitSR.sortingOrder;
				((UnitBehavior)unitSelected [unitSelected.Count - 1]).unitSR.sortingOrder = tmpSortingIndex;
			}

			Debug.Log ("Unit order changed! Now selecting " + unitSelected [0]);

		}
	}

	/// <summary>
	/// Method that needs to be called whenever a unitSelected[0] is added or removed.
	/// If added, onEnter must be set to true
	/// </summary>
	/// <param name="onEnter">If set to <c>true</c> on enter.</param>
	void ApplyFirstSelectedUnitChange (UnitBehavior newUnit, bool onEnter)
	{
		if (onEnter)
			newUnit.OnTileEnterEvent += OnUnitEntersTile;
		else
			newUnit.OnTileEnterEvent -= OnUnitEntersTile;
	}

	/// <summary>
	/// Whenever the selected Unit enters a new tile, add all friendly units to the selection
	/// </summary>
	void OnUnitEntersTile ()
	{
		//If there's only one unit on the tile, dont' bother, it's unitSelected[0]
		if (unitSelected [0].currentTile.unitsOnTile.Count > 1 || unitSelected.Count > 1) {
			if (unitSelected.Count > 1) {
				for (int i = 1; i < unitSelected [0].currentTile.unitsOnTile.Count; i++) {
					//Look at units to remove
					if (!unitSelected [0].currentTile.unitsOnTile.Contains ((UnitBehavior)unitSelected [i])) {
						DeselectUnit (unitSelected [i]);
					}
				}
			}
			//Add new units on tile
			for (int i = 0; i < unitSelected [0].currentTile.unitsOnTile.Count; i++) {
				if (!unitSelected.Contains ((UnitBehavior)unitSelected [0].currentTile.unitsOnTile [i]))
					SelectNewUnitsOnTile (unitSelected [0].currentTile.unitsOnTile [i]);
			}
		}
	}
}
