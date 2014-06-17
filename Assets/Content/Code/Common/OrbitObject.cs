using UnityEngine;
using System.Collections;

//Defines whether an object can be affected by gravity or not

public class OrbitObject : Entity 
{
	public float Mass = 1f;
	public bool AffectedByGravity = true;
	public bool CalculateButIgnoreGravity = false;

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
		if (AffectedByGravity && mCurrentGravityPull != Vector3.zero && !CalculateButIgnoreGravity)
		{
			mTransform.position += mCurrentGravityPull * Time.deltaTime;
		}
	}

	public override void OnSpawn()
	{
		OrbitObjectManager.Instance.RegisterOrbitObject(this);
	}
}
