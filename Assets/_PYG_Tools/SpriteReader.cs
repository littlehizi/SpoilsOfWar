using UnityEngine;
using System.Collections;

public class SpriteReader : MonoBehaviour
{
	public Sprite[] sprites;
	public float timeBetweenFrames;

	IEnumerator Start ()
	{
		SpriteRenderer SR = this.GetComponent<SpriteRenderer> ();

		int currentIndex = 0;

		while (true) {
			SR.sprite = sprites [currentIndex++];
			if (currentIndex >= sprites.Length)
				currentIndex = 0;

			yield return new WaitForSeconds (timeBetweenFrames);
		}
	}
}
