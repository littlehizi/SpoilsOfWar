using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Ground Data")]
public class BaseGroundData : ScriptableObject
{
	//HP REPRESENTS THE LIFE OF EACH TILE. ITS DEFAULT VALUE IS 100. WHEN IT REACHES 0, THE TILE IS DUG.
	public int hp;
	//DIGRES REPRESENS THE TILE RESISTANCE TO DIGGING. IT WILL INFLUENCE HOW HARD IT IS TO DIG A TILE
	public float digRes;
	//DEFAULT SPRITE OF EACH TILE
	public Sprite sprite;
	//MOVEMENT COST OF MOVING OVER TILES. IT WILL AFFECT THE PATHFINDING.
	public int moveCost;
}
