using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCamera : BaseObject
{   
	public float DefaultFieldOfView = 60f;
    public Vector3 OffsetFromTarget = new Vector3(0,0,-30);
	public float TrackingSpeed = 1f;

	private Vector3[] mPlayerViewportPositions;
	private Camera mCamera;
	private Vector3 mCameraFrameVector = Vector3.zero;

	protected override void Start()
	{
		base.Start();
		mCamera = camera;
	}

	void FixedUpdate ()
    {
		if (mCamera == null)
		{
			return;
		}

		Vector3 newPosition = Vector3.zero;

		if (BrawlerPlayerManager.Instance.CurrentActivePlayers == 0)
		{
			return;
		}

		mPlayerViewportPositions = new Vector3[BrawlerPlayerManager.Instance.PlayerList.Count];

		int count = 0;

		foreach(BrawlerPlayerComponent player in BrawlerPlayerManager.Instance.PlayerList)
		{
			if (player.IsActivePlayer)
			{
				newPosition += player.transform.position;
			}

			mPlayerViewportPositions[count] = mCamera.WorldToViewportPoint(player.transform.position);
		}

		if (BrawlerPlayerManager.Instance.CurrentActivePlayers > 2)
		{
			Bounds boundRect = new Bounds(new Vector3(mCamera.transform.position.x, mCamera.transform.position.y, 0f), Vector3.zero);

			foreach(Vector3 point in mPlayerViewportPositions)
			{
				boundRect.Encapsulate(point);
			}

			//mCameraFrameVector.z = (boundRect.max - boundRect.min).magnitude;

			//mCamera.orthographicSize = (boundRect.min - boundRect.max).magnitude;
			//Debug.Log((boundRect.max - boundRect.min).magnitude);
		}

		newPosition /= BrawlerPlayerManager.Instance.CurrentActivePlayers;

		mTransform.position = Vector3.Slerp(mTransform.position, (newPosition + OffsetFromTarget), TrackingSpeed * Time.deltaTime); 	     	   
	}
}
