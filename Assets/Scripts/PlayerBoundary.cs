using UnityEngine;

public class PlayerBoundary : MonoBehaviour
{
	public Collider playableArea;

	Vector3 lastValidPosition;

	void Start()
	{
		lastValidPosition = transform.position;
	}

	void Update()
	{
		if (playableArea.bounds.Contains(transform.position))
		{
			lastValidPosition = transform.position;
		}
		else
		{
			transform.position = lastValidPosition;
		}
	}
}