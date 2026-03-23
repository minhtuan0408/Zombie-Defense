using UnityEngine;
using UnityEngine.UI;

public class HitScreenUI : MonoBehaviour
{
	public Image hitImage;
	public float maxAlpha = 0.5f;
	public float fadeSpeed = 5f;

	private float currentAlpha = 0f;

	void Start()
	{
		SetAlpha(0f);
	}

	void Update()
	{
		if (currentAlpha > 0)
		{
			currentAlpha = Mathf.Lerp(currentAlpha, 0f, Time.deltaTime * fadeSpeed);
			SetAlpha(currentAlpha);
		}
	}

	public void Trigger()
	{
		currentAlpha = maxAlpha;
		SetAlpha(currentAlpha);
	}

	void SetAlpha(float a)
	{
		Color c = hitImage.color;
		c.a = a;
		hitImage.color = c;
	}
}