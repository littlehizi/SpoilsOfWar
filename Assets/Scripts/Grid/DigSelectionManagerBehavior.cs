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


		//If this is the first call, record the first position
		if (temporaryTile == null && !isDigging) {
			temporaryTile = newTile;
			isDigging = true;
		} else {
			if (temporaryTile == null)
				temporaryTile = groundTilesHolder [groundTilesHolder.Count - 1];

			Debug.Log ("Got two pos, get diggin !");
			//If the first pos and the 2nd are the same, just leave
			if (temporaryTile.ID == newTile.ID)
				return;

			//Debug
			temporaryTile.GetComponent<SpriteRenderer> ().color = Color.gray;
			newTile.GetComponent<SpriteRenderer> ().color = Color.yellow;


			//If the first position has already been given, the newTile is the 2nd, and you can collect all tiles using pathfinding
			GroundBehavior[] tmpPath = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, temporaryTile, newTile, false);

			//Add the starting tile
			if (!groundTilesHolder.Contains (temporaryTile))
				groundTilesHolder.Add (temporaryTile);

			//Record those tiles
			for (int i = 0; i < tmpPath.Length; i++) {
				//Check if that tile isn't already in the list.
				if (groundTilesHolder.Contains (tmpPath [i]))
					continue;
				groundTilesHolder.Add (tmpPath [i]);

				tmpPath [i].GetComponent<SpriteRenderer> ().color = Color.cyan;
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
		if (!PathfindingManagerBehavior.ListContainsGroundTile (PathfindingManagerBehavior.GetTileNeighbor (playerPos), groundTilesHolder [0])) {
			if (PathfindingManagerBehavior.ListContainsGroundTile (PathfindingManagerBehavior.GetTileNeighbor (playerPos), groundTilesHolder [groundTilesHolder.Count - 1])) {
				//If the last item is 1-tile away from the player, reverse the list
				groundTilesHolder.Reverse ();
			} else {
				//If none of the tiles are connected to the player tile, return an error or now
				Debug.LogWarning ("Tile too far away !");
			}
		}

		//reset the color
		for (int i = 0; i < groundTilesHolder.Count; i++)
			groundTilesHolder [i].GetComponent<SpriteRenderer> ().color = groundTilesHolder [i].colorBackup;

		//Stop the digging flag and return all optained tiles
		isDigging = false;
		return groundTilesHolder.ToArray ();
	}

	public static int GetPathLength ()
	{
		return groundTilesHolder.Count;
	}

	public static void ResetDigPath ()
	{
		Debug.Log ("Path Reseted !");
		groundTilesHolder.Clear ();
	}
}
