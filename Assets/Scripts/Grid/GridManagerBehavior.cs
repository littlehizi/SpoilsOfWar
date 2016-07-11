using UnityEngine;
using System.Collections;

public class GridManagerBehavior : MonoBehaviour, IManager
{
	//Storage Variables
	public Grid currentGrid;

	//Prefabs
	public GameObject[] tilePrefab;
	public LayerSize[] tileLayerSize;
	public BaseGroundData[] groundData;


	//Class responsible for easy storage of values. To be replaced by editor scripting
	[System.Serializable]
	public class LayerSize
	{
		public string name;
		public int percentage;
	}

	//LAWS
	public static int ID_X_DIGGIT = 10000;

	//START METHOD
	public void OnGameStart ()
	{
	}

	/// <summary>
	/// Creates the grid based on the height and width.
	/// </summary>
	public void CreateGrid ()
	{
		GameObject tileStorage = new GameObject ("Grid");

		currentGrid = new Grid (GameMasterScript.instance.gridWidth, GameMasterScript.instance.gridHeight);
		Vector3 tmpPos = Vector3.zero;

		for (int i = 0; i < GameMasterScript.instance.gridHeight; i++) {
			for (int k = 0; k < GameMasterScript.instance.gridWidth; k++) {
				tmpPos.x = k;
				tmpPos.y = -i;

				//Get the type of tile
				GroundBehavior.GroundType currentTile = GetNewTile (i);

				//Create the tile, give it a ground component and its appropriate data file
				GroundBehavior tmpTile = ((GameObject)Instantiate (tilePrefab [(int)currentTile], tmpPos, Quaternion.identity)).AddComponent<GroundBehavior> ();
				tmpTile.groundData = groundData [(int)currentTile];
				currentGrid.tiles [k, i] = tmpTile.GetComponent<GroundBehavior> ();

				//Give ID based on position
				tmpTile.ID = i * ID_X_DIGGIT + k;

				//Set the position
				tmpTile.tilePos.x = k;
				tmpTile.tilePos.y = i;

				//Make the hierarchy clean
				tmpTile.transform.SetParent (tileStorage.transform);
				tmpTile.name = currentTile.ToString () + " " + k.ToString () + "," + i.ToString ();

				//Finally, initialize the ground
				tmpTile.GetComponent<GroundBehavior> ().SetupGround ();
			}
		} 
	}

	/// <summary>
	/// This method returns a type of tile based on its height.
	/// </summary>
	/// <returns>The new tile.</returns>
	/// <param name="tileHeight">Tile height.</param>
	GroundBehavior.GroundType GetNewTile (int tileHeight)
	{
		//IF IT IS THE FINAL ROW, USE BEDROCK NO MATTER WAT
		if (tileHeight == GameMasterScript.instance.gridHeight - 1)
			return GroundBehavior.GroundType.bedRock;

		//First, get the tile height as a percentage
		float heightPer = ((float)tileHeight) / ((float)GameMasterScript.instance.gridHeight) * 100;

		//Return a type of tile according to the percentages entered in inspector
		for (int i = 0; i < tileLayerSize.Length; i++) {
			int sizeSum = 0;

			for (int k = i; k >= 0; k--)
				sizeSum += tileLayerSize [k].percentage;

			if (heightPer < sizeSum)
				return (GroundBehavior.GroundType)i;
		}

		//Use bedrock as a filler
		return GroundBehavior.GroundType.bedRock;
	}
}
