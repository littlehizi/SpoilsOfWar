using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VictoryManagerBehavior : MonoBehaviour, IManager
{
	public class BombSite
	{
		public GroundBehavior tile;
		int amountOfBombs;
		public List<UnitBehavior> owners;

		public int currentAmountOfBombs{ get { return amountOfBombs; } }

		public bool canDetonnate{ get { return amountOfBombs > tile.tilePos.y / 2; } }

		public int amountOfBombsNeeded{ get { return Mathf.RoundToInt (tile.tilePos.y / 2) - amountOfBombs; } }

		public BombSite (GroundBehavior newTile, UnitBehavior newOwner)
		{
			tile = newTile;
			amountOfBombs = 1;
			owners = new List<UnitBehavior> ();
			owners.Add (newOwner);
		}

		/// <summary>
		/// Adds a bomb to the stack of bombs.
		/// If there's enough bombs, this method will return true, and the player can detonnate it.
		/// </summary>
		/// <returns><c>true</c>, if bomb was added, <c>false</c> otherwise.</returns>
		public bool AddBomb ()
		{
			amountOfBombs++;

			return canDetonnate;
		}

		public void AddOwner (UnitBehavior newOwner)
		{
			owners.Add (newOwner);
		}

		/// <summary>
		/// Detonnates the bomb at the bombsite.
		/// This method triggers endgame.
		/// </summary>
		public void DetonnateBomb ()
		{
			((State_Game)GameMasterScript.currentState).GameOver (owners [0].alignment);
		}
	}

	public List<BombSite> bombSites;

	public void OnGameStart ()
	{
		//Find all units and add VictoryCheck to their event
		bombSites = new List<BombSite> ();
	}
		
}
