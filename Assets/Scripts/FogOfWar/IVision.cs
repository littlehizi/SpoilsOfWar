using UnityEngine;
using System.Collections;

public interface IVision
{
	//Access to the vision variable
	int visionStrength { get; }

	//The current tile of the FOW vision, such as, tile where unit stands on
	GroundBehavior visionStartTile { get; }

	//Used to apply changes
	void RegisterToIVisionStorage ();

	//Remove entry when dead
	void DeleteIVisionEntry ();
}
