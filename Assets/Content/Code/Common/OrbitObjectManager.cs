using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrbitObjectManager : Singleton<OrbitObjectManager> 
{
	private List<OrbitObject> mOrbitObjects = new List<OrbitObject>();
	private List<OrbitGravitator> mGravitators = new List<OrbitGravitator>();

	public List<OrbitObject> OrbitObjectList
	{
		get
		{
			return mOrbitObjects;
		}
	}

	public List<OrbitGravitator> OrbitGravitatorList
	{
		get
		{
			return mGravitators;
		}
	}

	private void FixedUpdate()
	{
		if(mOrbitObjects.Count == 0 || mGravitators.Count == 0)
		{
			return;
		}

		for(int i = mOrbitObjects.Count - 1; i >= 0; i--)
		{
			if (!mOrbitObjects[i].AffectedByGravity && !mOrbitObjects[i].AffectedByOrbitPull)
			{
				continue;
			}

			if (mOrbitObjects[i] == null || mOrbitObjects[i].transform == null)
			{
				mOrbitObjects.RemoveAt(i);
			}

			Vector3 directionToGravitator = Vector3.zero;

			foreach(OrbitGravitator gravitator in mGravitators)
			{
				directionToGravitator = (gravitator.BaseTransform.position - mOrbitObjects[i].BaseTransform.position).normalized;

				if (mOrbitObjects[i].AffectedByGravity)
				{
					mOrbitObjects[i].GravityPull += directionToGravitator * gravitator.GravityPullAmount; //TODO: Higher grav at lower orbits
				}

				if (mOrbitObjects[i].AffectedByOrbitPull)
				{
					mOrbitObjects[i].BaseTransform.RotateAround(gravitator.BaseTransform.position, Vector3.up, 
					                                           (gravitator.OrbitPullAmount * Mathf.Clamp((gravitator.OrbitPullAmount / mOrbitObjects[i].Mass), 0.00001f, 1f)) * Time.deltaTime);
				}
			}
		}
	}

	public void RegisterOrbitObject(OrbitObject obj)
	{
		if(!mOrbitObjects.Contains(obj))
		{
			mOrbitObjects.Add(obj);
		}
	}

	public void RegistorGravityObject(OrbitGravitator gravitator)
	{
		if(!mGravitators.Contains(gravitator))
		{
			mGravitators.Add(gravitator);
		}
	}

}
