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
	public int preferedDiggerDepth = 4;

	//Debug
	public static int YAY;

	public void SetupAI ()
	{
		//return;

		//Initalize
		currentUnit = this.GetComponent<UnitBehavior> ();

		//init
		currentType = currentUnit.unitData.typeOfUnit;

		//Pick a random bombsite on the uper half of the depth, under the player camp.
		do {
			selectedBombsite = GameMasterScript.instance.GMB.currentGrid.tiles [Random.Range (0, GridManagerBehavior.STATIC_WIDTH - 1), Random.Range (GridManagerBehavior.STATIC_HEIGHT + 3, GameMasterScript.instance.gridHeight / 2)];
		} while(selectedBombsite == null || selectedBombsite.isAnObstacle);

		//Get base action / goal
		switch (currentType) {
		case UnitData.UnitType.Digger:
			//Diggers find a path towards the common bombsite.
			YAY++;

			//Pick 5 points on the path to bomb in a snake-like path
			DigSelectionManagerBehavior.GetGroundTilesToDig (currentUnit.currentTile);

			bool isVertical = true;

			Vector2 pre_chosenTile = Vector2.zero;
			Vector2 chosenTile = currentUnit.currentTile.tilePos;

			for (int i = 0; i < pointsToPick; i++) {

				do {
					//Backup
					if (!GameMasterScript.instance.GMB.TileposToTile (chosenTile).isAnObstacle)
						pre_chosenTile = chosenTile;
					//else
					//	pointsToPick++;

					if (isVertical)
						chosenTile = new Vector2 (pre_chosenTile.x, 
							Mathf.Clamp (selectedBombsite.tilePos.y - Random.Range (-GameMasterScript.instance.gridHeight / preferedDiggerDepth, GameMasterScript.instance.gridHeight / 10), GridManagerBehavior.STATIC_HEIGHT + 2, GameMasterScript.instance.gridHeight - 1));
					else
						chosenTile = new Vector2 (Mathf.Clamp (Mathf.Abs (selectedBombsite.tilePos.x - currentUnit.currentTile.tilePos.x) / (i) + Random.Range (-2, 2), 0, GameMasterScript.instance.gridWidth - 1), 
							pre_chosenTile.y);
					
					isVertical = !isVertical;
				} while(GameMasterScript.instance.GMB.TileposToTile (chosenTile).isAnObstacle);
					

				Debug.Log ("chosen tile " + chosenTile + " going vertical? : " + !isVertical);

				GameMasterScript.instance.GMB.TileposToTile (chosenTile).tileSR.color = Color.green;

				DigSelectionManagerBehavior.GetGroundTilesToDig (GameMasterScript.instance.GMB.TileposToTile (chosenTile), true);
			}

			DigSelectionManagerBehavior.GetGroundTilesToDig (selectedBombsite, true);

			pathChosen = DigSelectionManagerBehavior.OutputTilesToDig (currentUnit.currentTile);

			if (pathChosen != null)
				for (int i = 0; i < pathChosen.Length; i++)
					pathChosen [i].tileSR.color = (YAY == 1 ? Color.green : Color.red);
			else
				Debug.Log ("NULL PATH!");

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
