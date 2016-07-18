using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Data/Unit Data")]
public class UnitData : ScriptableObject
{
	public enum UnitType
	{
		Fighter,
		Listener,
		Digger
	}

	public UnitType typeOfUnit;
	public Sprite sprite;
	public Color tmpColor;
	public int health;
	public int stamina;
	public int strength;
	public int diggingPower;
	public int vision;
	public int speed;
}
