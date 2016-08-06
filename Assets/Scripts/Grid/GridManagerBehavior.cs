using UnityEngine;
using System.Collections;

public class GridManagerBehavior : MonoBehaviour, IManager
{
	//Storage Variables
	public Grid currentGrid;

	//Prefabs
	public GameObject[] tilePrefab;
	public GameObject[] envTilePrefab;
	public LayerSize[] tileLayerSize;
	public BaseGroundData[] groundData;
	public BaseGroundData[] envData;


	//Class responsible for easy storage of values. To be replaced by editor scripting
	[System.Serializable]
	public class LayerSize
	{
		public string name;
		public int percentage;
	}

	//LAWS
	public const int ID_X_DIGGIT = 10000;
	public const int STATIC_HEIGHT = 4;
	public const int STATIC_WIDTH = 4;

	//Internal Variables
	[Range (0, 100)]
	public int percentOfObstacles;
	public Sprite obstacleSprite;
	public Sprite fortifiedSprite;

	//START METHOD
	public void OnGameStart ()
	{
		//Generate the terrain
		CreateGrid ();
	}

	/// <summary>
	/// Creates the grid based on the height and width.
	/// </summary>
	public void CreateGrid ()
	{
		GameObject tileStorage = new GameObject ("Grid");

		//Get the constant background (HQs and etc)
		HQ_Player playerHQ = new HQ_Player ();

		currentGrid = new Grid (GameMasterScript.instance.gridWidth, GameMasterScript.instance.gridHeight);
		Vector3 tmpPos = Vector3.zero;

		// Player and enemy indexs
		int humanIndex = 0;
		int enemyIndex = 0;

		for (int i = 0; i < GameMasterScript.instance.gridHeight; i++) {
			for (int k = 0; k < GameMasterScript.instance.gridWidth; k++) {
				tmpPos.x = k;
				tmpPos.y = -i;


				GroundBehavior tmpTile = null;

				if (i < STATIC_HEIGHT) {
					GroundBehavior.EnvGroundType currentTile = GroundBehavior.EnvGroundType.grass;

					if (k < STATIC_WIDTH) {
						//Player HQ
						currentTile = playerHQ.tileData [i, k];

                        
					} else if (k >= GameMasterScript.instance.gridWidth - STATIC_WIDTH) {
						//ENEMY HQ
						currentTile = playerHQ.tileData [i, GameMasterScript.instance.gridWidth - k - 1];
					} else {
						//ENV
						if (i >= 2)
							currentTile = GroundBehavior.EnvGroundType.sky;
						else
							currentTile = GroundBehavior.EnvGroundType.grass;
					}

					//Get the tile
					tmpTile = ((GameObject)Instantiate (envTilePrefab [(int)currentTile], tmpPos, Quaternion.identity)).AddComponent<GroundBehavior> ();
					tmpTile.groundData = envData [(int)currentTile];
					currentGrid.tiles [k, i] = tmpTile.GetComponent<GroundBehavior> ();

					if (currentTile == GroundBehavior.EnvGroundType.buildingBG && k < STATIC_WIDTH) {
						GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [humanIndex++] = tmpTile;
					} else if (currentTile == GroundBehavior.EnvGroundType.buildingBG && k >= GameMasterScript.instance.gridWidth - STATIC_WIDTH) {
						GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [enemyIndex++] = tmpTile;
					}
						
					tmpTile.name = currentTile.ToString () + " " + k.ToString () + "," + i.ToString ();

				} else {	

					//Get the type of tile
					GroundBehavior.GroundType currentTile = GetNewTile (i);

					//Create the tile, give it a ground component and its appropriate data file
					tmpTile = ((GameObject)Instantiate (tilePrefab [(int)currentTile], tmpPos, Quaternion.identity)).AddComponent<GroundBehavior> ();
					tmpTile.groundData = groundData [(int)currentTile];
					currentGrid.tiles [k, i] = tmpTile.GetComponent<GroundBehavior> ();

					tmpTile.name = currentTile.ToString () + " " + k.ToString () + "," + i.ToString ();
				}



				//Give ID based on position
				tmpTile.ID = i * ID_X_DIGGIT + k;

				//Set the position
				tmpTile.tilePos.x = k;
				tmpTile.tilePos.y = i;

				//Make the hierarchy clean
				tmpTile.transform.SetParent (tileStorage.transform);

				//Finally, initialize the ground
				tmpTile.GetComponent<GroundBehavior> ().SetupGround ();
			}
		} 

		AddObstacles ();
	}

	public void AddObstacles ()
	{
		//Generate obstacles based on perlin noise
		float rdmX = Random.Range (-10000, 10000) + 0.01f;
		float rdmY = Random.Range (-10000, 10000) + 0.01f;


		foreach (GroundBehavior tmpTile in currentGrid.tiles) {
			float perlinNoise = Mathf.PerlinNoise (rdmX + tmpTile.trueTilePos.x / 10, rdmY + tmpTile.trueTilePos.y / 10);
			if (tmpTile.tilePos.y > STATIC_WIDTH + 1) {
				if (perlinNoise == Mathf.Infinity)
					continue;
				
				if (perlinNoise > 1 - ((float)percentOfObstacles) / 100.0f) {
					//The tile is now an obstacle !
					tmpTile.isAnObstacle = true;
				}
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

	/// <summary>
	/// Initalizes the spawn tiles by setting them as Dug.
	/// This method will be called by the PlayerManagerBehavior during its initalization.
	/// </summary>
	public void InitalizeSpawnTiles ()
	{
		//human
		for (int i = 0; i < GameMasterScript.instance.PLMB.humanPlayer.spawnTiles.Length; i++)
			GameMasterScript.instance.PLMB.humanPlayer.spawnTiles [i].isDug = true;

		//enemy
		for (int i = 0; i < GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles.Length; i++)
			GameMasterScript.instance.PLMB.enemyPlayer.spawnTiles [i].isDug = true;
	}

	/// <summary>
	/// Returns the storage index X and Y values from a given ID.
	/// </summary>
	/// <returns>The identifier to index.</returns>
	/// <param name="newTile">New tile.</param>
	public Vector2 TileIDToIndex (GroundBehavior newTile)
	{
		return new Vector2 (newTile.ID % ID_X_DIGGIT, (newTile.ID - (newTile.ID % ID_X_DIGGIT)) / ID_X_DIGGIT);
	}
}
