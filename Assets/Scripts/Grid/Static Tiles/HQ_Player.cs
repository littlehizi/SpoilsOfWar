using UnityEngine;
using System.Collections;

public class HQ_Player
{
	public GroundBehavior.EnvGroundType[,] tileData;
	// MAP //
	// 4 x 4
	// 0 0 0 0
	// 2 1 1 0
	// 3 1 1 0
	// 0 1 1 0

	public HQ_Player ()
	{
		tileData = new GroundBehavior.EnvGroundType[4, 4] { {
				GroundBehavior.EnvGroundType.building,
				GroundBehavior.EnvGroundType.building,
				GroundBehavior.EnvGroundType.building,
				GroundBehavior.EnvGroundType.building
			}, {
				GroundBehavior.EnvGroundType.grass,
				GroundBehavior.EnvGroundType.buildingBG,
				GroundBehavior.EnvGroundType.buildingBG,
				GroundBehavior.EnvGroundType.building
			}, {
				GroundBehavior.EnvGroundType.sky,
				GroundBehavior.EnvGroundType.buildingBG,
				GroundBehavior.EnvGroundType.buildingBG,
				GroundBehavior.EnvGroundType.building
			}, {
				GroundBehavior.EnvGroundType.building,
				GroundBehavior.EnvGroundType.buildingBG,
				GroundBehavior.EnvGroundType.buildingBG,
				GroundBehavior.EnvGroundType.building
			},
		};
	}
}
