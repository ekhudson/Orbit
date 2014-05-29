using UnityEngine;
using System.Collections;

public class OrbitProjectile : OrbitObject 
{
	public FXDefinition FXDefinitions;

	private float mSpeed = 0f;
	private Vector3 mThrustMag = Vector3.zero;
	private float mLifetime = 100f;
	private float mCurrentTimeAlive = 0f;

	[System.Serializable]
	public class FXDefinition
	{
		public GameObject[] ImpactFX;
		public GameObject[] MissFX;
		public GameObject[] OnDeathFX;
	}

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
			OnDeath();
			Destroy (gameObject);

		}

		mTransform.position += (mThrustMag * (mSpeed * Time.deltaTime));
		mCurrentTimeAlive += Time.deltaTime;
	}

	public virtual void OnDeath()
	{
		EntityManager.RemoveFromDictionary (mInstanceID);

		if (FXDefinitions.OnDeathFX.Length > 0) 
		{
			GameObject.Instantiate (FXDefinitions.OnDeathFX [Random.Range (0, FXDefinitions.OnDeathFX.Length)], mTransform.position, mTransform.rotation);
		}
	}
}
