using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DigSelectionManagerBehavior : MonoBehaviour, IManager
{
	//Internal Vars
	private static List<GroundBehavior> groundTilesHolder;
	private static GroundBehavior temporaryTile;
	private static bool isDigging;

	public static int getGroundTilesHolderLength{ get { return groundTilesHolder.Count; } }

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
			//If the target tile is already dug, cancel and ask the player to start thinking and stop being retarded
			if (newTile.isDug)
				return;
			
			if (temporaryTile == null)
				temporaryTile = groundTilesHolder [groundTilesHolder.Count - 1];

			//Debug.Log ("Got two pos, get diggin !");

			//If the first pos and the 2nd are the same, just get a mini-path going..? Not here though
			if (temporaryTile.ID == newTile.ID)
				return;

			//If the first position has already been given, the newTile is the 2nd, and you can collect all tiles using pathfinding
			GroundBehavior[] tmpPath = PathfindingManagerBehavior.FindPathToTarget (GameMasterScript.instance.pathfindingType, temporaryTile, newTile, false);

			if (tmpPath == null) {
				return;
			}

			//Debug
			temporaryTile.tileSR.color = Color.gray;

			//Add the starting tile
			if (!groundTilesHolder.Contains (temporaryTile))
				groundTilesHolder.Add (temporaryTile);

			//Record those tiles
			for (int i = 0; i < tmpPath.Length; i++) {
				//Check if that tile isn't already in the list.
				if (groundTilesHolder.Contains (tmpPath [i]))
					continue;
				groundTilesHolder.Add (tmpPath [i]);

				tmpPath [i].tileSR.color = Color.cyan;
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
		//If the player simply double clicked on a tile, tell him off
		if (groundTilesHolder.Count == 0)
			return null;

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
		ResetTileColors ();

		//Stop the digging flag and return all optained tiles
		isDigging = false;
		return groundTilesHolder.ToArray ();
	}

	/// <summary>
	/// Reverts the tile colors to their original ones.
	/// </summary>
	public static void ResetTileColors ()
	{
		for (int i = 0; i < groundTilesHolder.Count; i++) {
			groundTilesHolder [i].tileSR.color = groundTilesHolder [i].colorBackup;
			if (groundTilesHolder [i].isDug)
				groundTilesHolder [i].ApplyDugColor ();
		}

		if (temporaryTile != null) {
			temporaryTile.tileSR.color = temporaryTile.colorBackup;
			temporaryTile = null;
		}

		//reset the flag 
		isDigging = false;
	}

	public static int GetPathLength ()
	{
		return groundTilesHolder.Count;
	}

	public static void ResetDigPath ()
	{
		//If there were tiles already selected, clear the colors
		if (groundTilesHolder.Count > 0)
			ResetTileColors ();
		Debug.Log ("Path Reseted !");
		groundTilesHolder.Clear ();
	}
}
