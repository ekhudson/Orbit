using UnityEngine;
using System.Collections;

[System.Serializable]
public class OrbitTurretDefinition 
{
	public Transform[] MuzzlePoints;
	public FireTypes FireType = FireTypes.SEQUENTIAL;
	public GameObject WeaponToUse;

	public enum FireTypes
	{
		SEQUENTIAL,
		SIMULTANEOUS,
		RANDOM,
	}

	public void Setup(GameObject parent)
	{
		OrbitWeapon newWeapon = (GameObject.Instantiate(WeaponToUse) as GameObject).GetComponent<OrbitWeapon>();
		newWeapon.SetMuzzlePoints(MuzzlePoints);
		newWeapon.SetFireType(FireType);
		newWeapon.transform.parent = parent.transform;
		newWeapon.transform.position = Vector3.zero;
	}
}
