using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScript : MonoBehaviour
{
	#region Variables

	public bool isSelected = false;
	public GameObject gridLocation;
	public int tileDistance;
	public int tileX;
	public int tileY;

	// Timer used to update every action together
	public float actionTimer;
	public float counter;

	//public GameObject[] tilePath;
	public GameObject destination;
	public List<GameObject> tilePathList;

	// Movement
	public bool isUnitMoving = false;

	public GameMasterScript gameMaster;

	public enum State
	{
		Idle,
		Moving,

		GoingToFight,
		GoingToDig,
		GoingToListen,

		Digging,
		Fighting,
		Listening,
	}

	public State unitState;

	// Used for digging (Pseudo-code)
	//int blockHealth = tileScript.blockHealth;

	// Used for fighting (Pseudo-code)
	//int unitHealth;
	//int enemyUnitHealth = enemyUnitScript.unitHealth;

	#endregion

	public void GoToActionLocation (GameObject actionLocation)
	{
		//int destinationX = actionLocation.GetComponent<TileScript>().x;
		// int destinationY = actionLocation.GetComponent<TileScript>().y;

		if (isUnitMoving) {
			//gameMaster.UnitMove(tileDistance);
			UnitMove (tileDistance);
            
			if (tileDistance <= 0) {
				isUnitMoving = false;
			}

			for (int i = 0; i < tilePathList.Count; ++i) {
				transform.position = new Vector3 (tilePathList [i].transform.position.x, tilePathList [i].transform.position.y, -1);
				tilePathList.Clear ();
			}

		}
	}

	#region Unit movement

	public void UnitMove (int numOfTimesToRun)
	{
		// If the current position is at the max X
		if (tileX >= gameMaster.gridWidth - 1) {
			for (int x = tileX - 1; x <= tileX; ++x) {
				// If the current position is at the max Y
				if (tileY >= gameMaster.gridHeight - 1) {
					for (int y = tileY - 1; y <= tileY; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
						}
					}
				}
                // If the current position is at the min Y
                else if (tileY <= 0) {
					for (int y = tileY; y <= tileY + 1; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
                           
						}
					}
				} else {
					for (int y = tileY - 1; y <= tileY + 1; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//gameMaster.selectedUnit.transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
						}
					}
				}
			}
		}
        // If the current position is at the min X
        else if (tileX <= 0) {
			for (int x = tileX; x <= tileX + 1; ++x) {
				// If the current position is at the max Y
				if (tileY >= gameMaster.gridHeight - 1) {
					for (int y = tileY - 1; y <= tileY; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//gameMaster.selectedUnit.transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
						}
					}
				}
                // If the current position is at the min Y
                else if (tileY <= 0) {
					for (int y = tileY; y <= tileY + 1; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//gameMaster.selectedUnit.transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
						}
					}
				} else {
					for (int y = tileY - 1; y <= tileY + 1; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
						}
					}
				}
			}
		} else {
			for (int x = tileX - 1; x <= tileX + 1; ++x) {
				// If the current position is at the max Y
				if (tileY >= gameMaster.gridHeight - 1) {
					for (int y = tileY - 1; y <= tileY; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							//gameMaster.selectedUnit.transform.position = new Vector3(gameMaster.trenchGrid[x, y].transform.position.x, gameMaster.trenchGrid[x, y].transform.position.y, -1);
						}
					}
				}
                // If the current position is at the min Y
                else if (tileY <= 0) {
					for (int y = tileY; y <= tileY + 1; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							gameMaster.selectedUnit.transform.position = new Vector3 (gameMaster.trenchGrid [x, y].transform.position.x, gameMaster.trenchGrid [x, y].transform.position.y, -1);
						}
					}
				} else {
					for (int y = tileY - 1; y <= tileY + 1; ++y) {
						if (gameMaster.trenchGrid [x, y] != null && gameMaster.trenchGrid [x, y].GetComponent<TileScript> ().distance < tileDistance) {
							tilePathList.Add (gameMaster.trenchGrid [x, y]);
							transform.position = new Vector3 (gameMaster.trenchGrid [x, y].transform.position.x, gameMaster.trenchGrid [x, y].transform.position.y, -1);
						}
					}
				}
			}
		}

		if (numOfTimesToRun > 0) {
			--numOfTimesToRun;
			UnitMove (numOfTimesToRun);
		}
	}

	#endregion

	void Update ()
	{
		Vector3 rayForward = transform.TransformDirection (Vector3.forward);
		RaycastHit hit;
		if (Physics.Raycast (transform.position, rayForward, out hit, 5)) {
			if (hit.transform.tag == "DirtTile") {
				gridLocation = hit.transform.gameObject;
				gridLocation.GetComponent<TileScript> ().isOccupied = true;
			}
		}
		if (gridLocation != null) {
			tileDistance = gridLocation.GetComponent<TileScript> ().distance;
			tileX = gridLocation.GetComponent<TileScript> ().x;
			tileY = gridLocation.GetComponent<TileScript> ().y;
		}

		#region Switch state machine
		switch (unitState) {
		case State.Idle:
                // DO NOTHING
			isUnitMoving = false;
			break;
		case State.Moving:
			GoToActionLocation (destination);
			isUnitMoving = true;
			if (transform.position.x == destination.transform.position.x && transform.position.y == destination.transform.position.y) {
				unitState = State.Idle;
			}
			break;
		case State.GoingToFight:
			GoToActionLocation (destination);
			if (transform.position == destination.transform.position) {
				/*
                    if (there is an enemy unit at the destination)
                    {
                        unitState = State.Fighting;
                    }
                    */
			}
			break;
		case State.GoingToDig:
			GoToActionLocation (destination);
			if (transform.position == destination.transform.position) {
				/*
                    if (if there is a digable tile at destination, dig)
                    {
                        unitState = State.Digging
                    }
                    */
			}
			break;
		case State.GoingToListen:
			GoToActionLocation (destination);
			if (transform.position == destination.transform.position) {
				unitState = State.Listening;
			}
			break;
		case State.Digging:
                /*
                if (blockHealth > 0)
                {
                    hit it
                }
                else
                {
                    unitState = State.Idle;
                }
                */
			break;
		case State.Fighting:
                /*
                if (enemyHealth > 0 && unitHealth > 0)
                {
                    attack enemy
                }
                else if (unitHealth > 0)
                {
                    unitState = State.Idle;
                }
                else
                {
                    die;
                }
                */
			break;
		case State.Listening:
                /*
                if (Listening isn't cancelled)
                {
                    do
                        listening
                }

                else
                {
                    unitState = State.Idle;
                }
                */
			break;
		}
		#endregion
	}
}

/*
    ==== Unit Action States ====
    An action occurs once every 0.5 seconds.

    Moving
        move one tile per action

    Digging (blocks have a durability)
        break 1 durability per action

    Fighting
        Attack once per action

    Listening
        

    === Pseudo Code: movement ===
    
    TODO:   - Note down shortest path to destination in array
            - go to next tile in array

*/
