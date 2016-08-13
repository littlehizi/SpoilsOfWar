using UnityEngine;
using System.Collections;

public abstract class BaseState : MonoBehaviour
{
	public enum State
	{
		mainMenu,
		game,
		endGame,
		tutorial
	}

	protected BaseState ()
	{
		//Debug.Log ("current state: " + this.ToString ());
	}

	public State stateID;

	public abstract void OnStateEnter ();

	public abstract void OnStateExit ();
}
