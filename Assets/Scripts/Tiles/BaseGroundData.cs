using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Ground Data")]
public class BaseGroundData : ScriptableObject
{
	public int hp;
	public float digRes;
	public Sprite sprite;
	public int moveCost;
}
