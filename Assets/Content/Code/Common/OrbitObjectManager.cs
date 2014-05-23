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

		foreach(OrbitObject obj in mOrbitObjects)
		{
			if (!obj.AffectedByGravity && !obj.AffectedByOrbitPull)
			{
				continue;
			}

			Vector3 directionToGravitator = Vector3.zero;

			foreach(OrbitGravitator gravitator in mGravitators)
			{
				directionToGravitator = (gravitator.BaseTransform.position - obj.BaseTransform.position).normalized;

				if (obj.AffectedByGravity)
				{
					obj.GravityPull += directionToGravitator * gravitator.GravityPullAmount; //TODO: Higher grav at lower orbits
				}

				if (obj.AffectedByOrbitPull)
				{
					obj.BaseTransform.RotateAround(gravitator.BaseTransform.position, Vector3.up, gravitator.OrbitPullAmount * Time.deltaTime);
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
