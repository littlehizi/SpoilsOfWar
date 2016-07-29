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
				//Check if there's any enemy unit on next tile
				if (tilesToDig [currentIndex].unitsOnTile.Count > 0 && tilesToDig [currentIndex].unitsOnTile [0].alignment != unitBehavior.alignment) {
					//Stop walking and engage combat
					unitBehavior.EngageCombat (tilesToDig [currentIndex]);
					StopDigging ();
				}

				yield return new WaitForSeconds (tilesToDig [currentIndex].digRes * (unitBehavior.isExhausted ? UnitBehavior.exhaustionMalus : 1) / unitBehavior.unitData.diggingPower);


				tilesToDig [currentIndex].hp -= unitBehavior.unitData.diggingPower * 4;
				Debug.Log ("Diggin tile... Hp left: " + tilesToDig [currentIndex].hp);

			}

			//If the upcoming tile has been dug, walk to it instead
			if (tilesToDig [currentIndex].isDug) {
				//Wait according to the speed stat. 
				yield return new WaitForSeconds (GameMasterScript.instance.baseUnitSpeed * (unitBehavior.isExhausted ? UnitBehavior.exhaustionMalus : 1) / (float)unitBehavior.unitData.speed);
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
