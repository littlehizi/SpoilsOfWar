using UnityEngine;
using System.Collections;
using UnityEditor;

[RequireComponent (typeof(UnitBehavior))]
public class AIBrainBehavior : MonoBehaviour
{
	//References
	UnitBehavior currentUnit;
	UnitData.UnitType currentType;
	public static GroundBehavior selectedBombsite;
	GroundBehavior[] pathChosen;

	//Internal variables
	public int stubbornness;
	public int pointsToPick = 5;


	public void SetupAI ()
	{
		//return;

		//Initalize
		currentUnit = this.GetComponent<UnitBehavior> ();

		//init
		currentType = currentUnit.unitData.typeOfUnit;

		//Pick a random bombsite on the uper half of the depth, under the player camp.
		do {
			selectedBombsite = GameMasterScript.instance.GMB.currentGrid.tiles [Random.Range (0, GridManagerBehavior.STATIC_WIDTH - 1), Random.Range (GridManagerBehavior.STATIC_HEIGHT + 1, GameMasterScript.instance.gridHeight / 2)];
		} while(selectedBombsite == null || selectedBombsite.isAnObstacle);

		//Get base action / goal
		switch (currentType) {
		case UnitData.UnitType.Digger:
			//Diggers find a path towards the common bombsite.

			//Pick 5 points on the path to bomb in a snake-like path
			DigSelectionManagerBehavior.GetGroundTilesToDig (currentUnit.currentTile);

			bool isVertical = true;

			Vector2 chosenTile = currentUnit.currentTile.tilePos;

			for (int i = 0; i < pointsToPick - 1; i++) {


				if (isVertical)
					chosenTile = new Vector2 (chosenTile.x, 
						Mathf.Clamp (selectedBombsite.tilePos.y + Random.Range (-GameMasterScript.instance.gridHeight / 5, GameMasterScript.instance.gridHeight / 5), GridManagerBehavior.STATIC_HEIGHT, GameMasterScript.instance.gridHeight - 1));
				else
					chosenTile = new Vector2 (Mathf.Clamp (chosenTile.x + Mathf.Abs (selectedBombsite.tilePos.x - currentUnit.currentTile.tilePos.x) / pointsToPick + Random.Range (-2, 2), 0, GameMasterScript.instance.gridWidth - 1), 
						chosenTile.y);

				Debug.Log ("chosen tile " + chosenTile);

				GameMasterScript.instance.GMB.TileposToTile (chosenTile).tileSR.color = Color.green;

				DigSelectionManagerBehavior.GetGroundTilesToDig (GameMasterScript.instance.GMB.TileposToTile (chosenTile));
				isVertical = !isVertical;
			}

			DigSelectionManagerBehavior.GetGroundTilesToDig (selectedBombsite);

			pathChosen = DigSelectionManagerBehavior.OutputTilesToDig (currentUnit.currentTile);

			//for (int i = 0; i < pathChosen.Length; i++)
			//	pathChosen [i].tileSR.color = Color.green;

			selectedBombsite.tileSR.color = Color.magenta;

			//DIG !
			currentUnit.StartDiggingProcess (pathChosen);

			break;
		case UnitData.UnitType.Fighter:
			break;
		case UnitData.UnitType.Listener:
			break;
		}

	}

	void Update ()
	{
		
	}
}
