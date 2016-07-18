using UnityEngine;
using System.Collections;

/// <summary>
/// EVERY UNIT AND OBJECT that can see through the Fog Of War need this class.
/// Tiles are not included
/// </summary>
[RequireComponent (typeof(EntityType))]
public class VisionFOWBehavior : MonoBehaviour
{
	//internal reference for better usage
	FogOfWarManagerBehavior FOWMB;


	void OnEnable ()
	{
		//Add UpdateVision to the vision event on the unit/tile's main behavior
		switch (this.GetComponent<EntityType> ().typeOfEntity) {
		case EntityType.TypeOfEntity.unit:
			UnitBehavior thisUnit = this.GetComponent<UnitBehavior> ();
			//Subscribe to OnTileEnter event
			thisUnit.OnTileEnterEvent += UpdateVision;
			thisUnit.RegisterToIVisionStorage ();
			break;
		}
	}

	void UpdateVision ()
	{
		GameMasterScript.instance.FOWMB.UpdateFOW ();
	}

	void OnDisable ()
	{
		//Desubscribe
		switch (this.GetComponent<EntityType> ().typeOfEntity) {
		case EntityType.TypeOfEntity.unit:
			this.GetComponent<UnitBehavior> ().DeleteIVisionEntry ();
			break;
		}

		//Remove this component
		Destroy (this);
	}
}
