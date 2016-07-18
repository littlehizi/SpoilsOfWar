using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnitBehavior))]
public class UnitMovementBehavior : MonoBehaviour
{
	UnitBehavior unitBehavior;

	void Awake ()
	{
		unitBehavior = this.GetComponent<UnitBehavior> ();
	}

	/// <summary>
	/// Moves the unit to target.
	/// </summary>
	/// <param name="destination">Destination.</param>
	public void MoveUnitToTarget (GroundBehavior destination)
	{
		//Get the path
		GroundBehavior[] path = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, unitBehavior.currentTile, destination);

		//If the path is unreachable, don't bother
		if (path == null || path.Length == 0) {
			unitBehavior.ReachedDestination ();
			return;
		}

		//Stop all walk coroutines if any
		StopCoroutine ("WalkUntilDestination");

		//Start the walk coroutine
		StartCoroutine ("WalkUntilDestination", path);
	}


	IEnumerator WalkUntilDestination (GroundBehavior[] path)
	{
		int currentIndex = 0;

		//if the first tile is the player's current tile, skip it
		if (path [0].ID == unitBehavior.currentTile.ID)
			currentIndex = 1;

		while (currentIndex < path.Length) {
			//Check if there's any enemy unit on next tile
			if (path [currentIndex].unitsOnTile.Count > 0 && path [currentIndex].unitsOnTile [0].alignment != unitBehavior.alignment) {
				//Stop walking and engage combat
				unitBehavior.EngageCombat (path [currentIndex]);
				StopWalking ();
			}

			//Wait according to the speed stat. 
			yield return new WaitForSeconds (GameMasterScript.instance.baseUnitSpeed / (float)unitBehavior.unitData.speed);

			//Move the unit to the next tile
			Vector3 tmpPos = path [currentIndex].tilePos;
			tmpPos.y *= -1;
			tmpPos.z = -2;
			this.transform.position = tmpPos;

			//Set current tile to new tile
			unitBehavior.currentTile = path [currentIndex];

			//Remove some stamina
			unitBehavior.stamina -= GameMasterScript.instance.staminaCostMove;

			//Update index
			currentIndex++;
		}

		unitBehavior.ReachedDestination ();
	}

	/// <summary>
	/// Stops the unit walking.
	/// </summary>
	public void StopWalking ()
	{
		StopCoroutine ("WalkUntilDestination");
	}
}
