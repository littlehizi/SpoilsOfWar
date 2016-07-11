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


	//START METHOD
	public void OnGameStart ()
	{
		unitSelected = new List<ISelection> ();
	}

	/// <summary>
	/// Selects the new units on tile and call its OnSelect method.
	/// </summary>
	/// <param name="newUnit">New unit.</param>
	public void SelectNewUnitsOnTile (ISelection newUnit)
	{
		unitSelected.Add (newUnit);
		newUnit.OnSelect ();
	}

	/// <summary>
	/// Selects the new units on tile and call its OnSelect method.
	/// </summary>
	/// <param name="newUnit">New unit.</param>
	public void SelectNewUnitsOnTile (UnitBehavior newUnit)
	{
		unitSelected.Add ((ISelection)newUnit);
		newUnit.OnSelect ();
	}

	/// <summary>
	/// Deselects all selected units, and call the OnDeselect method.
	/// </summary>
	public void DeselectUnits ()
	{
		for (int i = 0; i < unitSelected.Count; i++)
			unitSelected [i].OnDeselect ();
		
		unitSelected.Clear ();
	}

	/// <summary>
	/// Sorts the list according to the chosen type of change
	/// </summary>
	/// <param name="typeOfChange">Type of change.</param>
	public void ChangeUnitOrder (ChangeOrder typeOfChange)
	{
		if (typeOfChange == ChangeOrder.moveDown) {
			ISelection tmp = unitSelected [0];

			for (int i = 1; i < unitSelected.Count; i++)
				unitSelected [i - 1] = unitSelected [i];

			unitSelected [unitSelected.Count] = tmp;
		} else if (typeOfChange == ChangeOrder.moveUp) {
			ISelection tmp = unitSelected [unitSelected.Count];
			for (int i = 0; i < unitSelected.Count - 1; i++)
				unitSelected [i + 1] = unitSelected [i];

			unitSelected [0] = tmp;
		}
	}
}
