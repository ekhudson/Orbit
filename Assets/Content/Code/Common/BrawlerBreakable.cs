using UnityEngine;
using System.Collections;

public class BrawlerBreakable : BrawlerHittable 
{
	//public float Health = 100;
	public Transform DamageParticle;
	public Transform BreakParticle;

	//private float mCurrentHealth = 100;

	private const float kTestDamage = 50f;

	private Color mDamageColor = Color.red;
	private Color mOriginalColor = Color.white;
	private Material mOriginalMat;

	protected override void Start()
	{
		base.Start();
		//mCurrentHealth = Health;
		mDamageColor.a = 0f;
		mOriginalColor = mRenderer.sharedMaterial.color;
		mOriginalMat = mRenderer.material;
	}

	protected override void OnHit(HitEvent hitEvent)
	{
		TakeDamage(hitEvent.HitForce, hitEvent.HitPoint);
	}

	private void TakeDamage(float dmgAmt, Vector3 dmgLocation)
	{
		mCurrentHealth -= (int)dmgAmt;

		mRenderer.material.color = Color.Lerp(mOriginalColor, mDamageColor, 1 - ( (Health - (Health - mCurrentHealth)) / Health));

		if (mCurrentHealth <= 0)
		{
			DestroyBreakable();
		}
		else
		{
			Transform go = (Transform)Instantiate(DamageParticle, dmgLocation, Quaternion.identity);
			go.renderer.material = new Material(mOriginalMat);
			Destroy(go.gameObject, 10f); //HACK: Make a better particle spawning system
		}

		if(FlashWhenHit)
		{
			StartCoroutine(Flash());
		}
	}

	private void DestroyBreakable()
	{
		Transform go = (Transform)Instantiate(BreakParticle, mTransform.position, Quaternion.identity);
		go.renderer.material = new Material(mOriginalMat);
		Destroy (gameObject);
	}
}
