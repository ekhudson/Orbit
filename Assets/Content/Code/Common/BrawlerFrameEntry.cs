using UnityEngine;
using System.Collections;

[System.Serializable]
public class BrawlerFrameEntry
{
	public int SpriteIndex = 0;
	public BrawlerHitboxSettings AttackBoxSettings = null;
	public BrawlerHitboxSettings HeadBoxSettings = null;
	public BrawlerHitboxSettings BodyBoxSettings = null;
	public BrawlerHitboxSettings LegBoxSettings = null;
	public BrawlerHitboxSettings CollisionBoxSettings = null;
}
