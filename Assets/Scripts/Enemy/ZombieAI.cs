using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
	public Transform player;
	public Animator anim;

	NavMeshAgent agent;

	enum State
	{
		Idle,
		Move,
		Attack
	}

	State currentState = State.Idle;

	float dist;

	bool canMove = true;
	bool canAttack = true;

	public float attackCooldown = 3f;
	float attackTimer;

	public float detectRange = 12f;
	public float loseRange = 20f;
	public float attackRange = 2.5f;


	Vector3 spawnPoint;
	float updatePathTimer;
	float updatePathDelay = 0.3f;

	[Header("Auto Move")]
	public bool autoChase = false;
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		spawnPoint = transform.position;
	}
	public void ForceChasePlayer()
	{
		if (player == null) return;

		autoChase = true;
		ChangeState(State.Move);
	}
	void Update()
	{
		if (player == null || !agent.isOnNavMesh) return;

		dist = Vector3.Distance(transform.position, player.position);
		if (!player.gameObject.activeInHierarchy)
		{
			agent.isStopped = true;
			anim.SetBool("IsMove", false);
			return;
		}
		if (autoChase)
		{
			if (!canMove)
			{
				agent.isStopped = true;
				anim.SetBool("IsMove", false);
				return;
			}

			agent.isStopped = false;
			anim.SetBool("IsMove", true);

			agent.SetDestination(player.position);

			if (dist <= attackRange)
			{
				AttackState();
			}

			return;
		}

		// 👉 zombie thường
		switch (currentState)
		{
			case State.Idle:
				IdleState();
				break;

			case State.Move:
				MoveState();
				break;

			case State.Attack:
				AttackState();
				break;
		}
	}
	void ChasePlayer()
	{
		updatePathTimer -= Time.deltaTime;

		if (updatePathTimer <= 0f)
		{
			agent.SetDestination(player.position);
			updatePathTimer = updatePathDelay;
		}
	}

	void ReturnToSpawn()
	{
		agent.SetDestination(spawnPoint);
	}
	void IdleState()
	{
		agent.isStopped = true;
		anim.SetBool("IsMove", false);

		if (dist <= attackRange && canAttack)
		{
			ChangeState(State.Attack);
			return;
		}

		if (dist <= detectRange && canMove)
		{
			ChangeState(State.Move);
		}
	}

	void MoveState()
	{
		agent.isStopped = false;
		anim.SetBool("IsMove", true);

		updatePathTimer -= Time.deltaTime;

		if (updatePathTimer <= 0f)
		{
			agent.SetDestination(player.position);
			updatePathTimer = updatePathDelay;
		}

		if (dist <= attackRange)
		{
			ChangeState(State.Attack);
			return;
		}

		if (dist > loseRange)
		{
			ChangeState(State.Idle);
		}
	}

	void AttackState()
	{
		
		agent.isStopped = true;
		anim.SetBool("IsMove", false);

		attackTimer -= Time.deltaTime;

		if (attackTimer <= 0f)
		{
			anim.SetTrigger("Attack");
			attackTimer = attackCooldown;
		}
	}
	public void OnAttackFinish()
	{
		ChangeState(State.Idle);
		float distance = Vector3.Distance(transform.position, player.position);

		if (distance <= attackRange)
		{
			DealDamage();
		}
		StartCoroutine(WaitToCanMove(1.5f));
	}
	public void DealDamage()
	{
		SoundManager.Instance.PlaySFX("Zombie Die attack");
		IDamageable damageable = player.GetComponentInParent<IDamageable>();
		if (damageable != null) {
			damageable.TakeDamage(15);
		}
	}
	IEnumerator WaitToCanMove(float time)
	{
		canMove = false;

		yield return new WaitForSeconds(time);

		canMove = true;
	}
	void ChangeState(State newState)
	{
		currentState = newState;
	
	}
	
}