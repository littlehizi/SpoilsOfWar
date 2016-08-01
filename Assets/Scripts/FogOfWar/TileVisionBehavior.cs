using UnityEngine;
using System.Collections;

public class TileVisionBehavior : MonoBehaviour, IVision
{
	//Stats
	public int tileVision;
	[HideInInspector]public GroundBehavior tileReference;

	//Ivision
	public int visionStrength { get { return tileVision; } }


	public GroundBehavior visionStartTile {
		get { 
			if (tileReference == null)
				tileReference = this.GetComponent<GroundBehavior> ();
			return tileReference;
		} 
	}

	void OnEnable ()
	{
		RegisterToIVisionStorage ();
	}

	//Register to the FOW vision system
	public void RegisterToIVisionStorage ()
	{
		GameMasterScript.instance.FOWMB.visionBeacons.Add (this);
	}


	void OnDisable ()
	{
		DeleteIVisionEntry ();
	}
	//Tiles are static, so no need to delete entry
	public void DeleteIVisionEntry ()
	{
		GameMasterScript.instance.FOWMB.visionBeacons.Remove (this);
	}
}
