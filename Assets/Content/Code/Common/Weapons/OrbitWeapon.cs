using UnityEngine;
using System.Collections;

[System.Serializable]
public class OrbitWeapon : ScriptableObject 
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
	public int TurretIndex = 0;

	private float mCurrentCooldownTime = 0f;
	private float mCurrentReloadTime = 0f;
	private float mCurrentChargeTime = 0f;
	private float mTimeInCurrentState = 0f;
	private int mCurrentRoundInClip = 1;
	private int mClipsRemaining = 0;
	private OrbitPlayerComponent mOwnerPlayer;
	private WeaponStates mWeaponState = WeaponStates.READY;

	public enum WeaponStates
	{
		READY,
		CHARGING,
		FIRING,
		COOLDOWN,
		RELOAD,
		DISABLED,
	}

	public WeaponStates WeaponState
	{
		get
		{
			return mWeaponState;
		}
	}

	public void Start()
	{
		mClipsRemaining = TotalNumberOfClips;	
	}

	public void Update()
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

	public void FireWeapon(Transform[] muzzlePoints)
	{
		SetState(WeaponStates.FIRING);
		mCurrentRoundInClip++;

		Vector3 origin = Vector3.zero;
		Vector3 direction = Vector3.zero;

		foreach(Transform muzzlePoint in muzzlePoints)
		{
			origin = muzzlePoint.transform.position;
			direction = muzzlePoint.transform.forward;

			OrbitProjectile projectile = (GameObject.Instantiate(ProjectilePrefab, origin, Quaternion.LookRotation(direction)) as GameObject).GetComponent<OrbitProjectile>();
			projectile.SetupProjectile(ProjectileSpeed, direction, ProjectileLifetime);
		}
	}
}
