using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;

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
	public PlayerManagerBehavior PLMB;
	public FogOfWarManagerBehavior FOWMB;
	public AudioManagerBehavior AMB;
	public UserInterfaceManagerBehavior UIMB;
	public VictoryManagerBehavior VMB;
	public ResourceManagerBehavior RMB;
	public TickManagerBehavior TMB;
	public TimeManagerBehavior TIMB;
	public CursorManagerBehavior CMB;

	public IManager[] managers;

	public PathfindingType pathfindingType;

	//Fog Of War variables
	public static bool isFOWActive;

	// Grid Variables
	public int gridWidth;
	public int gridHeight;
	public GameObject[,] trenchGrid;

	//Game Variables
	public UnitData[] playerCharacters = new UnitData[6];
	public int daysInARound;

	//Unit Variable
	public int startResources;
	public float delayBetweenTicks;
	public float baseUnitSpeed;
	public int staminaCostMove;
	public int staminaCostDig;
	public int staminaCostFight;
	public float exhaustEfficiencyModifier;
	public float combatSpeed;
	public int oxygenLossPerTile;
	public int bombExplosionRadius;
	public float damagePerBomb;
	public int farBombDamage;
	public int resourceUsedPerFortification;
	public Vector3 tileHpBeforeNextFrame;

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

	#region FSM


	private static BaseState _currentState;

	public static BaseState currentState {
		get { return _currentState; }
		set {
			if (_currentState != null) {
				_currentState.OnStateExit ();
				Destroy (_currentState);
			}

			_currentState = value;

			_currentState.OnStateEnter ();
		}
	}

	private static System.Type[] possibleStateTypes;

	public static void ChangeState<T> () where T : BaseState
	{
		//Get all the types currently running
		System.Type[] allTypes = Assembly.GetExecutingAssembly ().GetTypes ();

		//Get all types that are inheritances of BaseState
		if (possibleStateTypes == null) {
			possibleStateTypes = (from System.Type type in allTypes
			                      where type.IsSubclassOf (typeof(BaseState))
			                      select type).ToArray ();
		}

		//loop through them. If the given type is present in that list, add it as the state.
		for (int i = 0; i < possibleStateTypes.Length; i++) {
			if (possibleStateTypes [i] == typeof(T)) {
				GameMasterScript.currentState = GameMasterScript.instance.gameObject.AddComponent<T> ();
			}
		}
	}




	#endregion

	void Start ()
	{
		//MAIN START
		GameMasterScript.ChangeState<State_MainMenu> ();
	}

	//Method will be run when State_Game begins.
	public void StartGame ()
	{
		//ORDER MATTERS !! DON'T TOUCH IF YOU DON'T KNOW
		if (managers == null)
			managers = new IManager[12]{ PLMB, GMB, FOWMB, IMB, SMB, PMB, DSMB, USMB, VMB, RMB, TMB, TIMB };

		//Initialize Managers
		for (int i = 0; i < managers.Length; i++)
			managers [i].OnGameStart ();
		

		// Spawn all the units
		PLMB.SpawnBaseUnit ();
	}

	/*
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
*/
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

