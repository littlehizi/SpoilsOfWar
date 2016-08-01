using UnityEngine;
using System.Collections;

public class ResourceManagerBehavior : MonoBehaviour, IManager
{
	//References
	PlayerManagerBehavior PLMB { get { return GameMasterScript.instance.PLMB; } }

	//Internal variables
	public bool canGetResources;
	public int resourceGainPerBatch;
	public float resourceDelay;
	public int resourcesCap;

	public void OnGameStart ()
	{
		StartCoroutine (UpdateResources ());
	}

	public void SetBaseResources ()
	{
		PLMB.humanPlayer.resources = GameMasterScript.instance.startResources;
		PLMB.enemyPlayer.resources = GameMasterScript.instance.startResources;
	}

	/// <summary>
	/// Adds resources to the current pools, both of them.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void AddResources (int amount)
	{
		if (PLMB.humanPlayer.resources < resourcesCap)
			PLMB.humanPlayer.resources += amount;

		if (PLMB.enemyPlayer.resources < resourcesCap)
			PLMB.enemyPlayer.resources += amount;
	}

	public void StopGettingResources ()
	{
		StopAllCoroutines ();
	}


	IEnumerator UpdateResources ()
	{
		while (true) {
			if (!canGetResources)
				yield return null;
			
			yield return new WaitForSeconds (resourceDelay);

			if (PLMB.humanPlayer.resources < resourcesCap)
				PLMB.humanPlayer.resources += resourceGainPerBatch;

			if (PLMB.enemyPlayer.resources < resourcesCap)
				PLMB.enemyPlayer.resources += resourceGainPerBatch;

			GameMasterScript.instance.UIMB.UpdateResourcesUI ();
		}
	}
}
