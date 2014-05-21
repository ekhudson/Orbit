using UnityEngine;
using System.Collections;

public class BrawlerHitboxController : BaseObject 
{
	public BrawlerHitbox HeadCollider;
	public BrawlerHitbox BodyCollider;
	public BrawlerHitbox LegCollider;
	public BrawlerHitbox CollisionCollider;
	public BrawlerAttackBox AttackCollider;

	protected override void Start()
	{
		base.Start();
	}

	public void ApplySettings(BrawlerHitboxSettings setting, BrawlerHitbox hitbox, Sprite sprite, int orientation)
	{
		BoxCollider boxCollider = hitbox.BaseCollider as BoxCollider;

		if (!setting.Active) 
		{
			hitbox.gameObject.SetActive (false);
			return;
		} 
		else if (!hitbox.gameObject.activeSelf) 
		{
			hitbox.gameObject.SetActive(true);
		}

		//Vector3 spritePosition = mGameObject.transform.parent.position;

        Rect hitboxRect = new Rect(setting.Position);       

		float scaleFactor = (512f / 100f) / 512f;

		boxCollider.center = new Vector3((hitboxRect.x * scaleFactor), (hitboxRect.y * scaleFactor), 0f);

        boxCollider.size = new Vector3( (hitboxRect.width * scaleFactor), (hitboxRect.height * scaleFactor), 1f);
	}
}
