using UnityEngine;
using System.Collections;

[CreateAssetMenu (menuName = "Data/Unit Data")]
public class UnitData : ScriptableObject
{
	public enum UnitType
	{
		Digger,
		Listener,
		Fighter
	}

	public UnitType typeOfUnit;
	public Sprite sprite;
	public Sprite filePicture;
	public Color tmpColor;
	public Sprite medal;
	public int health;
	public int stamina;
	public int strength;
	public int amountOfBombs;
	public int diggingPower;
	public int vision;
	public int speed;
}
