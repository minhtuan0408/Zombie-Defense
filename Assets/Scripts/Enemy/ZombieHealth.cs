using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour, IDamageable
{
	public int maxHealth = 50;
	ZombieAI ZombieAI;
	int currentHealth;
	bool isDead = false;

	Animator anim;
	NavMeshAgent agent;
	Collider col;

	public ParticleSystem VFX;

	public Renderer rend;
	MaterialPropertyBlock mpb;
	public Transform ORBpos;
	void Start()
	{
		currentHealth = maxHealth;
		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		col = GetComponent<Collider>();
		ZombieAI = GetComponent<ZombieAI>();
		mpb = new MaterialPropertyBlock();
	}

	public void TakeDamage(int damage)
	{
		if (isDead) return;   // không nhận damage nữa

		currentHealth -= damage;

		if (VFX != null)
			VFX.Play();

		rend.GetPropertyBlock(mpb);
		mpb.SetFloat("_HitBlend", 1f);
		rend.SetPropertyBlock(mpb);

		Invoke(nameof(ResetHit), 0.1f);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	void ResetHit()
	{
		rend.GetPropertyBlock(mpb);
		mpb.SetFloat("_HitBlend", 0f);
		rend.SetPropertyBlock(mpb);
	}

	void Die()
	{
		if (isDead) return;

		isDead = true;

		if (col) col.enabled = false;
		if (agent) agent.enabled = false;
		if (anim) anim.enabled = false;
		DropMana();
		ComboManager.Instance.AddCombo();
		StartCoroutine(DieAction());
	}
	IEnumerator DieAction()
	{

		yield return null;
		

		Destroy(gameObject);
	}
	public GameObject manaOrbPrefab;
	public int manaDrop = 10;
	public int manaPerOrb = 2;


	void DropMana()
	{
		int orbCount = Mathf.CeilToInt((float)manaDrop / manaPerOrb);

		for (int i = 0; i < orbCount; i++)
		{
			Vector3 offset = Random.insideUnitSphere * 0.5f;
			//offset.y += 6;

			GameObject orb = Instantiate(
				manaOrbPrefab,
				ORBpos.position + offset,
				Quaternion.identity
			);

			ManaOrb mana = orb.GetComponent<ManaOrb>();
			Transform player = ZombieAI.player;
			mana.Init(player);
			
			mana.manaValue = manaPerOrb;
		}
	}
}