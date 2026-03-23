using System.Collections.Generic;
using UnityEngine;

public class AmmoSpawnZone : MonoBehaviour
{
	public GameObject ammoPrefab;

	[Header("Spawn Settings")]
	public int maxAmmo = 10;
	public float spawnDelay = 3f;
	public float spawnRadius = 10f;

	[Header("Start Spawn")]
	public int startAmmoCount = 5;

	private float timer;
	private List<GameObject> ammos = new List<GameObject>();

	void Start()
	{
		// spawn sẵn lúc đầu
		for (int i = 0; i < startAmmoCount; i++)
		{
			SpawnAmmo();
		}
	}

	void Update()
	{
		timer += Time.deltaTime;

		// remove ammo đã bị nhặt
		ammos.RemoveAll(item => item == null);

		if (ammos.Count >= maxAmmo) return;

		if (timer >= spawnDelay)
		{
			timer = 0f;
			SpawnAmmo();
		}
	}

	void SpawnAmmo()
	{
		Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
		randomPos.y = transform.position.y; // giữ trên mặt đất

		GameObject ammo = Instantiate(ammoPrefab, randomPos, Quaternion.identity);

		ammos.Add(ammo);
	}
}