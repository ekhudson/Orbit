using UnityEngine;
using System.Collections;

public class BrawlerPlayerHitboxListener : BrawlerHittable 
{
	public BrawlerPlayerComponent ParentPlayer;

	protected override void Start()
	{
		base.Start();
	}

	protected override void OnHit(HitEvent evt)
	{
		if (ParentPlayer != null)
		{
			ParentPlayer.Hit(evt);
		}
	}
}
