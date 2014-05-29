using UnityEngine;
using System.Collections;

public class ProjectileImpactEvent : EventBase 
{
	public readonly OrbitProjectile Projectile;
	public readonly int OtherID;
	public readonly Collision CollisionInfo;

	public ProjectileImpactEvent(OrbitProjectile projectile, int otherID, Collision collision, object sender) : base(collision.transform.position, sender)
	{
		Projectile = projectile;
		OtherID = otherID;
		CollisionInfo = collision;
	}
}
