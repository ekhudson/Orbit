using UnityEngine;
using System.Collections;

public class BrawlerKillzone : TriggerVolume 
{
	public Transform[] SpawnPoints;
	public Vector3 SpawnSpreadAmount = Vector3.zero;
	public Transform KillParticlePrefab;
	public LayerMask PlayerLayer;

	public override void OnTriggerEnter(Collider collider)
	{		
		if ((1 << collider.gameObject.layer) == PlayerLayer)
		{
			if(collider.GetComponent<BrawlerPlayerComponent>() == null)
			{
				return;
			}

			Transform go = (Transform)Instantiate(KillParticlePrefab, collider.transform.position, Quaternion.identity);
			ParticleSystem deathParticle = go.GetComponent<ParticleSystem>();			

			if (deathParticle != null)
			{
				deathParticle.startColor = collider.GetComponent<BrawlerPlayerComponent>().PlayerColor;
				Destroy (go.gameObject, deathParticle.duration);
			}

			Transform spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];

			collider.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, collider.transform.position.z) 
				+ new Vector3(Random.Range(-SpawnSpreadAmount.x, SpawnSpreadAmount.x),Random.Range(-SpawnSpreadAmount.y, SpawnSpreadAmount.y), Random.Range(-SpawnSpreadAmount.z, SpawnSpreadAmount.z));


			if (collider.rigidbody != null)
			{
				collider.rigidbody.velocity = Vector3.zero;
			}
		}
		
		base.OnTriggerEnter(collider);
	}
}
