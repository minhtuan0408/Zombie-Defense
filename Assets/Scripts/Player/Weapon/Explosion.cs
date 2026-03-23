using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public int damage = 40;
	public float radius = 4f;
	public LayerMask enemyLayer;

	public ParticleSystem effect;

	IEnumerator Start()
	{
		yield return null; // ⏳ đợi 1 frame

		if (effect) effect.Play();

		DoDamage();

		Destroy(gameObject, 2f);
	}

	void DoDamage()
	{
		Collider[] hits = Physics.OverlapSphere(
			transform.position,
			radius,
			enemyLayer
		);
		foreach (var hit in Physics.OverlapSphere(transform.position, radius))
		{
			Debug.Log("Hit: " + hit.name + " | Layer: " + LayerMask.LayerToName(hit.gameObject.layer));
		}

		foreach (var hit in hits)
		{
			IDamageable dmg = hit.GetComponent<IDamageable>();

			if (dmg != null)
			{
				dmg.TakeDamage(damage);
			}
		}

		//Collider[] hits = Physics.OverlapSphere(transform.position, radius);

		//Debug.Log("Hit count (no mask): " + hits.Length);

		//foreach (var hit in hits)
		//{
		//	Debug.Log("Hit: " + hit.name + " | Layer: " + LayerMask.LayerToName(hit.gameObject.layer));
		//}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}