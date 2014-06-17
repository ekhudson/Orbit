using UnityEngine;
using System.Collections;

//Defines whether an object can be affected by gravity or not

public class OrbitObject : Entity 
{
	public float Mass = 1f;
	public bool AffectedByGravity = false;

	private Vector3 mCurrentGravityPull = Vector3.zero;

	public Vector3 GravityPull
	{
		get
		{
			return mCurrentGravityPull;
		}
		set
		{
			mCurrentGravityPull = value;
		}
	}

	protected override void Start()
	{
		base.Start();
	}

	public override void CalledUpdate()
	{
		if (AffectedByGravity && mCurrentGravityPull != Vector3.zero)
		{
			mTransform.position += mCurrentGravityPull * Time.deltaTime;
			mCurrentGravityPull = Vector3.zero;
		}
	}

	public override void OnSpawn()
	{
		OrbitObjectManager.Instance.RegisterOrbitObject(this);
	}
}
