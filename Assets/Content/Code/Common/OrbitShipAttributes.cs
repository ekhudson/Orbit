using UnityEngine;
using System.Collections;

[System.Serializable]
public class OrbitShipAttributes : MonoBehaviour
{
	public int Health = 100;
	public float ThrustPerSecond = 4f;
	public float ThrustMax = 0.75f;
	public float DragAmount = 0.97f;
	public float RotationDegreesPerSecond = 270f;
	public OrbitWeapon PrimaryWeapon;
	public OrbitWeapon SecondaryWeapon;
	public OrbitTurretDefinition[] TurretDefinitions;
}
