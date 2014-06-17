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
			if (!mOrbitObjects[i].AffectedByGravity)
			{
				continue;
			}

			mOrbitObjects[i].GravityPull = Vector3.zero;

			Vector3 directionToGravitator = Vector3.zero;

			foreach(OrbitGravitator gravitator in mGravitators)
			{
				if (mOrbitObjects[i] == null || mOrbitObjects[i].BaseTransform == null)
				{
					mOrbitObjects.RemoveAt(i);
					return;
				}

				Vector3 objectPos = mOrbitObjects[i].BaseTransform.position;
				Vector3 gravitatorPos = gravitator.BaseTransform.position;

				objectPos.y = 0f;
				gravitatorPos.y = 0f;

				directionToGravitator = (gravitatorPos - objectPos).normalized;

				if (mOrbitObjects[i].AffectedByGravity)
				{
					mOrbitObjects[i].GravityPull += directionToGravitator * ((gravitator.GravityPullAmount * Mathf.Clamp((gravitator.GravityPullAmount / mOrbitObjects[i].Mass), 0.00001f, 1f)));  //gravitator.GravityPullAmount; //TODO: Higher grav at lower orbits
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

	public void RegisterGravityObject(OrbitGravitator gravitator)
	{
		if(!mGravitators.Contains(gravitator))
		{
			mGravitators.Add(gravitator);
		}
	}

}
