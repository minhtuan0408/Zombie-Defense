using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawnZone : MonoBehaviour
{
	public GameObject zombiePrefab;
	public Transform player;

	public int maxZombie = 6;
	public float spawnDelay = 3f;
	public float spawnRadius = 10f;
	public int startZombieCount = 30;
	bool PlayerIn;
	float timer;

	List<GameObject> zombies = new List<GameObject>();

	public Transform[] specialSpawnPoints;
	public int specialSpawnCount = 3;

	BoxCollider zone;

	[Header("Crazy Zombie")]
	public int maxCrazyZombie = 3;
	public float crazySpawnDelay = 5f;

	float crazyTimer;
	void Start()
	{

		timer = spawnDelay;
		zone = GetComponent<BoxCollider>();
		Debug.Log(zone);
		foreach (Transform point in specialSpawnPoints)
		{
			for (int i = 0; i < specialSpawnCount; i++)
			{
				SpawnAtPosition(point.position);
			}
		}

		// spawn random
		for (int i = 0; i < startZombieCount; i++)
		{
			SpawnZombie();
		}

	}


	void SpawnAtPosition(Vector3 pos)
	{
		Vector3 randomOffset = Random.insideUnitSphere * 4f; // bán kính quanh spawn point
		randomOffset.y = 0;

		Vector3 randomPos = pos + randomOffset;

		NavMeshHit hit;

		if (NavMesh.SamplePosition(randomPos, out hit, 3f, NavMesh.AllAreas))
		{
			GameObject zombie = Instantiate(zombiePrefab, hit.position, Quaternion.identity);

			ZombieAI ai = zombie.GetComponent<ZombieAI>();

			if (ai != null)
			{
				ai.player = player;
			}

			zombies.Add(zombie);
		}
	}

	void Update()
	{
		timer -= Time.deltaTime;
		crazyTimer -= Time.deltaTime;

		zombies.RemoveAll(z => z == null);

		// 🧟 zombie thường
		if (timer <= 0f && GetNormalZombieCount() < maxZombie)
		{
			SpawnZombie();
			timer = spawnDelay;
		}

		// 😈 zombie điên
		if (crazyTimer <= 0f && GetCrazyZombieCount() < maxCrazyZombie)
		{
			SpawnCrazyZombie();
			crazyTimer = crazySpawnDelay;
		}
	}

	void SpawnZombie()
	{
		Vector3 center = zone.bounds.center;
		Vector3 size = zone.bounds.size;

		Vector3 randomPos = center + new Vector3(
			Random.Range(-size.x / 2, size.x / 2),
			0,
			Random.Range(-size.z / 2, size.z / 2)
		);
		NavMeshHit hit;
		if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
		{
			Quaternion randomRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

			GameObject zombie = Instantiate(zombiePrefab, hit.position, randomRot);

			ZombieAI ai = zombie.GetComponent<ZombieAI>();

			if (ai != null)
				ai.player = player;

			zombies.Add(zombie);
		}
	}

	public void SpawnCrazyZombie()
	{
		zombies.RemoveAll(z => z == null);

		// 👉 check riêng zombie điên
		if (GetCrazyZombieCount() >= maxCrazyZombie) return;

		Vector3 center = zone.bounds.center;
		Vector3 size = zone.bounds.size;

		Vector3 randomPos = center + new Vector3(
			Random.Range(-size.x / 2, size.x / 2),
			0,
			Random.Range(-size.z / 2, size.z / 2)
		);

		NavMeshHit hit;
		if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
		{
			Quaternion randomRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

			GameObject zombie = Instantiate(zombiePrefab, hit.position, randomRot);

			ZombieAI ai = zombie.GetComponent<ZombieAI>();

			if (ai != null)
			{
				ai.player = player;
				ai.ForceChasePlayer();
			}

			zombies.Add(zombie);
		}
	}
	int GetNormalZombieCount()
	{
		int count = 0;

		foreach (var z in zombies)
		{
			if (z == null) continue;

			ZombieAI ai = z.GetComponent<ZombieAI>();
			if (ai != null && !ai.autoChase)
				count++;
		}

		return count;
	}
	int GetCrazyZombieCount()
	{
		int count = 0;

		foreach (var z in zombies)
		{
			if (z == null) continue;

			ZombieAI ai = z.GetComponent<ZombieAI>();
			if (ai != null && ai.autoChase)
				count++;
		}

		return count;
	}
}