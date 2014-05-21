using UnityEngine;
using System.Collections;

[System.Serializable]
public class BrawlerHitboxSettings
{
	public bool Active = false;

	[SerializeField]private Rect mPosition;

	private const float kRectMinWidth = 24f;

	public Rect Position
	{
		get
		{
			if (mPosition.width < kRectMinWidth)
			{
				mPosition.width = kRectMinWidth;
			}

			if (mPosition.height < kRectMinWidth)
			{
				mPosition.height = kRectMinWidth;
			}

			return mPosition;
		}
		set
		{
			mPosition = value;

			if (mPosition.width < kRectMinWidth)
			{
				mPosition.width = kRectMinWidth;
			}
			
			if (mPosition.height < kRectMinWidth)
			{
				mPosition.height = kRectMinWidth;
			}
		}
	}

	public void Translate(Vector2 delta)
	{
		mPosition.center += delta;
	}
}
