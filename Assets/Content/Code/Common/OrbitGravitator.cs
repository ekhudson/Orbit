using UnityEngine;
using System.Collections;

public class OrbitGravitator : OrbitObject 
{
	public float GravityPullAmount = 1f;
	public float OrbitPullAmount = 1f;

	public override void OnSpawn()
	{
		OrbitObjectManager.Instance.RegistorGravityObject(this);
	}
}
