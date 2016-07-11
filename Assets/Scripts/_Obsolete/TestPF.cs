using UnityEngine;
using System.Collections;

public class TestPF : MonoBehaviour
{
	GroundBehavior origin;
	GroundBehavior destination;

	public LayerMask tileLayer;

	public GroundBehavior[] path;

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			Ray clickRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit stuffHit;

			if (Physics.Raycast (clickRay, out stuffHit, Mathf.Infinity, tileLayer)) {
				if (origin == null) {
					origin = stuffHit.transform.GetComponent<GroundBehavior> ();
					Debug.Log ("Origin Set!");
					return;
				} else if (origin != null && destination == null) {
					destination = stuffHit.transform.GetComponent<GroundBehavior> ();
					Debug.Log ("Destination Set!");

					//Get the path finding happening !
					path = PathfindingManagerBehavior.FindPathToTarget (PathfindingType.AS, origin, destination);

					for (int i = 0; i < path.Length; i++) {
						path [i].transform.GetComponent<Renderer> ().material.color += Color.cyan;
					}

					Debug.Log ("Pathfinding Happenned !");

					origin = null;
					destination = null;
				}
			}
		}

	}
}
