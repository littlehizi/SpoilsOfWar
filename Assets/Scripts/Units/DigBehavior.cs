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

			bool justDugCurrentTile = false;

			//First,  break the tile
			while (tilesToDig [currentIndex].hp > 0) {
				//Check if there's any enemy unit on next tile
				if (tilesToDig [currentIndex].unitsOnTile.Count > 0 && tilesToDig [currentIndex].unitsOnTile [0].alignment != unitBehavior.alignment) {
					//Stop walking and engage combat
					unitBehavior.EngageCombat (tilesToDig [currentIndex]);
					StopDigging ();
				}

				yield return new WaitForSeconds (tilesToDig [currentIndex].digRes * ((1 - unitBehavior.stamina / unitBehavior.unitData.stamina) * GameMasterScript.instance.exhaustEfficiencyModifier) / unitBehavior.unitData.diggingPower);


				tilesToDig [currentIndex].hp -= unitBehavior.unitData.diggingPower * 4;
				Debug.Log ("Diggin tile... Hp left: " + tilesToDig [currentIndex].hp);
				justDugCurrentTile = true;
			}

			//If the upcoming tile has been dug, walk to it instead
			if (!justDugCurrentTile && tilesToDig [currentIndex].isDug) {
				//Wait according to the speed stat. 
				yield return new WaitForSeconds (GameMasterScript.instance.baseUnitSpeed * (unitBehavior.isExhausted ? GameMasterScript.instance.exhaustEfficiencyModifier : 1) / (float)unitBehavior.unitData.speed);
			}

			//Move the unit to the next tile once it's broken
			Vector3 tmpPos = tilesToDig [currentIndex].tilePos;
			tmpPos.y *= -1;
			tmpPos.z = -2;
			this.transform.position = tmpPos;

			//Set current tile to new tile
			unitBehavior.currentTile = tilesToDig [currentIndex];

			//Remove some stamina
			unitBehavior.stamina -= GameMasterScript.instance.staminaCostDig;

			//Lit the current tile with zero vision
			tilesToDig [currentIndex].gameObject.AddComponent<TileVisionBehavior> ().tileVision = -1; //Make this = -1 if you don't want gradient 

			//Fortify tile
			if (tilesToDig [currentIndex].isFortified == false && unitBehavior.UseResource (GameMasterScript.instance.resourceUsedPerFortification)) {
				tilesToDig [currentIndex].isFortified = true;
			}

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
