using UnityEngine;
using System.Collections;

public class HitEvent : EventBase 
{
	public readonly Vector3 HitPoint;
    public readonly BaseObject Hitter;
	public readonly float HitForce;
	public readonly Vector3 HitVector;
	public readonly Entity Victim = null;

	public HitEvent(Object sender, BaseObject hitter, Entity victim, Vector3 hitPoint, float hitForce, Vector3 hitVector) : base(hitPoint, sender)
	{		
		HitPoint = hitPoint;
        Hitter = hitter;
		HitForce = hitForce;
		HitVector = hitVector;
		Victim = victim;
	}
	
	public HitEvent()
	{		
		HitPoint = Vector3.zero;
		HitForce = 0f;
		HitVector = Vector3.zero;
	}
}
