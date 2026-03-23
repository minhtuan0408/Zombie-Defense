using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour, IDamageable
{
	public Transform player;
	public Animator anim;

	private NavMeshAgent agent;

	[Header("Range")]
	public float detectRange = 15f;
	public float attackRange = 2.5f;

	[Header("Attack")]
	public float attackCooldown = 2f;
	float attackTimer;

	[Header("Stun")]
	public float stunDuration = 0.5f;
	float stunTimer;

	[Header("HP")]
	public int maxHP = 200;
	int currentHP;

	public GameObject DamagedRegion;
	bool isAttacking;
	public GameObject Obstacle;
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		currentHP = maxHP;
	}

	void Update()
	{
		if (player == null || !agent.isOnNavMesh) return;

		float dist = Vector3.Distance(transform.position, player.position);

		// 🔵 đang attack thì KHÓA state
		if (isAttacking)
		{
			agent.isStopped = true;
			anim.SetBool("IsMove", false);
			return;
		}

		// 🟢 ngoài tầm đánh → đuổi
		if (dist > attackRange)
		{
			agent.isStopped = false;
			agent.SetDestination(player.position);

			anim.SetBool("IsMove", true);
		}
		else
		{
			// 🟡 trong tầm đánh

			agent.isStopped = true;
			anim.SetBool("IsMove", false);

			// nếu hồi chiêu xong → đánh
			if (attackTimer <= 0f)
			{
				StartAttack();
			}
			else
			{
				// chưa hồi xong → đứng idle
				attackTimer -= Time.deltaTime;
			}
		}
	}
	void StartAttack()
	{
		isAttacking = true;

		agent.isStopped = true;
		anim.SetBool("IsMove", false);
		anim.SetTrigger("Attack");

		attackTimer = attackCooldown;
	}


	// 👉 gọi từ Animation Event
	public void TriggerDamageRegion()
	{
		if (DamagedRegion == null) return;

		DamagedRegion.SetActive(true);
		StartCoroutine(DisableDamageRegion(2f));
	}
	public void OnAttackFinish()
	{
		isAttacking = false;
	}
	IEnumerator DisableDamageRegion(float time)
	{
		yield return new WaitForSeconds(time);
		DamagedRegion.SetActive(false);
	}
	// 🔥 bị bắn
	public void TakeDamage(int damage)
	{
		currentHP -= damage;
		Debug.Log(currentHP);
		// stun
		stunTimer = stunDuration;

		if (currentHP <= 0)
		{
			Die();
		}
	}
	void Die()
	{
		BaseStory story = StoryManager.instance.stories[4];
		StoryManager.instance.PlayStory(4);
		gameObject.SetActive(false);
		Obstacle.SetActive(false);
	}


}