using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Data/Ground Data")]
public class BaseGroundData : ScriptableObject
{
	//HP REPRESENTS THE LIFE OF EACH TILE. ITS DEFAULT VALUE IS 100. WHEN IT REACHES 0, THE TILE IS DUG.
	public int hp;
	//DIGRES REPRESENS THE TILE RESISTANCE TO DIGGING. IT WILL INFLUENCE HOW HARD IT IS TO DIG A TILE
	public float digRes;
	//DEFAULT SPRITE OF EACH TILE
	public Sprite sprite;
	public Sprite[] destructionSprites;
	//MOVEMENT COST OF MOVING OVER TILES. IT WILL AFFECT THE PATHFINDING.
	public int moveCost;
	//WHEN THIS HITS ZERO, THE TILE WILL COLLAPSE
	public int collapseHP;
	public bool canCollapse;
}
