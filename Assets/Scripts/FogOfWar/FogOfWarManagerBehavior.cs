using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWarManagerBehavior : MonoBehaviour, IManager
{
	public enum VisibleState
	{
		hidden,
		halfHidden,
		visible
	}

	public enum LightType
	{
		constant,
		dynamic
	}

	//Prefab
	public GameObject FOW_Prefab;

	//Colors
	public Color[] colorStates;

	//LAWS
	const int SMOOTH_DISTANCE = 1;

	//Internal vars
	public List<IVision> visionBeacons;
	public FOWTileBehavior[,] fowSRStorage;
	public LayerMask tileMask;
	[HideInInspector]public List<FOWTileBehavior> tmpTileScanStorage;

	void Awake ()
	{
		//Initialize arrays / lists
		visionBeacons = new List<IVision> ();
		tmpTileScanStorage = new List<FOWTileBehavior> ();

	}

	//Start
	public void OnGameStart ()
	{
		
		//Create all the FOW tiles
		CreateFOW ();

		StartCoroutine (FadeIn ());
	}

	IEnumerator FadeIn ()
	{
		yield return new WaitForSeconds (0.3f);
		UpdateFOW ();
	}

	void CreateFOW ()
	{
		//Initalize the storage
		fowSRStorage = new FOWTileBehavior[GameMasterScript.instance.gridWidth, GameMasterScript.instance.gridHeight];
		GameObject fowHolder = new GameObject ();
		fowHolder.name = "FogOfWar Holder";

		for (int i = 0; i < GameMasterScript.instance.gridHeight; i++) {
			for (int k = 0; k < GameMasterScript.instance.gridWidth; k++) {
				//Create a position and instantiate
				Vector3 spawnPos = new Vector3 (k, -i, 0);
				FOWTileBehavior tmpTile = ((GameObject)Instantiate (FOW_Prefab, spawnPos, Quaternion.identity)).GetComponent<FOWTileBehavior> ();

				//Store the FOW tile
				fowSRStorage [k, i] = tmpTile;


				//Do clean hierarchy suff
				tmpTile.gameObject.name = "Fog Of War" + k + "," + i;
				tmpTile.transform.SetParent (fowHolder.transform);
			}
		}
	}


	/// <summary>
	/// Updates the Fog Of War according to all the IVISION components stored in visionBeacons.
	/// This method can get expensive, so don't abuse it too much!
	/// </summary>
	public void UpdateFOW ()
	{
		
		List<FOWTileBehavior> tilesToRemove = new List<FOWTileBehavior> ();

		//First check all items. If they are too distant from all IVISIONS, remove them. Do change their VisibleState if needed.
		if (tmpTileScanStorage.Count > 0) {
			for (int i = 0; i < tmpTileScanStorage.Count; i++) {
				//If the tile is too far from all beacons, remove it from the list
				int countCheck = 0;
				for (int k = 0; k < visionBeacons.Count; k++) {
					//Check for outskirts
					if (tmpTileScanStorage [i].currentState == VisibleState.halfHidden &&
					    Vector2.Distance (visionBeacons [k].visionStartTile.trueTilePos, tmpTileScanStorage [i].transform.position) <= visionBeacons [k].visionStrength + SMOOTH_DISTANCE) {

						//Check if it can be moved to VISIBLE. Only one approval is enough.
						if (Vector2.Distance (visionBeacons [k].visionStartTile.trueTilePos, tmpTileScanStorage [i].transform.position) <= visionBeacons [k].visionStrength)
							tmpTileScanStorage [i].currentState = VisibleState.visible;
						
						countCheck++;
					}

					//Check for inside outskirts
					if (tmpTileScanStorage [i].currentState == VisibleState.visible &&
					    Vector2.Distance (visionBeacons [k].visionStartTile.tilePos, tmpTileScanStorage [i].transform.position) <= visionBeacons [k].visionStrength) {
						countCheck++;
					}
				}

				if (countCheck <= 0)
					tilesToRemove.Add (tmpTileScanStorage [i]);
			}
		}

		for (int i = 0; i < tilesToRemove.Count; i++) {
			//TO LEAVE DISCOVERED AREA HALF-HIDDEN INSTEAD OF PITCH BLACK, COMMENT THE LINE BELOW 
			tilesToRemove [i].currentState = VisibleState.hidden;
			tmpTileScanStorage.Remove (tilesToRemove [i]);
		}
		

		//Then, OVERLAP to find all new outskirt tiles
		for (int i = 0; i < visionBeacons.Count; i++) {
			Collider2D[] overlapResult = Physics2D.OverlapCircleAll (visionBeacons [i].visionStartTile.trueTilePos, (float)visionBeacons [i].visionStrength + SMOOTH_DISTANCE, tileMask);

			for (int k = 0; k < overlapResult.Length; k++) {
				GroundBehavior tmpGB = overlapResult [k].GetComponent<GroundBehavior> ();
				FOWTileBehavior tmpFOWT = fowSRStorage [(int)tmpGB.tilePos.x, (int)tmpGB.tilePos.y];

				//If it doesn't already appear in the storage, put it in (lolnope) and change its state
				if (!tmpTileScanStorage.Contains (tmpFOWT) && tmpFOWT.currentState != VisibleState.halfHidden) {
					tmpTileScanStorage.Add (tmpFOWT);
					tmpFOWT.currentState = VisibleState.halfHidden;
				}
			}
		}

		//Then, OVERLAP to find all new inside tiles
		for (int i = 0; i < visionBeacons.Count; i++) {
			Collider2D[] overlapResult = Physics2D.OverlapCircleAll (visionBeacons [i].visionStartTile.trueTilePos, (float)visionBeacons [i].visionStrength, tileMask);

			for (int k = 0; k < overlapResult.Length; k++) {
				GroundBehavior tmpGB = overlapResult [k].GetComponent<GroundBehavior> ();
				FOWTileBehavior tmpFOWT = fowSRStorage [(int)tmpGB.tilePos.x, (int)tmpGB.tilePos.y];

				//If it doesn't already appear in the storage (which is impossible..), put it in (lolnope) and change its state
				if (tmpFOWT.currentState != VisibleState.visible) {
					tmpFOWT.currentState = VisibleState.visible;
				}
			}
		}
	}

	/*
	void OnDrawGizmos ()
	{
		if (visionBeacons != null && visionBeacons.Count > 0) {
			for (int i = 0; i < visionBeacons.Count; i++) {
				Gizmos.DrawWireSphere ((Vector3)visionBeacons [i].visionStartTile.trueTilePos, (float)visionBeacons [i].visionStrength);
			}
		}
	}
	*/
}
