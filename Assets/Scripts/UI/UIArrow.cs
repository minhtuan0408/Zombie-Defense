using UnityEngine;

public class UIArrow : MonoBehaviour
{
	public Transform player;
	public Transform target;
	public Transform home;

	public RectTransform arrowUI;
	public Camera cam;

	public float Layofff = 40f; // offset cho sprite (bạn đang cần +90)

	void Update()
	{
		// 👉 chọn mục tiêu hiện tại
		Transform currentTarget = GetCurrentTarget();
		if (currentTarget == null) return;

		// 👉 hướng player → target (bỏ Y)
		Vector3 dir = currentTarget.position - player.position;
		dir.y = 0;

		// 👉 góc world
		float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

		// 👉 trừ rotation camera
		float camY = cam.transform.eulerAngles.y;

		// 👉 xoay UI
		arrowUI.rotation = Quaternion.Euler(0, 0, -(angle - camY) + Layofff);
	}

	// 🎯 chọn target
	Transform GetCurrentTarget()
	{
		// nếu đã win → chỉ về home
		if (GameManager.Instance.doneMission)
		{
			return home;
		}

		// chưa win → chỉ target
		return target;
	}
}