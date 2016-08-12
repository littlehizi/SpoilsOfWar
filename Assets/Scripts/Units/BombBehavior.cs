using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombBehavior : MonoBehaviour
{
	/// <summary>
	/// Bomb class.
	/// </summary>
	class Bomb
	{
		public GroundBehavior bombTile;

		public Bomb (GroundBehavior newBombTile)
		{
			bombTile = newBombTile;
		}
	}

	UnitBehavior unitBehavior;

	//int amountOfBombsPlaced = 0;
	Bomb currentBomb;
	public float maxDelayBetweenBombs;
	public int maxAmountOfBombs;
	public LayerMask groundTileLayer;
	public bool canDetonnateABombsite = false;
	bool isExploding;

	void Start ()
	{
		unitBehavior = this.GetComponent<UnitBehavior> ();

		isExploding = false;
	}

	/// <summary>
	/// Starts the process of bombing the current tile, or adds bombs to the current tile.
	/// </summary>
	public bool BombCurrentTile ()
	{
		VictoryManagerBehavior.BombSite bombsiteOnTile = null;

		if (IsUnitOverAllyBombsite (out bombsiteOnTile)) {
			//Late debug nullcheck
			if (bombsiteOnTile == null)
				return false;

			//Add bombs to the bomsite !
			if (bombsiteOnTile.AddBomb ()) {
				Debug.Log ("Bomb ready to be detonnated !");
				canDetonnateABombsite = true;
			} else
				Debug.Log ("A bomb has been planted!. Need " + bombsiteOnTile.amountOfBombsNeeded.ToString () + " more.");

			//Update some UI stuff? 

			return true;
		} else {
			if (currentBomb != null)
				return false;
			Debug.Log ("a bomb has been planted");
			currentBomb = new Bomb (unitBehavior.currentTile);
			return true;
		}
	}


	/// <summary>
	/// Detonnates the bomb.
	/// Will return false if that action cannot be done.
	/// </summary>
	/// <returns><c>true</c>, if bomb was detonnated, <c>false</c> otherwise.</returns>
	public void DetonnateBomb ()
	{
		if (isExploding)
			return;

		StartCoroutine (BombBooming ());

		isExploding = true;
	}

	IEnumerator BombBooming ()
	{

		if (CheckForBombsiteDetonnation ())
			yield return null;

		if (currentBomb == null)
			yield return null;

		yield return new WaitForSeconds (0);


		//KABOOM
		Collider2D[] overlapResult = Physics2D.OverlapCircleAll (currentBomb.bombTile.trueTilePos, (float)GameMasterScript.instance.bombExplosionRadius, groundTileLayer);

		List<GroundBehavior> tmpExplosionTileList = new List<GroundBehavior> ();
		List<GroundBehavior> orderedExplosionTileList = new List<GroundBehavior> ();

		for (int i = 0; i < overlapResult.Length; i++) {
			tmpExplosionTileList.Add (overlapResult [i].transform.GetComponent<GroundBehavior> ());
		}


		//Remove current tile
		if (tmpExplosionTileList.Contains (unitBehavior.currentTile))
			tmpExplosionTileList.Remove (unitBehavior.currentTile);

		Debug.Log (tmpExplosionTileList.Count.ToString () + " tiles affected !");

		//GIVE ORDER !
		while (tmpExplosionTileList.Count > 0) {
			float shortestDistance = Vector2.Distance (currentBomb.bombTile.tilePos, tmpExplosionTileList [0].tilePos);
			int shortestIndex = 0;

			for (int i = 0; i < tmpExplosionTileList.Count; i++) {
				if (Vector2.Distance (currentBomb.bombTile.tilePos, tmpExplosionTileList [i].tilePos) < shortestDistance) {
					shortestDistance = Vector2.Distance (currentBomb.bombTile.tilePos, tmpExplosionTileList [i].tilePos);
					shortestIndex = i;
				}
			}

			orderedExplosionTileList.Add (tmpExplosionTileList [shortestIndex]);
			tmpExplosionTileList.Remove (tmpExplosionTileList [shortestIndex]);
		}

		//Go boom!
		for (int i = 0; i < orderedExplosionTileList.Count; i++) {
			orderedExplosionTileList [i].hp -= (int)(GameMasterScript.instance.damagePerBomb / (orderedExplosionTileList [i].digRes / 10 * i));
			//Debug.Log ((int)(GameMasterScript.instance.damagePerBomb / (orderedExplosionTileList [i].digRes / 10 * i)));

			//Destroy obstacles
			if (orderedExplosionTileList [i].isAnObstacle) {
				orderedExplosionTileList [i].hp = 0;
				orderedExplosionTileList [i].isAnObstacle = false;
			}

			//Kill all units in the explosion range, unless they're too far, then try to kill them
			if (orderedExplosionTileList [i].unitsOnTile.Count > 0) {
				for (int k = 0; k < orderedExplosionTileList [i].unitsOnTile.Count; k++) {
					//If the unit is too closed from the bomb, kill it. If not, remove soem HP.
					if (Vector3.Distance (currentBomb.bombTile.trueTilePos, orderedExplosionTileList [i].trueTilePos) < GameMasterScript.instance.bombExplosionRadius / 2)
						orderedExplosionTileList [i].unitsOnTile [k].OnDeathEnter ();
					else
						orderedExplosionTileList [i].unitsOnTile [k].health -= GameMasterScript.instance.farBombDamage;
				}
			}
		}

		//Kill the unit on the tilebomb
		if (currentBomb.bombTile.unitsOnTile.Count > 0) {
			for (int i = 0; i < currentBomb.bombTile.unitsOnTile.Count; i++)
				currentBomb.bombTile.unitsOnTile [i].OnDeathEnter ();
		}

		//Reset bombs
		currentBomb = null;

		Debug.Log ("a bomb has been detonated");
	}


	/// <summary>
	/// Checks for bombsite detonnation.
	/// Note that this will end the game.
	/// </summary>
	/// <returns><c>true</c>, if for bombsite detonnation was checked, <c>false</c> otherwise.</returns>
	bool CheckForBombsiteDetonnation ()
	{
		if (GameMasterScript.instance.VMB.bombSites.Count == 0)
			return false;

		Debug.Log ("checkin for detonnation...");
		for (int i = 0; i < GameMasterScript.instance.VMB.bombSites.Count; i++) {
			//Check if the current unit is the owner (or partial owner) of the bombsite
			if (GameMasterScript.instance.VMB.bombSites [i].owners.Contains (unitBehavior)) {
				Debug.Log ("Current bomb found, looking if there's enough explosives..");
				//If so, then check if there's enough bombs planted.
				if (GameMasterScript.instance.VMB.bombSites [i].canDetonnate) {
					Debug.Log ("BOOOOOOOOOOOOOOOOOOOOM");
					//If so, win the game ! KABOOM !
					GameMasterScript.instance.VMB.bombSites [i].DetonnateBomb ();
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Checks if the current unit is on an ally bomb site.
	/// </summary>
	/// <returns><c>true</c>, if unit over ally bombsite was ised, <c>false</c> otherwise.</returns>
	bool IsUnitOverAllyBombsite (out VictoryManagerBehavior.BombSite currentBombsite)
	{
		if (unitBehavior.isBelowEnemyHQ ()) {
			for (int i = 0; i < GameMasterScript.instance.VMB.bombSites.Count; i++) {
				if (GameMasterScript.instance.VMB.bombSites [i].tile.ID == unitBehavior.currentTile.ID) {
					currentBombsite = GameMasterScript.instance.VMB.bombSites [i];
					return true;
				}
			}

			//If the unit is not over a bombsite, CREATE ONE ! =D ! Creating stuff is always fun!
			currentBombsite = new VictoryManagerBehavior.BombSite (unitBehavior.currentTile, unitBehavior);
			GameMasterScript.instance.VMB.bombSites.Add (currentBombsite);
			Debug.Log ("A bombsite has been created !");
			return true;
		}

		currentBombsite = null;
		return false;
	}



	//Debug stuff
	void OnDrawGizmos ()
	{
		if (currentBomb != null)
			Gizmos.DrawWireSphere (currentBomb.bombTile.trueTilePos, (float)GameMasterScript.instance.bombExplosionRadius);
	}
}
