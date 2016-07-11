using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DigSelectionManagerBehavior : MonoBehaviour, IManager
{
	//Internal Vars
	private static List<GroundBehavior> groundTilesHolder;
	private static GroundBehavior temporaryTile;
	private static bool isDigging;


	//START METHOD
	public void OnGameStart ()
	{
		groundTilesHolder = new List<GroundBehavior> ();
		isDigging = false;
	}

	/// <summary>
	/// Add tiles to dig 
	/// </summary>
	/// <returns>The ground tiles to dig.</returns>
	public static void GetGroundTilesToDig (GroundBehavior newTile)
	{
		//Empty the list of the last digging process
		if (!isDigging)
			ResetDigPath ();

		isDigging = true;

		//If this is the first call, record the first position
		if (temporaryTile == null)
			newTile = temporaryTile;
		else {
			//If the first pos and the 2nd are the same, just leave
			if (temporaryTile.ID == newTile.ID)
				return;
			
			//If the first position has already been given, the newTile is the 2nd, and you can collect all tiles using pathfinding
			GroundBehavior[] tmpPath = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, temporaryTile, newTile, false);

			//Record those tiles
			for (int i = 0; i < tmpPath.Length; i++) {
				//Check if that tile isn't already in the list.
				if (groundTilesHolder.Contains (tmpPath [i]))
					continue;
				groundTilesHolder.Add (tmpPath [i]);
			}

			//Reset temporary variable for next digging
			temporaryTile = null;
		}
	}

	/// <summary>
	/// Outputs the tiles to dig arranged according to the player's position.
	/// </summary>
	/// <returns>The tiles to dig.</returns>
	/// <param name="playerPos">Player position.</param>
	public static GroundBehavior[] OutputTilesToDig (GroundBehavior playerPos)
	{
		//If the first tile is not 1-tile away from the [0]'s item of the path, check for the last item
		if (!PathfindingManagerBehavior.GetTileNeighbor (playerPos).Contains (groundTilesHolder [0])) {
			if (PathfindingManagerBehavior.GetTileNeighbor (playerPos).Contains (groundTilesHolder [groundTilesHolder.Count])) {
				//If the last item is 1-tile away from the player, reverse the list
				groundTilesHolder.Reverse ();
			} else {
				//If none of the tiles are connected to the player tile, return an error or now
				Debug.LogError ("Tile too far away !");
			}
		}

		//Stop the digging flag and return all optained tiles
		isDigging = false;
		return groundTilesHolder.ToArray ();
	}

	public static void ResetDigPath ()
	{
		groundTilesHolder.Clear ();
	}
}
