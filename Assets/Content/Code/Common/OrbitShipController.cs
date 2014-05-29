﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(OrbitPlayerComponent), typeof(OrbitObject))]
public class OrbitShipController : OrbitHittable 
{
	private Vector3 mCurrentThrust = Vector3.zero;
	private Vector3 mMouseWorldPosition = Vector3.zero;
	private bool mUsingGamepad = false;
	private Quaternion mTargetLookRotation = Quaternion.identity;
	private Vector3 mTargetLookVector = Vector3.zero;
	private float mRotationMagnitude = 0f;

	private const float kStopThreshold = 0.05f;

	//Component Refs
	private OrbitPlayerComponent mPlayerComponent;
	private OrbitShipAttributes mShipAttributes;
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

	protected override void Start () 
	{
		base.Start ();

		mPlayerComponent = GetComponent<OrbitPlayerComponent>();
		mOrbitObject = GetComponent<OrbitObject>();
		mShipAttributes = GetComponent<OrbitShipAttributes>();
		mTransform = transform;
		EventManager.Instance.AddHandler<UserInputKeyEvent>(InputHandler);

		if (mShipAttributes != null && mShipAttributes.PrimaryWeapon != null)
		{
			mShipAttributes.PrimaryWeapon.Start();
		}
		
		if (mShipAttributes != null && mShipAttributes.SecondaryWeapon != null)
		{
			mShipAttributes.SecondaryWeapon.Start();
		}
	}	

	private void Update()
	{
		if (mShipAttributes == null)
		{
			return;
		}

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

		if (mShipAttributes != null && mShipAttributes.PrimaryWeapon != null)
		{
			mShipAttributes.PrimaryWeapon.Update();
		}

		if (mShipAttributes != null && mShipAttributes.SecondaryWeapon != null)
		{
			mShipAttributes.SecondaryWeapon.Update();
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
		mTransform.rotation = Quaternion.RotateTowards (mTransform.rotation, mTargetLookRotation, (mShipAttributes.RotationDegreesPerSecond * Time.deltaTime) * mRotationMagnitude);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawSphere(mMouseWorldPosition, 5f);
	}

	private void ApplyDrag()
	{
		mCurrentThrust *= mShipAttributes.DragAmount;

		if (Mathf.Abs(mCurrentThrust.x) <= kStopThreshold)
		{
			mCurrentThrust.x = 0f;
		}

		if (Mathf.Abs(mCurrentThrust.z) <= kStopThreshold)
		{
			mCurrentThrust.z = 0f;
		}
	}

	protected override void OnHit(ProjectileImpactEvent evt)
	{
		mShipAttributes.Health -= evt.Projectile.Damage;

		if (mShipAttributes.Health == 0) 
		{
			OnDeath();
		}
	}

	protected override void OnDeath()
	{

	}

	public void InputHandler(object sender, UserInputKeyEvent evt)
	{
		if (mShipAttributes == null)
		{
			return;
		}

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
				mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + (mShipAttributes.ThrustPerSecond * Time.deltaTime), -mShipAttributes.ThrustMax, mShipAttributes.ThrustMax);
			}

			if(evt.KeyBind == OrbitUserInput.Instance.MoveLeft && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + ((mShipAttributes.ThrustPerSecond * Time.deltaTime) * -1), -mShipAttributes.ThrustMax, mShipAttributes.ThrustMax);
			}

			if(evt.KeyBind == OrbitUserInput.Instance.MoveUp && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + (mShipAttributes.ThrustPerSecond * Time.deltaTime), -mShipAttributes.ThrustMax, mShipAttributes.ThrustMax);
			}
			
			if(evt.KeyBind == OrbitUserInput.Instance.MoveDown && (evt.Type == UserInputKeyEvent.TYPE.KEYDOWN || evt.Type == UserInputKeyEvent.TYPE.KEYHELD))
			{
				mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + ((mShipAttributes.ThrustPerSecond * Time.deltaTime) * -1), -mShipAttributes.ThrustMax, mShipAttributes.ThrustMax);
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
			mCurrentThrust.x = Mathf.Clamp(mCurrentThrust.x + (evt.JoystickInfo.AmountX * (mShipAttributes.ThrustPerSecond * Time.deltaTime)), -mShipAttributes.ThrustMax, mShipAttributes.ThrustMax);
			mCurrentThrust.z = Mathf.Clamp(mCurrentThrust.z + (evt.JoystickInfo.AmountY * (mShipAttributes.ThrustPerSecond * Time.deltaTime)), -mShipAttributes.ThrustMax, mShipAttributes.ThrustMax);
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

		if (evt.KeyBind == OrbitUserInput.Instance.PrimaryWeapon && (evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_HELD))
		{
			if (mShipAttributes.PrimaryWeapon.WeaponState == OrbitWeapon.WeaponStates.READY)
			{
				mShipAttributes.PrimaryWeapon.FireWeapon(mShipAttributes.TurretDefinitions[mShipAttributes.PrimaryWeapon.TurretIndex].GetCurrentMuzzlesForFiring(), gameObject.GetInstanceID(), mPlayerComponent.PlayerColor);
			}
		}

		if (evt.KeyBind == OrbitUserInput.Instance.SecondaryWeapon && (evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_HELD))
		{
			if (mShipAttributes.SecondaryWeapon.WeaponState == OrbitWeapon.WeaponStates.READY)
			{
				mShipAttributes.SecondaryWeapon.FireWeapon(mShipAttributes.TurretDefinitions[mShipAttributes.SecondaryWeapon.TurretIndex].GetCurrentMuzzlesForFiring(), gameObject.GetInstanceID(), mPlayerComponent.PlayerColor);
			}
		}

#endregion
	}

}
