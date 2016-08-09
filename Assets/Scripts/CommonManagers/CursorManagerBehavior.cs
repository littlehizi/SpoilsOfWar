using UnityEngine;
using System.Collections;

public class CursorManagerBehavior : MonoBehaviour, IManager
{
	public enum CursorState
	{
		idle,
		pointDigging,
		pointFighting
	}

	CursorState _currentCursorState;

	public CursorState currentCursorState {
		get{ return _currentCursorState; }
		set {
			_currentCursorState = value;
			Cursor.SetCursor (cursors [(int)_currentCursorState], Vector2.zero, CursorMode.ForceSoftware);
		}
	}


	//internal variables
	public bool canChangeCursor;
	public Texture2D[] cursors;
	public LayerMask groundLayer;

	public void OnGameStart ()
	{
		currentCursorState = CursorState.idle;
	}


	void FixedUpdate ()
	{
		if (!canChangeCursor)
			return;
//		RaycastHit stuffHit;
//
//		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out stuffHit, Mathf.Infinity, groundLayer)) {
//			if (stuffHit != null) {
//				GroundBehavior tile = stuffHit.transform.GetComponent<GroundBehavior> ();
//
		Vector2 tileHit = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		tileHit = new Vector2 (Mathf.RoundToInt (tileHit.x), Mathf.RoundToInt (tileHit.y));
		GroundBehavior tile = GameMasterScript.instance.GMB.TileposToTile (tileHit);

		if (tile.unitsOnTile.Count > 0) {
			if (tile.unitsOnTile [0].alignment == PlayerData.TypeOfPlayer.enemy) {
				//Change to pointFighting
				currentCursorState = CursorState.pointFighting;
			}
		} else if (!tile.isDug) {
			//Change to pointDigging
			currentCursorState = CursorState.pointDigging;
		} else if (currentCursorState != CursorState.idle) {
			//Change to idle
			currentCursorState = CursorState.idle;
		}


//			}
//		}

	
	}
}
