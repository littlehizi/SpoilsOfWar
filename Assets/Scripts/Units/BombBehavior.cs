using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombBehavior : MonoBehaviour
{
	UnitBehavior unitBehavior;

	int amountOfBombsPlaced = 0;
	public float maxDelayBetweenBombs;
	public int maxAmountOfBombs;
	public LayerMask groundTileLayer;

	void Start ()
	{
		unitBehavior = this.GetComponent<UnitBehavior> ();
	}

	/// <summary>
	/// Starts the process of bombing the current tile, or adds bombs to the current tile.
	/// </summary>
	public bool BombCurrentTile ()
	{
		if (unitBehavior.isOnEnemyTrench) {
			if (amountOfBombsPlaced <= maxAmountOfBombs) {
				amountOfBombsPlaced++;
				return true;
			}
		} else {
			if (amountOfBombsPlaced > 0)
				return false;
			amountOfBombsPlaced++;
			return true;
		}
			
		return false;
	}

	public bool DetonnateBomb ()
	{
		if (amountOfBombsPlaced == 0)
			return false;

		//KABOOM
		Collider2D[] overlapResult = Physics2D.OverlapCircleAll (unitBehavior.currentTile.trueTilePos, (float)GameMasterScript.instance.bombExplosionRadius, groundTileLayer);

		List<GroundBehavior> tmpExplosionTileList = new List<GroundBehavior> ();
		List<GroundBehavior> orderedExplosionTileList = new List<GroundBehavior> ();

		for (int i = 0; i < overlapResult.Length; i++) {
			tmpExplosionTileList.Add (overlapResult [i].transform.GetComponent<GroundBehavior> ());
		}

		//Remove current tile
		if (tmpExplosionTileList.Contains (unitBehavior.currentTile))
			tmpExplosionTileList.Remove (unitBehavior.currentTile);

		//GIVE ORDER !
		while (tmpExplosionTileList.Count > 0) {
			float shortestDistance = Vector2.Distance (unitBehavior.currentTile.tilePos, tmpExplosionTileList [0].tilePos);
			int shortestIndex = 0;

			for (int i = 0; i < tmpExplosionTileList.Count; i++) {
				if (Vector2.Distance (unitBehavior.currentTile.tilePos, tmpExplosionTileList [i].tilePos) < shortestDistance) {
					shortestDistance = Vector2.Distance (unitBehavior.currentTile.tilePos, tmpExplosionTileList [i].tilePos);
					shortestIndex = i;
				}
			}

			orderedExplosionTileList.Add (tmpExplosionTileList [shortestIndex]);
			tmpExplosionTileList.Remove (tmpExplosionTileList [shortestIndex]);
		}

		//Go boom!
		for (int i = 0; i < orderedExplosionTileList.Count; i++) {
			orderedExplosionTileList [i].hp -= (int)(GameMasterScript.instance.damagePerBomb * amountOfBombsPlaced / (orderedExplosionTileList [i].digRes / 10 * i));
			Debug.Log ((int)(GameMasterScript.instance.damagePerBomb * amountOfBombsPlaced / (orderedExplosionTileList [i].digRes / 10 * i)));
		}

		//Reset bombs
		amountOfBombsPlaced = 0;

		Debug.Log ("a bomb has been detonated");

		return true;
	}

	IEnumerator Kaboom ()
	{
		yield return null;
	}

}
