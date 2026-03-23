using UnityEngine;

public class MiniMapPlayerIcon : MonoBehaviour
{
	public Transform player;
	public RectTransform map;
	public RectTransform playerIcon;
	public Collider playableArea;

	void Update()
	{
		Bounds b = playableArea.bounds;

		float x = Mathf.InverseLerp(b.min.x, b.max.x, player.position.x);
		float z = Mathf.InverseLerp(b.min.z, b.max.z, player.position.z);

		//playerIcon.anchoredPosition = new Vector2(
		//	x * map.rect.width,
		//	z * map.rect.height
		//);
	}
}