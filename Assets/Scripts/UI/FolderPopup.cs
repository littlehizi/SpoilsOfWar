using UnityEngine;
using System.Collections;

public class FolderPopup : MonoBehaviour
{
	//Internal variables
	Vector3 startPos;
	public float distanceUp;
	public float timeToDistance;

	void OnEnable ()
	{
		//Backup
		startPos = this.transform.position;
		iTween.PunchPosition (this.gameObject, Vector3.up * distanceUp, timeToDistance);
	}

	void OnDisable ()
	{
		this.transform.position = startPos;
		iTween.Stop (this.gameObject);
	}
}
