using UnityEngine;
using System.Collections;

// This class is used to highlight selected units.
public class UnitHighlightScript : MonoBehaviour
{
    public GUISkin gameSkin;
    public UnitScript unitScript;

    private Color startColour;

    void OnGUI()
    {
        GUI.skin = gameSkin;
    }

    void Start()
    {
        unitScript = GetComponent<UnitScript>();
        startColour = GetComponent<Renderer>().material.color;
    }

    void Update()
    {
        if (unitScript.isSelected)
        {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        if (!unitScript.isSelected)
        {
            GetComponent<Renderer>().material.color = startColour;
        }
    }
}
