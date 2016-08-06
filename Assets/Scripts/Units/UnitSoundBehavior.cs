using UnityEngine;
using System.Collections;

public class UnitSoundBehavior : MonoBehaviour
{
	//Manually rebuild this enum according to the names of the clips in the SFXs array.
	//Note that the naming doesn't need to be perfect, but order matters.
	public enum SFX
	{
		yay,
		wee,
		lol,
		yaah,
		dududuuu,
		ohnoes
	}

	AudioSource unitAS;
	public AudioClip[] SFXs;

	void Awake ()
	{
		unitAS = this.GetComponent<AudioSource> ();
	}

	/// <summary>
	/// Plays the selected SFX.
	/// </summary>
	/// <param name="newSFX">New SF.</param>
	public void PlaySFX (SFX newSFX)
	{
		unitAS.PlayOneShot (SFXs [(int)newSFX]);
	}
}
