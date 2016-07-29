using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu (menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    
	public enum TypeOfPlayer
	{
		human,
		enemy}

	;

	TypeOfPlayer typeOfPlayer;

	public List<UnitBehavior> storedUnits;

	public GroundBehavior[] spawnTiles;

	public int resources;

}
