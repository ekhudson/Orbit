using UnityEngine;
using System.Collections;

public class OrbitGravitator : OrbitObject 
{
	public float GravityPullAmount = 1f;
	public static float MaxGravity = 6f;

	public override void OnSpawn()
	{
		OrbitObjectManager.Instance.RegisterGravityObject(this);
	}
}
