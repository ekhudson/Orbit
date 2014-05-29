using UnityEngine;
using System.Collections;

public class OrbitEffect : MonoBehaviour 
{
	private ParticleSystem[] mParticleSystems;

	private void Start()
	{
		mParticleSystems = GetComponentsInChildren<ParticleSystem>();
	}

	private void FixedUpdate () 
	{
		foreach(ParticleSystem mParticleSystem in mParticleSystems)
		{
			if (mParticleSystem.IsAlive())
			{
				return;
			}
		}

		Destroy(gameObject);
	}
}
