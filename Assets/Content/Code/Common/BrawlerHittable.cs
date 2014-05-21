using UnityEngine;
using System.Collections;

public class BrawlerHittable : Entity
{
	public bool FlashWhenHit = true;
	private bool mHasCollider = false;

	private Color mCurrentFlashColor = Color.white;
	private float mCurrentFlashTime = 0f;
	private const float kTotalFlashTime = 0.5f;

	protected override void Start()
	{
		base.Start();
		EventManager.Instance.AddHandler<HitEvent>(HitEventHandler);
	}

	public void HitEventHandler(object sender, HitEvent hitEvent)
	{
		if (hitEvent.Victim != this)
		{
			return;
		}

		OnHit(hitEvent);
	}

	protected virtual void OnHit(HitEvent hitEvent)
	{
		Debug.Log (gameObject.name + " hit" + hitEvent.Hitter.name);
		//override to do stuff on hit
	}

	private void OnDestroy()
	{
		EventManager.Instance.RemoveHandler<HitEvent>(HitEventHandler);
	}

	protected IEnumerator Flash()
	{
		mCurrentFlashTime = 0f;
		Color originalColor = mRenderer.material.color;

		while(mCurrentFlashTime < kTotalFlashTime)
		{
			mRenderer.material.color = Color.Lerp(originalColor, mCurrentFlashColor, 1 - (mCurrentFlashTime / kTotalFlashTime));
			yield return new WaitForSeconds(Time.deltaTime);
			mCurrentFlashTime += Time.deltaTime;
		}
	}

}
