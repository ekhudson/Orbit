using UnityEngine;
using System.Collections;

[System.Serializable]
public class OrbitShipAttributes : MonoBehaviour
{
	public bool Invincible = false;
	public int MaxHealth = 100;
	public float ThrustPerSecond = 4f;
	public float ThrustMax = 0.75f;
	public float DragAmount = 0.97f;
	public float RotationDegreesPerSecond = 270f;
	public OrbitWeapon PrimaryWeapon;
	public OrbitWeapon SecondaryWeapon;
	public OrbitTurretDefinition[] TurretDefinitions;

	private int mCurrentHealth = -1;

	public int Health
	{
		get
		{
			return mCurrentHealth;
		}
		set
		{
			if (Invincible)
			{
				return;
			}

			mCurrentHealth = Mathf.Clamp(value, 0, MaxHealth);
		}
	}

	private void Start()
	{
		mCurrentHealth = MaxHealth;
	}
}
