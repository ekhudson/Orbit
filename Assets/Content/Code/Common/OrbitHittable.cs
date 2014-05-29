using UnityEngine;
using System.Collections;

public class OrbitHittable : MonoBehaviour 
{
	private int mInstanceID = -1;

	protected virtual void Start () 
	{
		mInstanceID = collider.GetInstanceID();
		EventManager.Instance.AddHandler<ProjectileImpactEvent>(ProjectileImpactHandler);
	}

	protected virtual void OnHit(ProjectileImpactEvent evt)
	{

	}

	public void ProjectileImpactHandler(object sender, ProjectileImpactEvent evt)
	{
		foreach(ContactPoint point in evt.CollisionInfo.contacts)
		{
			if (point.otherCollider.GetInstanceID() == mInstanceID)
			{
				OnHit(evt);
			}
		}
	}
}
