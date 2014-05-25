using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class OrbitTurretDefinition 
{
	public Transform[] MuzzlePoints;
	public FireTypes FireType = FireTypes.SEQUENTIAL;
	public GameObject WeaponToUse;

	private int mCurrentMuzzleIndex = 0;

	public Transform[] GetCurrentMuzzlesForFiring()
	{
		List<Transform> muzzlePoints = new List<Transform>();

		switch(FireType)
		{
			case FireTypes.SEQUENTIAL:

			muzzlePoints.Add(MuzzlePoints[mCurrentMuzzleIndex]);

			mCurrentMuzzleIndex++;

			if (mCurrentMuzzleIndex >= MuzzlePoints.Length)
			{
				mCurrentMuzzleIndex = 0;
			}

			break;

			case FireTypes.SIMULTANEOUS:

			muzzlePoints = new List<Transform>(muzzlePoints);

			break;

			case FireTypes.RANDOM:

			muzzlePoints.Add(MuzzlePoints[Random.Range (0, MuzzlePoints.Length)]);

			break;
		}

		return muzzlePoints.ToArray();
	}

	public enum FireTypes
	{
		SEQUENTIAL,
		SIMULTANEOUS,
		RANDOM,
	}
}
