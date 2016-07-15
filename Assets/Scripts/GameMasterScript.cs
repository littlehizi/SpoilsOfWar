using UnityEngine;
using System.Collections;

//using System.Collections.Generic;

public class GameMasterScript : MonoBehaviour
{
	#region Variables

	//REFERENCES
	public GridManagerBehavior GMB;
	public InputManagerBehavior IMB;
	public SelectionManagerBehavior SMB;
	public PathfindingManagerBehavior PMB;
	public DigSelectionManagerBehavior DSMB;
	public UnitSpawnerManagerBehavior USMB;

	public IManager[] managers;

	public PathfindingType pathfindingType;

	//public GameObject selectedUnit;


	// Grid Variables
	public int gridWidth;
	public int gridHeight;
	public GameObject[,] trenchGrid;

	//Unit Variable
	public float baseUnitSpeed;
	public int staminaCostMove;
	public int staminaCostDig;

	public UnitScript unitScript;

	// Camera Variables
	public static WorldCamera.BoxLimit cameraLimits = new WorldCamera.BoxLimit ();
	public static WorldCamera.BoxLimit mouseScrollLimits = new WorldCamera.BoxLimit ();

	#endregion

	#region static call

	public static GameMasterScript instance;

	public GameMasterScript ()
	{
		instance = this;
	}

	#endregion

	public void Start ()
	{
		managers = new IManager[6]{ GMB, IMB, SMB, PMB, DSMB, USMB };

		//Initialize Managers
		for (int i = 0; i < managers.Length; i++)
			managers [i].OnGameStart ();
		
		//Initialize inputs
		IMB.currentState = InputManagerBehavior.InputState.idle;

		//Start the game
		GMB.CreateGrid ();
	}

	#region Grid tapping

	public void StartTap (int x, int y)
	{
		for (int i = 0; i < gridWidth; ++i) {
			for (int j = 0; j < gridHeight; ++j) {
				if (trenchGrid [i, j] != null) {
					trenchGrid [i, j].GetComponent<TileScript> ().distance = 10000;
				}
			}
		}

		TapCubes (x, y, 0);
	}

	void TapCubes (int x, int y, int distance)
	{
		if (trenchGrid [x, y] != null) {
			if (trenchGrid [x, y].GetComponent<TileScript> ().distance > distance) {
				//trenchGrid[x, y].name = distance.ToString();
				trenchGrid [x, y].GetComponent<TileScript> ().distance = distance;

				if (x + 1 < gridWidth) {
					TapCubes (x + 1, y, distance + 1);
				}

				if (y + 1 < gridHeight) {
					TapCubes (x, y + 1, distance + 1);
				}

				if (x - 1 >= 0) {
					TapCubes (x - 1, y, distance + 1);
				}

				if (y - 1 >= 0) {
					TapCubes (x, y - 1, distance + 1);
				}
			}
		}
	}

	#endregion

	/*

	#region Update()

	void Update ()
	{
		// If left mouse button clicked
		if (Input.GetMouseButton (0)) {
			// Deselect current selected unit if it exists
			// Also deselects if we click on something other than a unit
			if (selectedUnit != null) {
				selectedUnit = null;
				unitScript.isSelected = false;
			}

			// Cast raycast from the mouse position, to the world
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				// Select unit only if it has the tag "Unit"
				if (hit.transform.tag == "Unit") {
					selectedUnit = hit.transform.gameObject;
					unitScript = selectedUnit.GetComponent<UnitScript> ();
					unitScript.gameMaster = this;
					// Tell the unit that it has been selected so it is able move, attack, etc.
					if (unitScript.isSelected == false) {
						unitScript.isSelected = true;
					}
				}
			}
		}

		if (Input.GetMouseButtonUp (1)) {
			// Cast raycast from the mouse position, to the world
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				// Select the ground only if it has the tag "DirtTile"
				if (hit.transform.tag == "DirtTile") {
					//if (unitScript.isSelected)
					if (selectedUnit != null) {
						unitScript.destination = hit.transform.gameObject;
						unitScript.unitState = UnitScript.State.Moving;
					}
				}
			}
		}
	}

	#endregion

*/
}

