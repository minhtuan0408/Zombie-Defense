using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
	public int ammoAmount = 30;
	public float attractSpeed = 10f;
	public float attractDistance = 5f;

	public Transform player;
	private bool isAttracting = false;


	void Update()
	{
		if (player == null) return;

		float distance = Vector3.Distance(transform.position, player.position);

		// 👉 bắt đầu hút khi vào range
		if (distance < attractDistance)
		{
			isAttracting = true;
		}

		// 👉 bay về phía player
		if (isAttracting)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				player.position,
				attractSpeed * Time.deltaTime
			);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerWeaponHandler handler = other.GetComponent<PlayerWeaponHandler>();

			if (handler != null)
			{
				handler.AddAmmo(ammoAmount);
			}
			SoundManager.Instance.PlaySFX("PickUp");
			Destroy(gameObject);
		}
	}
}