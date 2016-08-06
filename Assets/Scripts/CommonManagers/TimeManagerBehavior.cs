using UnityEngine;
using System.Collections;

public class TimeManagerBehavior : MonoBehaviour, IManager
{
	public bool isTimeRunning = false;

	public float debugTimeScale;

	public float currentTime;

	public void OnGameStart ()
	{
		isTimeRunning = true;	
	}

	/// <summary>
	/// NOTES: 1 day = 2 minutes = 1 big handle cycle = 120 seconds.
	/// 
	/// </summary>
	void FixedUpdate ()
	{
		if (!isTimeRunning)
			return;

		//Display time !
		currentTime += Time.fixedDeltaTime * debugTimeScale;

		//Minutes / seconds are calculated normally.
		int days = Mathf.FloorToInt (currentTime / 120);
		float hour = (currentTime / 60) * 24.0f;
		float minute = hour * 60.0f;

		//Debug.Log (days + " " + hour + " " + minute);

		GameMasterScript.instance.UIMB.G_UpdateTime (hour, minute, GameMasterScript.instance.daysInARound - days);

		//Check for time over
		if (GameMasterScript.instance.daysInARound - days <= -1) {
			//GAME OVER, TIME OUT
			isTimeRunning = false;

			((State_Game)GameMasterScript.currentState).GameOver (PlayerData.TypeOfPlayer.enemy);
		}

	}


}
