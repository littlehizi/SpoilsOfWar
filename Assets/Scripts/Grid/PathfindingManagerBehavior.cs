using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum PathfindingType
{
	BFS,
	//breadth first search
	DI,
	//Dijkstra
	AS
	//A*
}



public class PathfindingManagerBehavior : MonoBehaviour, IManager
{
	//START METHOD
	public void OnGameStart ()
	{
	}

	/// <summary>
	/// Based on a type of pathfinding, this method will calculate using the appropriate algorithm 
	/// and return a path as a GroundBehavior array.
	/// </summary>
	/// <returns>The path to target.</returns>
	/// <param name="newType">New type.</param>
	/// <param name="origin">Origin.</param>
	/// <param name="destination">Destination.</param>
	public static GroundBehavior[] FindPathToTarget (PathfindingType newType, GroundBehavior origin, GroundBehavior destination, bool hasToBeDug = true)
	{
		switch (newType) {
		case PathfindingType.BFS:
			return FindWithBFS (origin, destination, hasToBeDug);
		case PathfindingType.AS:
			return FindWithAStar (origin, destination, hasToBeDug);
		}

		return null;
	}

	#region BFS

	/// <summary>
	/// This method will return a path (array of GroundBehavior from origin to destination) using Breath First Search algorithm
	/// </summary>
	/// <returns>The with BF.</returns>
	/// <param name="origin">Origin.</param>
	/// <param name="destination">Destination.</param>
	static GroundBehavior[] FindWithBFS (GroundBehavior origin, GroundBehavior destination, bool hasToBeDug)
	{
		//Open list are the positions to check
		List<GroundBehavior> open = new List<GroundBehavior> ();
		//Closed list are the positions that aren't available or that are checked already
		List<GroundBehavior> closed = new List<GroundBehavior> ();

		//Start searching from the character's position
		open.Add (origin);
		bool pathFound = false;

		//Loop until full path found
		while (open.Count > 0 && !pathFound) {
			
			//Look at the current position and place it in the "checked" list
			GroundBehavior currentTile = open [0];
			closed.Add (currentTile);
			open.RemoveAt (0);

			//Check if the current position is the destination, then the path is found and you can move to next step
			if (currentTile.ID == destination.ID) {
				pathFound = true;
			} else {
				//If not valid, then look at its neighbors
				List<GroundBehavior> successors = GetTileNeighbor (currentTile);

				//Add all available neighbors to the open list
				for (int i = 0; i < successors.Count; i++) {
					if (!ListContainsGroundTile (closed, successors [i]) && !ListContainsGroundTile (open, successors [i])) {
						//CHECK IF DUG
						if (hasToBeDug) {
							if (!successors [i].isDug)
								continue;
						}

						//Make a path using the "parent" storage
						successors [i].parent = currentTile;
						open.Add (successors [i]);
					}
				}
			}
		}

		//Create the list output and add the destination first
		List<GroundBehavior> listOutput = new List<GroundBehavior> ();
		listOutput.Add (destination);


		//Add the next position to check to get ready for next go
		GroundBehavior parent = destination.parent;

		while (parent != null) {
			listOutput.Add (parent);
			parent = parent.parent;
		}

		//Flip the list so the next position is first
		listOutput.Reverse ();

		return listOutput.ToArray ();
	}

	#endregion

	#region A*

	private class AStarNode : IHeapContent<AStarNode>
	{
		public GroundBehavior tile;
		public int gCost, hCost;

		public int fCost { get { return gCost + hCost; } }

		public AStarNode parent;

		public AStarNode (GroundBehavior newTile)
		{
			tile = newTile;
		}

		//region IHeapContent vars

		private int _heapIndex;

		public int heapIndex {
			get{ return _heapIndex; }
			set { _heapIndex = value; }
		}

		/// <summary>
		/// Compares the current node to another, firstly based on FCOST, and then on HCOST if both FCOSt are equals.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="newNode">New node.</param>
		public int CompareTo (AStarNode newNode)
		{
			int compare = fCost.CompareTo (newNode.fCost);

			if (compare == 0)
				compare = hCost.CompareTo (newNode.hCost);

			return -compare;
		}
	}

	private static int mapSize { get { return GameMasterScript.instance.gridWidth * GameMasterScript.instance.gridHeight; } }

