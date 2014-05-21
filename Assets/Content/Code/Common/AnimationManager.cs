using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationManager : Singleton<AnimationManager> 
{
	public float AnimationTicksPerSecond = 60f;
	public List<BrawlerAnimationClip> ActiveClips = new List<BrawlerAnimationClip>(); //TODO: Make this private

	private float mTickWaitTime = 1f;

	private void Start()
	{
		mTickWaitTime = 1 / AnimationTicksPerSecond;
		StartCoroutine ( AnimationTicker() );
	}

	IEnumerator AnimationTicker()
	{
		while (true) 
		{
			for(int i = 0; i < ActiveClips.Count; i++)
			{
				if (ActiveClips[i] == null)
				{
					continue;
				}

				ActiveClips[i].Tick(Time.realtimeSinceStartup);
			}

			yield return new WaitForSeconds(mTickWaitTime);
		}
	}
}
