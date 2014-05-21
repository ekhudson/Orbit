using UnityEngine;
using System.Collections;

public class BrawlerAttackBox : BrawlerHitbox 
{
	private float mAttackStrength = 1f;

	private const float kMinAttackStrength = 1f; //do we need a max attack strength?

	public float AttackStrength
	{
		get
		{
			return mAttackStrength;
		}
		set
		{
			mAttackStrength = value;
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		if (HitboxType != HitboxTypes.Attack) //TODO: Somehow make it so attack boxes are the only ones who do collision handling [See above]
		{
			return;
		}        
		
		int victimID = 0;
		
		victimID = other.gameObject.GetInstanceID();	
		
		if (!EntityManager.EntityDictionary.ContainsKey(victimID))
		{
			Debug.Log(string.Format("Hit something called {0}, but entity id {1} could not be found in the Entity Manager", other.gameObject.name, other.gameObject.GetInstanceID()));
			return;
		}
		
		Entity victim = EntityManager.EntityDictionary[victimID];
		
		if (victim != null)
		{
			Debug.Log(mAttackStrength);
			EventManager.Instance.Post(new HitEvent(this, ParentEntity, victim, transform.position, Mathf.Clamp(mAttackStrength, kMinAttackStrength, Mathf.Infinity), transform.right));
		}
	}
}
