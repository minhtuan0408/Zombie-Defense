using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;

	public Vector3 offset = new Vector3(0, 4, -6);

	public float rotateSpeed = 0.2f;
	public float smoothSpeed = 10f;

	public float pitch = 20f;
	public float minPitch = 10f;
	public float maxPitch = 60f;

	float yaw = 0f;
	int cameraFingerId = -1;

	public float minZoom = -3f;
	public float maxZoom = -10f;
	public float zoomSpeed = 0.01f;
	[HideInInspector] public Vector3 shakeOffset;
	void Update()
	{
		// ROTATE CAMERA
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);

			if (cameraFingerId == -1 && touch.phase == TouchPhase.Began)
			{
				if (touch.position.x > Screen.width * 0.4f)
				{
					cameraFingerId = touch.fingerId;
				}
			}

			if (touch.fingerId == cameraFingerId)
			{
				if (touch.phase == TouchPhase.Moved)
				{
					yaw += touch.deltaPosition.x * rotateSpeed;

					pitch -= touch.deltaPosition.y * rotateSpeed;
					pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
				}

				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					cameraFingerId = -1;
				}
			}
		}
	}

	void LateUpdate()
	{
		Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

		Vector3 desiredPosition = target.position + rotation * offset;

		Vector3 finalPos = desiredPosition + shakeOffset;

		transform.position = Vector3.Lerp(
			transform.position,
			finalPos,
			smoothSpeed * Time.deltaTime
		);

		transform.LookAt(target.position + Vector3.up * 1.5f);
	}
}