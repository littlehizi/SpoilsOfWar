using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingManatherBehavior : MonoBehaviour
{
	public enum PathfindingType
	{
		BFS,
		//breath first search
		DI,
		//Digjstradsfknsdflk
		AS
		//A*
	}

	public GroundBehavior[] FindPathToTarget (PathfindingType newType, Vector2 origin, Vector2 destination)
	{

		return new GroundBehavior[10];
	}

	GroundBehavior[] FindWithBFS (Vector2 origin, Vector2 destination)
	{

		return new GroundBehavior[10];
	}
}
