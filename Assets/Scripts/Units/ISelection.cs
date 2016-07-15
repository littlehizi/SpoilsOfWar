using UnityEngine;
using System.Collections;

public interface ISelection
{
	void OnSelect ();

	GroundBehavior currentTile{ get; set; }

	void OnDeselect ();
}
