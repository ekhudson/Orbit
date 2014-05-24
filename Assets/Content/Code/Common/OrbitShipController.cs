using UnityEngine;
using System.Collections;

[RequireComponent(typeof(OrbitPlayerComponent), typeof(OrbitObject))]
public class OrbitShipController : MonoBehaviour 
{
	public float MaxThrustAmount = 10f;
	public float ThrustPerSecond = 1f;
	public float Drag = 0.8f; //0 to 1;
	public float RotationMaxDegPerSecond = 1;

	private Vector3 mCurrentThrust = Vector3.zero;
	private Vector3 mMouseWorldPosition = Vector3.zero;
	private bool mUsingGamepad = false;
	private Quaternion mTargetLookRotation = Quaternion.identity;
	private Vector3 mTargetLookVector = Vector3.zero;
	private float mRotationMagnitude = 0f;

	private const float kStopThreshold = 0.05f;

	//Component Refs
	private OrbitPlayerComponent mPlayerComponent;
	private OrbitObject mOrbitObject;
	private Transform mTransform;

	public Quaternion CurrentTargetLookRotation
	{
		get
		{
			return mTargetLookRotation;
		}
	}

	public Vector3 CurrentTargetLookVector
	{
		get
		{
			return mTargetLookVector;
		}
	}

	private void Start () 
	{
		mPlayerComponent = GetComponent<OrbitPlayerComponent>();
		mOrbitObject = GetComponent<OrbitObject>();
		mTransform = transform;
		EventManager.Instance.AddHandler<UserInputKeyEvent>(InputHandler);
	}	

	private void Update()
	{
		mTransform.position += mCurrentThrust;

		ApplyDrag();


		if (!mUsingGamepad) 
		{
			MouseLook ();
		}
		else
		{
			RotateShip();
		}
	}

	private void MouseLook()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = 100f;

		mMouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
		mMouseWorldPosition.y = mTransform.position.y;

		mTransform.LookAt(mMouseWorldPosition);
	}

	private void RotateShip()
	{
		mTransform.rotation = Quaternion.RotateTowards (mTransform.rotation, mTargetLookRotation, (RotationMaxDegPerSecond * Time.deltaTime) * mRotationMagnitude);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(mMouseWorldPosition, 5f);
	}

	private void ApplyDrag()
	{
		mCurrentThrust *= Drag;

		if (Mathf.Abs(mCurrentThrust.x) <= kStopThreshold)
		{
			mCurrentThrust.x = 0f;
		}

		if (Mathf.Abs(mCurrentThrust.z) <= kStopThreshold)
		{
			mCurrentThrust.z = 0f;
		}
	}

	public void InputHandler(object sender, UserInputKeyEvent evt)
	{
		if (!gameObject.activeSelf)
		{
			return;
		}

		if(evt.PlayerIndexInt != mPlayerComponent.PlayerID - 1 && evt.PlayerIndexInt != -1)
		{
			return;
		}

#region Keyboard Input

		if(!OrbitUserInput.Instance.IsGamePadActive(mPlayerComponent.AssociatedGamepad))
		{
			mUsingGamepad = false;

			if(evt.KeyBind == OrbitUserInput.Instance.MoveRight && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + (ThrustPerSecond * Time.deltaTime), -MaxThrustAmount, MaxThrustAmount);
			}

			if(evt.KeyBind == OrbitUserInput.Instance.MoveLeft && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + ((ThrustPerSecond * Time.deltaTime) * -1), -MaxThrustAmount, MaxThrustAmount);
			}

			if(evt.KeyBind == OrbitUserInput.Instance.MoveUp && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + (ThrustPerSecond * Time.deltaTime), -MaxThrustAmount, MaxThrustAmount);
			}
			
			if(evt.KeyBind == OrbitUserInput.Instance.MoveDown && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + ((ThrustPerSecond * Time.deltaTime) * -1), -MaxThrustAmount, MaxThrustAmount);
			}
		}
		else
		{
			mUsingGamepad = true;
		}

#endregion

#region Gamepad Input

		if (evt.KeyBind == OrbitUserInput.Instance.MoveCharacter) 
		{
			mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + (evt.JoystickInfo.AmountX * (ThrustPerSecond * Time.deltaTime)), -MaxThrustAmount, MaxThrustAmount);
			mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + (evt.JoystickInfo.AmountY * (ThrustPerSecond * Time.deltaTime)), -MaxThrustAmount, MaxThrustAmount);
		}

		if (evt.KeyBind == OrbitUserInput.Instance.Look) 
		{
			if (evt.JoystickInfo.AmountX == 0 && evt.JoystickInfo.AmountY == 0)
			{
				mTargetLookRotation = mTransform.rotation;
				mTargetLookVector = mTransform.forward;
				mRotationMagnitude = 0f;
			}

			Vector3 joystickVector = new Vector3(evt.JoystickInfo.AmountX, 0f, evt.JoystickInfo.AmountY);

			Vector3 lookVector = (mTransform.position + joystickVector) - mTransform.position;

			mRotationMagnitude = joystickVector.magnitude;
			mTargetLookVector = lookVector;

			if (lookVector == Vector3.zero)
			{
				mTargetLookVector = mTransform.forward;
				return;
			}

			mTargetLookRotation = Quaternion.LookRotation( lookVector, Vector3.up);
		}

#endregion
	}

}
