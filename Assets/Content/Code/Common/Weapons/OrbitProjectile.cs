using UnityEngine;
using System.Collections;

public class OrbitProjectile : OrbitObject 
{
	public FXDefinition FXDefinitions;
	public ParticleSystem[] ParticleSystemsToColorize;
	public MeshRenderer[] RenderersToColorize;

	private float mSpeed = 0f;
	private Vector3 mThrustMag = Vector3.zero;
	private float mLifetime = 100f;
	private float mCurrentTimeAlive = 0f;
	private int mParentID = -1;
	private int mDamage = 0;

	[System.Serializable]
	public class FXDefinition
	{
		public GameObject[] ImpactFX;
		public GameObject[] MissFX;
		public GameObject[] OnDeathFX;
	}

	public int Damage
	{
		get
		{
			return mDamage;
		}
	}

	public void SetupProjectile(float speed, Vector3 direction, float lifetime, Color tracerColor, int parentID, int damage)
	{
		mSpeed = speed;
		mThrustMag = direction;
		mLifetime = lifetime;
		mParentID = parentID;
		mDamage = damage;

		foreach (ParticleSystem particlesSystem in ParticleSystemsToColorize) 
		{
			particleSystem.startColor = tracerColor;
		}

		foreach (MeshRenderer renderer in RenderersToColorize) 
		{
			renderer.material.color = tracerColor;
		}
	}

	public void FixedUpdate()
	{
		if (mCurrentTimeAlive > mLifetime)
		{
			OnDeath();
			RemoveProjectile();
		}

		mTransform.position += (mThrustMag * (mSpeed * Time.deltaTime));
		mCurrentTimeAlive += Time.deltaTime;
	}

	public virtual void OnDeath()
	{
		if (FXDefinitions.OnDeathFX.Length > 0) 
		{
			GameObject.Instantiate (FXDefinitions.OnDeathFX [Random.Range (0, FXDefinitions.OnDeathFX.Length)], mTransform.position, mTransform.rotation);
		}

		RemoveProjectile();
	}

	public virtual void RemoveProjectile()
	{
		EntityManager.RemoveFromDictionary (mInstanceID);
		Destroy (gameObject);
	}

	public void OnCollisionEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts) 
		{
			if (contact.otherCollider.gameObject.GetInstanceID() == mParentID)
			{
				return;
			}
		}

		EventManager.Instance.Post(new ProjectileImpactEvent(this, -1, collision, this));

		if (FXDefinitions.ImpactFX.Length > 0)
		{
			GameObject.Instantiate (FXDefinitions.ImpactFX [Random.Range (0, FXDefinitions.ImpactFX.Length)], mTransform.position, mTransform.rotation);
		}

		RemoveProjectile();
	}
}