	static GroundBehavior[] FindWithAStar (GroundBehavior origin, GroundBehavior destination, bool hasToBeDug)
	{
		//Check if the origin and destination are the same tile. If so, just return the destination as a path.
		if (origin.ID == destination.ID) {
			GroundBehavior[] path = new GroundBehavior[1];
			path [0] = destination;
			return path;
		}

		//Open list are the positions to check
		Heap<AStarNode> open = new Heap<AStarNode> (mapSize);
		//Closed list are the positions that aren't available or that are checked already
		List<AStarNode> closed = new List<AStarNode> (mapSize);

		//Prepare a destination node and a origin node
		AStarNode originNode = new AStarNode (origin);
		AStarNode destinationNode = new AStarNode (destination);

		//Start searching from the character's position
		open.Add (originNode);
		bool pathFound = false;

		while (open.Count > 0 && !pathFound) {
			//Check if the path cannot be found. Return null in this case.
			if (open.Count >= mapSize - 1 || closed.Count >= mapSize - 1)
				return null;

			//Get a new node
			AStarNode currentNode = open.RemoveFirst ();

			closed.Add (currentNode);

			//If destination has been reached
			if (currentNode.tile.ID == destinationNode.tile.ID) {
				destinationNode = currentNode;
				pathFound = true;
			}

			//Get all the neighbors and add those in the list
			List<GroundBehavior> currentNeighbors = GetTileNeighbor (currentNode.tile);
			List<AStarNode> neighborNodes = new List<AStarNode> ();

			int dugCount = 0; 

			for (int i = 0; i < currentNeighbors.Count; i++) {
				//CHECK IF DUG
				if (hasToBeDug) {
					if (!currentNeighbors [i].isDug) {
						dugCount++;
						continue;
					}
				}

				//Change the tile into a node
				neighborNodes.Add (new AStarNode (currentNeighbors [i]));

				//If that neighbor has already been checked, don't bother
				if (ListContainsGroundTile (closed, neighborNodes [i - dugCount]))
					continue;


				int newMovCost = currentNode.gCost + GetDistance (currentNode, neighborNodes [i - dugCount]);

				if (newMovCost < neighborNodes [i - dugCount].gCost || !open.Contains (neighborNodes [i - dugCount])) {
					//Calculate new costs
					neighborNodes [i - dugCount].gCost = newMovCost;
					neighborNodes [i - dugCount].hCost = GetDistance (neighborNodes [i - dugCount], destinationNode);

					//SetNeighbor
					neighborNodes [i - dugCount].parent = currentNode;

					if (!open.Contains (neighborNodes [i - dugCount]))
						open.Add (neighborNodes [i - dugCount]);
					else
						open.UpdateObject (neighborNodes [i - dugCount]);
				}
			}
		}

		//Create a path
		List<AStarNode> nodePath = new List<AStarNode> ();
		AStarNode currentPathNode = destinationNode;

		bool isNull = false;
		while (currentPathNode != originNode) {
			nodePath.Add (currentPathNode);
			if (currentPathNode.parent == null) {
				isNull = true;
				break;
			}
			currentPathNode = currentPathNode.parent;
		}

		if (isNull)
			return null;

		//Reverse !
		nodePath.Reverse ();

		//Create GroundBehavior list output
		GroundBehavior[] output = new GroundBehavior[nodePath.Count];

		for (int i = 0; i < nodePath.Count; i++)
			output [i] = nodePath [i].tile;

		return output;
	}


	/// <summary>
	/// Gets the distance between two nodes.
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="nodeA">Node a.</param>
	/// <param name="nodeB">Node b.</param>
	static int GetDistance (AStarNode nodeA, AStarNode nodeB)
	{
		int xDistance = (int)Mathf.Abs (nodeA.tile.tilePos.x - nodeB.tile.tilePos.x);
		int yDistance = (int)Mathf.Abs (nodeA.tile.tilePos.y - nodeB.tile.tilePos.y);

		if (xDistance > yDistance)
			return 14 * xDistance + 10 * (xDistance - yDistance);

		return 14 * yDistance + 10 * (yDistance - xDistance);
	}

	#endregion

	#region Pathfinding tool methods

	/// <summary>
	/// Gets the tile neighbors based on the groundbehavior ID (SEE GRIDMANAGERBEHAVIOR)
	/// </summary>
	/// <returns>The tile neighbor.</returns>
	/// <param name="currenTile">Curren tile.</param>
	public static List<GroundBehavior> GetTileNeighbor (GroundBehavior currenTile)
	{
		//Check UP, DOWN, LEFT, RIGHT according to the IDs
		List<GroundBehavior> output = new List<GroundBehavior> ();


		foreach (GroundBehavior GB in GameMasterScript.instance.GMB.currentGrid.tiles) {
			//UP
			if (GB.ID == currenTile.ID + GridManagerBehavior.ID_X_DIGGIT)
				output.Add (GB);

			//DOWN
			if (GB.ID == currenTile.ID - GridManagerBehavior.ID_X_DIGGIT)
				output.Add (GB);

			//LEFT
			if (GB.ID == currenTile.ID - 1)
				output.Add (GB);

			//RIGHT
			if (GB.ID == currenTile.ID + 1)
				output.Add (GB);
		}

		return output;
	}

