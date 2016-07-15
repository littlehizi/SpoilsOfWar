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
		//Stop all walk coroutines if any
		StopCoroutine ("WalkUntilDestination");
		//Get the path
		GroundBehavior[] path = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, unitBehavior.currentTile, destination);

		//If the path is unreachable, don't bother
		if (path == null) {
			unitBehavior.ReachedDestination ();
			return;
		}
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
