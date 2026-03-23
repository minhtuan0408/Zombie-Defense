using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance;

	CameraController cam;
	Coroutine shakeCoroutine;

	void Awake()
	{
		Instance = this;
		cam = GetComponent<CameraController>();
	}

	public void Shake(float duration, float magnitude)
	{
		if (shakeCoroutine != null)
			StopCoroutine(shakeCoroutine);

		shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
	}

	IEnumerator ShakeRoutine(float duration, float magnitude)
	{
		float timer = 0f;

		while (timer < duration)
		{
			timer += Time.deltaTime;

			float x = Random.Range(-2f, 2f) * magnitude;
			float y = Random.Range(-2f, 2f) * magnitude;

			// 👉 KHÔNG đụng transform nữa
			cam.shakeOffset = new Vector3(x, y, 0f);

			yield return null;
		}

		cam.shakeOffset = Vector3.zero;
	}
}