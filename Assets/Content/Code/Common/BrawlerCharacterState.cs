using UnityEngine;
using System.Collections;

[System.Serializable]
public class BrawlerCharacterState 
{
	public string StateName = "New State";
	public BrawlerAnimationClip[] AnimationClips;

    private int mCurrentAnimClip = 0;

    public BrawlerAnimationClip CurrentAnimationClip
    {
        get
        {
            return AnimationClips[mCurrentAnimClip];
        }
    }

    public void ChooseRandomClip()
    {
        mCurrentAnimClip = Random.Range(0, AnimationClips.Length);
    }
}
