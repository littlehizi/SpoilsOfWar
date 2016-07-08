using UnityEngine;
using System.Collections;

/// <summary>
/// This class is responsible for holding all the tiles.
/// </summary>
[System.Serializable]
public class Grid
{
	public GroundBehavior[,] tiles;

	public Grid (int xLength, int yLength)
	{
		tiles = new GroundBehavior[xLength, yLength];
	}
}