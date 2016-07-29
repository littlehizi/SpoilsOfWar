using UnityEngine;
using System.Collections;

public class BombBehavior : MonoBehaviour
{
	int amountOfBombsPlaced = 0;
	public float maxDelayBetweenBombs;
	public int maxAmountOfBombs;

	/// <summary>
	/// Starts the process of bombing the current tile, or adds bombs to the current tile.
	/// </summary>
	public void BombCurrentTile ()
	{
		//if there's already enough bombs placed, don't destroy the world. Give it a break.
		if (amountOfBombsPlaced >= maxAmountOfBombs)
			return;
		
		//Check if it's the beginning of the process
		if (amountOfBombsPlaced == 0) {
			amountOfBombsPlaced = 1;
			StartCoroutine (BombingProcess ());
		} else {
			amountOfBombsPlaced++;
		}
	}

	IEnumerator BombingProcess ()
	{
		float timer = maxDelayBetweenBombs;
		int memAmountOfBombs = amountOfBombsPlaced;

		while (timer > 0) {
			//if a bomb has been added, reset the timer
			if (memAmountOfBombs != amountOfBombsPlaced)
				timer = maxDelayBetweenBombs;

			timer += Time.deltaTime;

			yield return new WaitForFixedUpdate ();
		}

		//KABOOM !
	}
}
