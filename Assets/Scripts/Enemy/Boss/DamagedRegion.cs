using System.Collections.Generic;
using UnityEngine;

public class DamagedRegion : MonoBehaviour
{
	public int damage = 30;

	private HashSet<GameObject> hitObjects = new HashSet<GameObject>();

	public ParticleSystem posion;
	public ParticleSystem posionQuirt;
	private void OnEnable()
	{
		hitObjects.Clear();


	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (hitObjects.Contains(other.gameObject)) return;
			hitObjects.Add(other.gameObject);

			IDamageable damageable = other.GetComponentInParent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(damage);
			}
		}
	}
}