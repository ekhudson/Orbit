using UnityEngine;
using System.Collections;

public class OrbitUICamera : Singleton<OrbitUICamera> 
{
	private static Camera mCamera;

	public Camera UICamera
	{
		get
		{

			if (mCamera == null)
			{
				mCamera = camera;
			}

			return mCamera;
		}
	}
}
