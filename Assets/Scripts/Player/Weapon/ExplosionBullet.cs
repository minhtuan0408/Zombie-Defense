using System.Collections;
using UnityEngine;

public class ExplosionBullet : MonoBehaviour
{
	public float speed = 15f;
	public float lifeTime = 3f;

	[Header("Explosion")]
	public GameObject explosionPrefab;
	public float explodeDelay = 0.3f; // delay sau khi chạm

	public LayerMask enemyLayer;

	private float timer;
	private bool isTriggered = false;
	int impactDamage = 20;
	public ParticleSystem hitEffect; // particle khi chạm
	public GameObject visual; // model viên đạn (optional)
	Collider col;

	void Awake()
	{
		col = GetComponent<Collider>();
	}

	void OnEnable()
	{
		timer = 0;
		isTriggered = false;

		// ✅ bật lại collider
		if (col != null)
			col.enabled = true;

		// ✅ bật lại visual
		if (visual != null)
			visual.SetActive(true);

		// ✅ reset particle nếu cần
		if (hitEffect != null)
			hitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}
	void Update()
	{
		if (isTriggered) return;

		transform.position += transform.forward * speed * Time.deltaTime;

		timer += Time.deltaTime;
		if (timer >= lifeTime)
		{
			Explode();
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (isTriggered) return;

		if (((1 << other.gameObject.layer) & enemyLayer) == 0)
			return;

		IDamageable dmg = other.GetComponentInParent<IDamageable>();

		if (dmg != null)
		{

			dmg.TakeDamage(impactDamage); // 💥 lần 1
		}

		isTriggered = true;

		StartCoroutine(HitAndExplode());
	}
	IEnumerator HitAndExplode()
	{
		col.enabled = false;

		if (visual != null)
			visual.SetActive(false);

		if (hitEffect != null)
		{
			hitEffect.Play();
			yield return new WaitUntil(() => !hitEffect.IsAlive());
		}

		Explode();
	}
	void Explode()
	{
		// spawn explosion
		Instantiate(explosionPrefab, transform.position, Quaternion.identity);

		// trả về pool hoặc destroy
		ObjectPool.Instance.ReturnObject(gameObject);
	}
}