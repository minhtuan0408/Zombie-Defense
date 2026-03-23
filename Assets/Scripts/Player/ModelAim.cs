using UnityEngine;

public class ModelAim : MonoBehaviour
{
	public Transform player;
	public AutoAim autoAim;

	public float aimRotateSpeed = 10f;
	public float idleRotateSpeed = 3f; // xoay chậm khi không có enemy

	void Update()
	{
		if (autoAim.target != null)
		{
			Vector3 dir = autoAim.target.position - player.position;
			dir.y = 0;

			if (dir.sqrMagnitude > 0.01f)
			{
				Quaternion rot = Quaternion.LookRotation(dir);
				transform.rotation = Quaternion.Slerp(
					transform.rotation,
					rot,
					Time.deltaTime * aimRotateSpeed
				);
			}
		}
		else
		{
			Quaternion rot = Quaternion.LookRotation(player.forward);

			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				rot,
				Time.deltaTime * idleRotateSpeed
			);
		}
	}
}