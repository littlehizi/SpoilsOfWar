using UnityEngine;
using System.Collections;

/// <summary>
/// Audio manager behavior.
/// This class is responsible for handling the audio sources for both SFX and BGM
/// This manager will not be initlialized during StartGame, since it can be used on Mainmenu
/// </summary>
public class AudioManagerBehavior : MonoBehaviour, IManager
{
	//References to sources
	public AudioSource sourceBGM;
	public AudioSource sourceSFX;

	public AudioClip[] BGMs;
	public AudioClip[] SFXs;

	public enum TypeOfBGM
	{
		thisBGM,
		thatBGM,
		etc
	}

	public enum TypeOfSFX
	{
		kaboom,
		screamOfAgony,
		popCorn,
		pizza
		//THE SOURCE OF PIZZA !!!
	}

	void Awake ()
	{
		//Initialization
	}

	public void OnGameStart ()
	{
		//LEAVE BILLY ALONE
	}

	public void PlayNewBGM (TypeOfBGM newBGM)
	{
		sourceBGM.Stop ();

		sourceBGM.clip = BGMs [(int)newBGM];

		sourceBGM.Play ();
	}

	public void PlayNewSFX (TypeOfSFX newSFX)
	{
		sourceSFX.PlayOneShot (SFXs [(int)newSFX]);
	}
}
