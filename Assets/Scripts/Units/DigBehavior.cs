using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnitBehavior))]
public class DigBehavior : MonoBehaviour
{
	//Reference

	UnitBehavior unitBehavior;

	void Awake ()
	{
		unitBehavior = this.GetComponent<UnitBehavior> ();
	}

	public void StartDigging (GroundBehavior[] tilesToDig)
	{
		//Stop any coroutines
		StopAllCoroutines ();

		Debug.Log ("Digging begun!");

		//Start it !
		StartCoroutine ("DigUntilDestination", tilesToDig);
	}

	IEnumerator DigUntilDestination (GroundBehavior[] tilesToDig)
	{
		int currentIndex = 0;

		while (currentIndex < tilesToDig.Length) {

			//First,  break the tile
			while (tilesToDig [currentIndex].hp > 0) {
				yield return new WaitForSeconds (tilesToDig [currentIndex].digRes / unitBehavior.unitData.diggingPower);

				tilesToDig [currentIndex].hp -= unitBehavior.unitData.diggingPower * 4;
				Debug.Log ("Diggin tile... Hp left: " + tilesToDig [currentIndex].hp);
			}

			//Set tile as dug
			tilesToDig [currentIndex].isDug = true;

			//Move the unit to the next tile once it's broken
			Vector3 tmpPos = tilesToDig [currentIndex].tilePos;
			tmpPos.y *= -1;
			tmpPos.z = -2;
			this.transform.position = tmpPos;

			//Set current tile to new tile
			unitBehavior.currentTile = tilesToDig [currentIndex];

			//Remove some stamina
			unitBehavior.stamina -= GameMasterScript.instance.staminaCostDig;

			//Update index
			currentIndex++;
		}

		Debug.Log ("Diggin finished !");
	}

	public void StopDigging ()
	{
		StopAllCoroutines ();
	}
}