	/// <summary>
	/// Replaces List<T>.Contains for GroundBehavior, by checking their ID.
	/// </summary>
	/// <returns><c>true</c>, if contains ground tile was listed, <c>false</c> otherwise.</returns>
	/// <param name="currentList">Current list.</param>
	/// <param name="currentGround">Current ground.</param>
	public static bool ListContainsGroundTile (List<GroundBehavior> currentList, GroundBehavior currentGround)
	{
		for (int i = 0; i < currentList.Count; i++) {
			if (currentList [i].ID == currentGround.ID)
				return true;
		}

		return false;
	}

	static bool ListContainsGroundTile (List<AStarNode> currentList, AStarNode currentGround)
	{
		for (int i = 0; i < currentList.Count; i++) {
			if (currentList [i].tile.ID == currentGround.tile.ID)
				return true;
		}

		return false;
	}

	#endregion

	#region OBSTACLE FINDING USING A*

	//Yes, I was too lazy to fix the one above, so am takin an easy step

	public static GroundBehavior[] FindWithAStarNoObstacles (GroundBehavior origin, GroundBehavior destination)
	{
		Debug.Log ("wee");
		//Check if the origin and destination are the same tile. If so, just return the destination as a path.
		if (origin.ID == destination.ID) {
			GroundBehavior[] path = new GroundBehavior[1];
			path [0] = destination;
			return path;
		}

		//Open list are the positions to check
		Heap<AStarNode> open = new Heap<AStarNode> (mapSize);
		//Closed list are the positions that aren't available or that are checked already
		List<AStarNode> closed = new List<AStarNode> (mapSize);

		//Prepare a destination node and a origin node
		AStarNode originNode = new AStarNode (origin);
		AStarNode destinationNode = new AStarNode (destination);

		//Start searching from the character's position
		open.Add (originNode);
		bool pathFound = false;

		while (open.Count > 0 && !pathFound) {
			//Check if the path cannot be found. Return null in this case.
			if (open.Count >= mapSize - 1 || closed.Count >= mapSize - 1)
				return null;

			//Get a new node
			AStarNode currentNode = open.RemoveFirst ();

			closed.Add (currentNode);

			//If destination has been reached
			if (currentNode.tile.ID == destinationNode.tile.ID) {
				destinationNode = currentNode;
				pathFound = true;
			}

			//Get all the neighbors and add those in the list
			List<GroundBehavior> currentNeighbors = GetTileNeighbor (currentNode.tile);
			List<AStarNode> neighborNodes = new List<AStarNode> ();

			int dugCount = 0; 

			for (int i = 0; i < currentNeighbors.Count; i++) {
				//CHECK IF DUG
				if (currentNeighbors [i].isAnObstacle) {
					dugCount++;
					continue;
				}


				//Change the tile into a node
				neighborNodes.Add (new AStarNode (currentNeighbors [i]));

				//If that neighbor has already been checked, don't bother
				if (ListContainsGroundTile (closed, neighborNodes [i - dugCount]))
					continue;


				int newMovCost = currentNode.gCost + GetDistance (currentNode, neighborNodes [i - dugCount]);

				if (newMovCost < neighborNodes [i - dugCount].gCost || !open.Contains (neighborNodes [i - dugCount])) {
					//Calculate new costs
					neighborNodes [i - dugCount].gCost = newMovCost;
					neighborNodes [i - dugCount].hCost = GetDistance (neighborNodes [i - dugCount], destinationNode);

					//SetNeighbor
					neighborNodes [i - dugCount].parent = currentNode;

					if (!open.Contains (neighborNodes [i - dugCount]))
						open.Add (neighborNodes [i - dugCount]);
					else
						open.UpdateObject (neighborNodes [i - dugCount]);
				}
			}
		}

		//Create a path
		List<AStarNode> nodePath = new List<AStarNode> ();
		AStarNode currentPathNode = destinationNode;

		bool isNull = false;
		while (currentPathNode != originNode) {
			nodePath.Add (currentPathNode);
			if (currentPathNode.parent == null) {
				isNull = true;
				break;
			}
			currentPathNode = currentPathNode.parent;
		}

		if (isNull)
			return null;

		//Reverse !
		nodePath.Reverse ();

		//Create GroundBehavior list output
		GroundBehavior[] output = new GroundBehavior[nodePath.Count];

		for (int i = 0; i < nodePath.Count; i++)
			output [i] = nodePath [i].tile;

		return output;

	}


	#endregion
}
