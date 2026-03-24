using UnityEngine;
using UnityEngine.Pool;

public class PenetratingBullet : MonoBehaviour
{
	public float speed = 20f;
	public float lifeTime = 3f;
	public int damage = 10;
	public int maxPenetrations = 3; // Số lượng mục tiêu có thể xuyên
	public ParticleSystem particleEnd;
	public GameObject GunBullet;

	float timer;
	int penetratedCount = 0;
	Collider col;

	void Awake()
	{
		col = GetComponent<Collider>();
	}

	void OnEnable()
	{
		GunBullet.SetActive(true);
		timer = 0f;
		penetratedCount = 0;
		col.enabled = true;
	}

	void Update()
	{
		// Di chuyển đạn
		transform.position += transform.forward * speed * Time.deltaTime;

		timer += Time.deltaTime;
		if (timer >= lifeTime || penetratedCount >= maxPenetrations)
		{
			ReturnToPool();
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			IDamageable damageable = other.GetComponentInParent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(damage);
			}

			penetratedCount++;

			// Chơi particle khi xuyên xong
			if (penetratedCount >= maxPenetrations)
			{
				particleEnd.Play();
				GunBullet.SetActive(false);
				col.enabled = false;
			}
		}
	}

	void ReturnToPool()
	{
		particleEnd.Play();
		GunBullet.SetActive(false);
		col.enabled = false;
		ObjectPool.Instance.ReturnObject(gameObject);
	}
}