using UnityEngine;
using System.Collections;

public class SetupAndSpawningScript : MonoBehaviour
{
	/*
    // Used to adjust the width (x axis) of the grid
    public int trenchWidth;

    // Different layers, used to adjust the height (y axis) of the grid
    public int mudLayer;
    public int hardDirtLayer;
    public int compactedDirtLayer;
    public int compactedDirtClayLayer;
    public int clayLayer;
    public int hardClayLayer;

    public GameObject[,] trenchGrid;
    public int trenchHeight;

    // Grid tile prefabs
    public GameObject mudPrefab;
    public GameObject hardDirtPrefab;
    public GameObject compactedDirtPrefab;
    public GameObject compactedDirtClayPrefab;
    public GameObject clayPrefab;
    public GameObject hardClayPrefab;

    // Test character
    public GameObject testCharacterPrefab;
    private GameObject testCharacter;

    // Setup for other scripts
    public GameMasterScript gameMaster;

    void Awake()
    {
        trenchHeight = mudLayer + hardDirtLayer + compactedDirtLayer + compactedDirtClayLayer + clayLayer + hardClayLayer;
        trenchGrid = new GameObject[trenchWidth, trenchHeight];
    }

    void Start()
    {
        testCharacter = Instantiate(testCharacterPrefab);
        testCharacter.transform.position = new Vector3(0, trenchHeight-1, -1);
        testCharacter.tag = "Unit";

        // Game Master Setup
        gameMaster.gridWidth = trenchWidth;
        gameMaster.gridHeight = trenchHeight;

        // Grid setup
        for (int y = 0; y < trenchHeight; ++y)
        {
            for (int x = 0; x < trenchWidth; ++x)
            {
                // Decide what kind of block needs to be spawned...
                if (y < trenchHeight - clayLayer - compactedDirtClayLayer - compactedDirtLayer - hardDirtLayer - mudLayer)
                {
                    trenchGrid[x, y] = Instantiate(hardClayPrefab);
                    trenchGrid[x, y].name = "Hard Clay (" + x + ", " + y + ")";
                }
                else if (y < trenchHeight  - compactedDirtClayLayer - compactedDirtLayer - hardDirtLayer - mudLayer)
                {
                    trenchGrid[x, y] = Instantiate(clayPrefab);
                    trenchGrid[x, y].name = "Clay (" + x + ", " + y + ")";
                }
                else if (y < trenchHeight - compactedDirtLayer - hardDirtLayer - mudLayer)
                {
                    trenchGrid[x, y] = Instantiate(compactedDirtClayPrefab);
                    trenchGrid[x, y].name = "Compacted Dirt/Clay (" + x + ", " + y + ")";
                }
                else if (y < trenchHeight - hardDirtLayer - mudLayer)
                {
                    trenchGrid[x, y] = Instantiate(compactedDirtPrefab);
                    trenchGrid[x, y].name = "Compacted Dirt (" + x + ", " + y + ")";
                }
                else if (y < trenchHeight - mudLayer)
                {
                    trenchGrid[x, y] = Instantiate(hardDirtPrefab);
                    trenchGrid[x, y].name = "Hard Dirt (" + x + ", " + y + ")";
                }
                else
                {
                    trenchGrid[x, y] = Instantiate(mudPrefab);
                    trenchGrid[x, y].name = "Mud (" + x + ", " + y + ")";
                }
                // ...then give it a position, tag to be used by units, and setup the other variables
                trenchGrid[x, y].transform.position = new Vector3(x, y, 0);
                trenchGrid[x, y].tag = "DirtTile";
                TileScript tileScript = trenchGrid[x,y].GetComponent<TileScript>();
                tileScript.GameMaster = this.GetComponent<GameMasterScript>();
                tileScript.x = x;
                tileScript.y = y;
            } 
        }
        // Grid to be used by Game Master
        gameMaster.trenchGrid = trenchGrid;

        // Camera setup
        //Camera.main.transform.position = new Vector3(trenchWidth / 2, trenchHeight / 2, -10);
        //Camera.main.orthographicSize = trenchHeight / 2 + 1;
    }
	*/
}
