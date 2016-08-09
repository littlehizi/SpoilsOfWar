using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UnitBehavior))]
public class CombatBehavior : MonoBehaviour
{
	//Reference
	UnitBehavior unitBehavior;

	//Internal vars
	public bool isFighting;
	public bool canRunAway;

	public void StartCombat (GroundBehavior opponent, bool isEngaging)
	{
		if (unitBehavior == null)
			unitBehavior = this.GetComponent<UnitBehavior> ();
		
		isFighting = true;
		canRunAway = isEngaging;

		for (int i = 0; i < opponent.unitsOnTile.Count; i++) {
			if (!opponent.unitsOnTile [i].GetComponent<CombatBehavior> ().isFighting)
				opponent.unitsOnTile [i].IsEngagedInCombat (unitBehavior.currentTile);
		}

		StartCoroutine ("Fighting", opponent);
	}

	IEnumerator Fighting (GroundBehavior opponentTile)
	{
		//The unit engaging has a little advance
		if (!canRunAway)
			yield return new WaitForSeconds (0.2f);

		while (isFighting) {
			yield return new WaitForSeconds (GameMasterScript.instance.combatSpeed);

			//Check if there's still some enemies left to attack
			if (opponentTile.unitsOnTile.Count == 0) {
				//Congratz on not dying ! 
				isFighting = false;
				break;
			}

			//Pick a random enemy from the ones available
			UnitBehavior selectedEnemy = opponentTile.unitsOnTile [Random.Range (0, opponentTile.unitsOnTile.Count)];

			Debug.Log (((int)(((float)unitBehavior.unitData.strength) * (0.1f + ((float)unitBehavior.unitData.stamina) / ((float)unitBehavior.stamina)))) + "Damage dealt to " + selectedEnemy.name);

			//Enemy takes damage according to its strength and its percentage of stamina left
			selectedEnemy.health -= (int)(((float)unitBehavior.unitData.strength) * (0.1f + ((float)unitBehavior.unitData.stamina) / ((float)unitBehavior.stamina)));

			unitBehavior.stamina -= GameMasterScript.instance.staminaCostFight;
		}
	}

	public void StopCombat ()
	{
		StopCoroutine ("Fighting");
		CombatEnded ();
	}

	void CombatEnded ()
	{
		//Reset the flags
		isFighting = false;

		if (unitBehavior.health > 0 && unitBehavior.stamina > 0)
			unitBehavior.canBeSelected = true;
	}
}
