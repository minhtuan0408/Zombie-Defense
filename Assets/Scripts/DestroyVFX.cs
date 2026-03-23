using UnityEngine;

public class DestroyVFX : MonoBehaviour
{
	ParticleSystem ps;

	void Start()
	{
		ps = GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if (!ps.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}