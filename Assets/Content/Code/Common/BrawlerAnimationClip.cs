using UnityEngine;
using System.Collections;

[System.Serializable]
public class BrawlerAnimationClip : MonoBehaviour
{
	public Sprite[] Sprites;
	public BrawlerFrameEntry[] Frames;
	public LoopModes LoopMode;
	public int StartingFrame;
	public float FramesPerSecond = 30f;

	private bool mIsPlaying = false;
	private bool mIsPaused = false;
	private float mCurrentFrameTime = 0f;
	private float mFrameStartTime = 0f;
	private float mFrameTime = -1f;
	private float mLastTimeCheck = 0f;
	private int mCurrentFrame = 0;
	private int mCurrentPlayDirection = 1; //1 == forward, -1 == backward;

	public int CurrentFrame
	{
		get
		{
			return mCurrentFrame;
		}
	}

	public Sprite CurrentSprite
	{
		get
		{
            mCurrentFrame = Mathf.Clamp(mCurrentFrame, 0, Frames.Length);

            if (Frames[mCurrentFrame].SpriteIndex > Sprites.Length - 1)
            {
                Frames[mCurrentFrame].SpriteIndex = Sprites.Length - 1;
            }

			return Sprites[Frames[mCurrentFrame].SpriteIndex];
		}
	}

	public BrawlerFrameEntry CurrentFrameEntry
	{
		get
		{
			return Frames[mCurrentFrame];
		}
	}

	public bool IsPlaying
	{
		get
		{
			return mIsPlaying;
		}
	}

	public enum LoopModes
	{
		NONE,
		WRAP,
		PINGPONG,
	}

	public void Play()
	{
		mFrameStartTime = 0f;

		if (mIsPaused) 
		{
			mIsPaused = false;
		} 
		else 
		{
			mCurrentFrame = StartingFrame;
		}

		mIsPlaying = true;
		mLastTimeCheck = Time.realtimeSinceStartup;

		if (Application.isPlaying)
		{
			AnimationManager.Instance.ActiveClips.Add (this);
		}
	}

	public void Stop()
	{
		mIsPlaying = false;
		mFrameTime = -1;

		if (Application.isPlaying)
		{
			AnimationManager.Instance.ActiveClips.Remove (this);
		}
	}

	public void Pause()
	{
		mIsPaused = true;

		if (Application.isPlaying)
		{
			AnimationManager.Instance.ActiveClips.Remove (this);
		}
	}

	public void Tick(float realTime)
	{
		if (mFrameStartTime == 0) 
		{
			mFrameStartTime = realTime;
		}

		if (mFrameTime == -1) 
		{
			RecalculateFrameTime();
		}

		mCurrentFrameTime = realTime - mFrameStartTime;

		if (mCurrentFrameTime > mFrameTime) 
		{
			if (mCurrentPlayDirection == 1 && mCurrentFrame == Frames.Length - 1)
			{
				if (LoopMode == LoopModes.NONE)
				{
					Stop ();
					return;
				}
				else if (LoopMode == LoopModes.WRAP)
				{
					mCurrentFrame = 0;
				}
				else if (LoopMode == LoopModes.PINGPONG)
				{
					mCurrentPlayDirection = mCurrentPlayDirection * -1;
				}
			}
			else if (mCurrentPlayDirection == -1 && mCurrentFrame == 0)
			{
				if (LoopMode == LoopModes.NONE)
				{
					Stop ();
					return;
				}
				else if (LoopMode == LoopModes.WRAP)
				{
					mCurrentFrame = Frames.Length - 1;
				}
				else if (LoopMode == LoopModes.PINGPONG)
				{
					mCurrentPlayDirection = mCurrentPlayDirection * -1;
				}
			}
			else
			{
				mCurrentFrame += mCurrentPlayDirection;
			}

			mFrameStartTime = 0f;
			mCurrentFrameTime = 0f;
		}
	}

	public void RecalculateFrameTime()
	{
		mFrameTime = (1f / FramesPerSecond);
	}
}













