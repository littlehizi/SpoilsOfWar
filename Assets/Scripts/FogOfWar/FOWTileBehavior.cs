using UnityEngine;
using System.Collections;

public class FOWTileBehavior : MonoBehaviour
{
	//Sprite renderer reference
	[HideInInspector]public SpriteRenderer tileSR;

	private FogOfWarManagerBehavior.VisibleState _currentState = FogOfWarManagerBehavior.VisibleState.hidden;

	public FogOfWarManagerBehavior.VisibleState currentState {
		get{ return _currentState; } 
		set { 
			if (value != _currentState) {
				StopAllCoroutines ();
				_currentState = value;
				StartCoroutine (LerpColorTo (_currentState));
			}
		}
	}

	void OnEnable ()
	{
		_currentState = FogOfWarManagerBehavior.VisibleState.hidden;

		if (tileSR == null)
			tileSR = this.GetComponent<SpriteRenderer> ();
	}

	/// <summary>
	/// Lerps the color from a color to another (colors stored on FOWMB)
	/// </summary>
	/// <returns>The color to.</returns>
	/// <param name="toState">To state.</param>
	IEnumerator LerpColorTo (FogOfWarManagerBehavior.VisibleState toState)
	{
		//Cancel flickering 
		yield return new WaitForSeconds (0.05f);
		if (toState == currentState)
			yield return null;

		while (!Mathf.Approximately (tileSR.color.a, GameMasterScript.instance.FOWMB.colorStates [(int)toState].a)) {
			tileSR.color = Color.Lerp (tileSR.color, GameMasterScript.instance.FOWMB.colorStates [(int)toState], 3 * Time.deltaTime);

			yield return null;
		}
	}
}
