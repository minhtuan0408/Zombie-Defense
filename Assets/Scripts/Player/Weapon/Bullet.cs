using UnityEngine;

using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
	public float speed = 20f;
	public float lifeTime = 3f;
	public int damage = 10;

	public ParticleSystem particleEnd;
	public GameObject GunBullet;
	float timer;
	bool hasHit;
	Collider col;

	void Awake()
	{
		col = GetComponent<Collider>();
	}

	void OnEnable()
	{
		GunBullet.SetActive(true);
		timer = 0;
		hasHit = false;
		col.enabled = true;
	}

	void Update()
	{
		if (!hasHit)
		{
			transform.position += transform.forward * speed * Time.deltaTime;
		}

		timer += Time.deltaTime;

		if (timer >= lifeTime)
		{
			ObjectPool.Instance.ReturnObject(gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (hasHit) return;

		if (other.CompareTag("Enemy"))
		{
			hasHit = true;

			IDamageable damageable = other.GetComponentInParent<IDamageable>();
			if (damageable != null)
			{
				damageable.TakeDamage(damage);
			}

			col.enabled = false;
			particleEnd.Play();
			GunBullet.SetActive(false);
		}
	}
}
public enum BulletType
{
	Bullet_1, Bullet_2, Bullet_3,
	Normal,     
	Fire,       
	Explosive  
}