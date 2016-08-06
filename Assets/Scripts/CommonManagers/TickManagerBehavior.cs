using UnityEngine;
using System.Collections;

public delegate void OnNewTick ();

public class TickManagerBehavior : MonoBehaviour, IManager
{
	//Event
	public event OnNewTick OnNewTickE;

	//Tick encapsulation
	private static int _timeTick;

	public static int timeTick{ get { return _timeTick; } }

	//Internal variables
	public bool canTick = false;
	float timerBetweenTicks;

	public void OnGameStart ()
	{
		timerBetweenTicks = ResetTick ();
		canTick = true;
	}

	void FixedUpdate ()
	{
		if (!canTick)
			return;

		if (timerBetweenTicks > 0)
			timerBetweenTicks -= Time.deltaTime;
		else {
			_timeTick++;
			timerBetweenTicks = ResetTick ();
			if (OnNewTickE != null)
				OnNewTickE ();
		}
	}

	float ResetTick ()
	{
		return GameMasterScript.instance.delayBetweenTicks;
	}
}
