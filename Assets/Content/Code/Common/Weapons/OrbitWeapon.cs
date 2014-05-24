using UnityEngine;
using System.Collections;

public class OrbitWeapon : MonoBehaviour 
{
	public string WeaponName = "New Weapon";
	public int DamageAmount = 1;
	public float ProjectileSpeed = 10f;
	public float ProjectileLifetime = 100f;
	public GameObject ProjectilePrefab;
	public bool IsAutomatic = true;
	public int RoundsPerShot = 1;
	public float TimeBetweenShots = 1f;
	public int RoundsPerClip = 50; //-1 for infinite clips
	public int TotalNumberOfClips = 2; //-1 for infinite ammo
	public float ReloadTime = 2f;
	public float ChargeTime = 0f;
	public WeaponTypes WeaponType = WeaponTypes.PRIMARY;

	private float mCurrentCooldownTime = 0f;
	private float mCurrentReloadTime = 0f;
	private float mCurrentChargeTime = 0f;
	private float mTimeInCurrentState = 0f;
	private int mCurrentRoundInClip = 1;
	private int mClipsRemaining = 0;
	private OrbitPlayerComponent mOwnerPlayer;
	private WeaponStates mWeaponState = WeaponStates.READY;
	private Transform[] mMuzzlePoints;
	private int mCurrentMuzzleIndex = 0;
	private OrbitTurretDefinition.FireTypes mFireType = OrbitTurretDefinition.FireTypes.SEQUENTIAL;

	public enum WeaponStates
	{
		READY,
		CHARGING,
		FIRING,
		COOLDOWN,
		RELOAD,
		DISABLED,
	}

	public enum WeaponTypes
	{
		PRIMARY,
		SECONDARY,
	}

	public void SetOwner(OrbitPlayerComponent player)
	{
		mOwnerPlayer = player;
	}

	public void SetMuzzlePoints(Transform[] muzzlePoints)
	{
		mMuzzlePoints = muzzlePoints;
	}

	public void SetFireType(OrbitTurretDefinition.FireTypes fireType)
	{
		mFireType = fireType;
	}

	public WeaponStates WeaponState
	{
		get
		{
			return mWeaponState;
		}
	}

	private void Start()
	{
		mClipsRemaining = TotalNumberOfClips;
		EventManager.Instance.AddHandler<UserInputKeyEvent>(InputHandler);
	}

	private void Update()
	{
		mTimeInCurrentState += Time.deltaTime;

		switch(mWeaponState)
		{
			case WeaponStates.READY:

			break;

			case WeaponStates.CHARGING:

				if(mTimeInCurrentState > ChargeTime)
				{
					SetState(WeaponStates.FIRING);
				}
			
			break;

			case WeaponStates.COOLDOWN:

				if(mTimeInCurrentState > TimeBetweenShots)
				{
					SetState(WeaponStates.READY);
				}

			break;

			case WeaponStates.FIRING:

				FireWeapon();

				if (RoundsPerClip != -1 && mCurrentRoundInClip > RoundsPerClip)
				{					
					SetState(WeaponStates.RELOAD);
				}
				else
				{
					SetState(WeaponStates.COOLDOWN);
				}

			break;

			case WeaponStates.RELOAD:

				if(mTimeInCurrentState > ReloadTime)
				{
					SetState(WeaponStates.READY);
				}
			
			break;

			case WeaponStates.DISABLED:
			
			break;
		}
	}

	private void SetState(WeaponStates newState)
	{
		if (mWeaponState == newState)
		{
			return;
		}

		switch(mWeaponState)
		{
			case WeaponStates.READY:

				if (newState == WeaponStates.FIRING)
				{
					if (mClipsRemaining == 0)
					{
						return;
					}
				}
			
			break;
			
			case WeaponStates.CHARGING:
			
			break;
			
			case WeaponStates.COOLDOWN:
			
			break;
			
			case WeaponStates.FIRING:

				if (newState == WeaponStates.RELOAD)
				{
					mClipsRemaining--;					
				}
			
			break;
			
			case WeaponStates.RELOAD:

				if (newState == WeaponStates.READY)
				{
					mCurrentRoundInClip = 1;
				}
			
			break;
			
			case WeaponStates.DISABLED:
			
			break;
		}

		mTimeInCurrentState = 0f;
		mWeaponState = newState;
	}

	private void FireWeapon()
	{
		mCurrentRoundInClip++;

		Transform muzzlePoint = null;
		Vector3 origin = Vector3.zero;
		Vector3 direction = Vector3.zero;
		int numberOfShots = 1;

		if(mFireType == OrbitTurretDefinition.FireTypes.RANDOM)
		{
			muzzlePoint = mMuzzlePoints[ Random.Range(0, mMuzzlePoints.Length) ];
		}
		else if (mFireType == OrbitTurretDefinition.FireTypes.SEQUENTIAL)
		{
			muzzlePoint = mMuzzlePoints[mCurrentMuzzleIndex];
			mCurrentMuzzleIndex++;

			if (mCurrentMuzzleIndex >= mMuzzlePoints.Length)
			{
				mCurrentMuzzleIndex = 0;
			}
		}
		else if (mFireType == OrbitTurretDefinition.FireTypes.SIMULTANEOUS)
		{
			//TODO: Simultaneous
		}

		if (muzzlePoint == null)
		{
			Debug.LogError("Weapon has no muzzle point");
		}

		origin = muzzlePoint.transform.position;
		direction = muzzlePoint.transform.forward;

		OrbitProjectile projectile = (GameObject.Instantiate(ProjectilePrefab, origin, Quaternion.LookRotation(direction)) as GameObject).GetComponent<OrbitProjectile>();
		projectile.SetupProjectile(ProjectileSpeed, direction, ProjectileLifetime);
	}

	public void InputHandler(object sender, UserInputKeyEvent evt)
	{
		if (evt.KeyBind == OrbitUserInput.Instance.PrimaryWeapon && (evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_DOWN || evt.Type == UserInputKeyEvent.TYPE.GAMEPAD_BUTTON_HELD))
		{
			if (mWeaponState == WeaponStates.READY)
			{
				SetState(WeaponStates.FIRING);
			}
		}
	}

}
