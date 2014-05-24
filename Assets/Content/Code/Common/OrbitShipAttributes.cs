using UnityEngine;
using System.Collections;

public class OrbitShipAttributes : MonoBehaviour 
{
	public int Health = 100;
	public float ThrustPerSecond = 4f; //TODO: Hook these in properly
	public float ThrustMax = 0.75f;
	public float DragAmount = 0.97f;
	public OrbitTurretDefinition[] TurretDefinitions;

	private void Start()
	{
		foreach(OrbitTurretDefinition turret in TurretDefinitions)
		{
			turret.Setup(gameObject);
		}
	}
}
