using UnityEngine;

public class AutoAim : MonoBehaviour
{
	public float aimRange = 15f;
	public LayerMask enemyLayer;

	public Transform target;

	//void Update()
	//{
	//	if (target == null || Vector3.Distance(transform.position, target.position) > aimRange)
	//	{
	//		FindTarget();
	//	}
	//}

	void FixedUpdate()
	{
		if (target == null || Vector3.Distance(transform.position, target.position) > aimRange)
		{
			FindTarget();
		}
	}

	void FindTarget()
	{
		Collider[] enemies = Physics.OverlapSphere(transform.position, aimRange, enemyLayer);

		float closestDistance = Mathf.Infinity;
		Transform closestTarget = null;

		foreach (Collider enemy in enemies)
		{
			float distance = Vector3.Distance(transform.position, enemy.transform.position);

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestTarget = enemy.transform;
			}
		}

		target = closestTarget;
	}
}