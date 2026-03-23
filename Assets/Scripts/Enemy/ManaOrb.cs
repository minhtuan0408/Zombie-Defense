using UnityEngine;

public class ManaOrb : MonoBehaviour
{
	public float moveSpeed = 10f;
	public float attractDistance = 6f;
	public float collectDistance = 0.5f;
	public int manaValue = 2;

	Transform player;
	bool isAttracted = false;

	public void Init(Transform targetPlayer)
	{
		player = targetPlayer;
		
	}

	void Update()
	{
		if (player == null) return;

		float dist = Vector3.Distance(transform.position, player.position);

		if (dist < attractDistance)
			isAttracted = true;

		if (isAttracted)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				player.position,
				moveSpeed * Time.deltaTime
			);

			if (dist < collectDistance)
				Collect();
		}
	}

	void Collect()
	{
		PlayerStats stats = player.GetComponent<PlayerStats>();

		if (stats != null)
			stats.AddMana(manaValue);

		Destroy(gameObject);
	}
}