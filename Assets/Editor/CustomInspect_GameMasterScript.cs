using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof(GameMasterScript))]
public class CustomInspect_GameMasterScript : Editor
{

	public override void OnInspectorGUI ()
	{
		GameMasterScript GMS = (GameMasterScript)target;
		DrawDefaultInspector ();
	}
}
