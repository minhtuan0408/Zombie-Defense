using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
	public Transform player;
	public float height = 30f;

	void LateUpdate()
	{
		transform.position = new Vector3(player.position.x, height, player.position.z);
	}
}