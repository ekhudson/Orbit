using UnityEngine;
using System.Collections;

public class OrbitProjectile : OrbitObject 
{
	private float mSpeed = 0f;
	private Vector3 mThrustMag = Vector3.zero;
	private float mLifetime = 100f;
	private float mCurrentTimeAlive = 0f;


	public void SetupProjectile(float speed, Vector3 direction, float lifetime)
	{
		mSpeed = speed;
		mThrustMag = direction;
		mLifetime = lifetime;
	}

	public void FixedUpdate()
	{
		if (mCurrentTimeAlive > mLifetime)
		{
			Destroy (gameObject);
		}

		mTransform.position += (mThrustMag * (mSpeed * Time.deltaTime));
		mCurrentTimeAlive += Time.deltaTime;
	}
}
