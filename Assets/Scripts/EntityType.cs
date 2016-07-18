using UnityEngine;
using System.Collections;

public class EntityType : MonoBehaviour
{

	public enum TypeOfEntity
	{
		unit,
		tile,
		item,
		joke
	}

	public TypeOfEntity typeOfEntity;
}
