using UnityEngine;
using TMPro;
using System.Collections;

public class ComboManager : MonoBehaviour
{
	public static ComboManager Instance;
	public int bestCombo = 0;

	[Header("UI")]
	public TextMeshProUGUI comboText;

	[Header("Settings")]
	public float comboResetTime = 3f;

	int combo = 0;
	float timer;

	Coroutine scaleRoutine;

	void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		if (!gameObject.activeInHierarchy) return; // 👈 thêm dòng này
		if (combo > 0)
		{
			timer -= Time.deltaTime;

			if (timer <= 0)
			{
				ResetCombo();
			}
		}
	}

	public void AddCombo()
	{
		combo++;
		timer = comboResetTime;

		// 👉 cập nhật best combo
		if (combo > bestCombo)
		{
			bestCombo = combo;
		}

		UpdateUI();
	}

	void ResetCombo()
	{
		combo = 0;
		UpdateUI();
	}

	void UpdateUI()
	{
		if (combo <= 1)
		{
			comboText.text = "";
			return;
		}

		comboText.text = "COMBO x" + combo;

		// 👉 màu mượt từ trắng → đỏ
		float t = Mathf.InverseLerp(1, 20, combo);
		comboText.color = Color.Lerp(Color.white, Color.red, t);

		// 👉 tránh chồng coroutine
		if (scaleRoutine != null)
			StopCoroutine(scaleRoutine);

		scaleRoutine = StartCoroutine(ScaleEffect());
	}

	IEnumerator ScaleEffect()
	{
		float duration = 0.25f;
		float time = 0f;

		// 👉 scale theo combo
		float maxScale = 1f + combo * 0.08f;
		maxScale = Mathf.Clamp(maxScale, 1f, 2.2f);

		Vector3 start = Vector3.one * maxScale;
		Vector3 end = Vector3.one;

		while (time < duration)
		{
			time += Time.deltaTime;
			float t = time / duration;

			// mượt hơn
			t = Mathf.SmoothStep(0, 1, t);

			comboText.transform.localScale = Vector3.Lerp(start, end, t);
			yield return null;
		}

		comboText.transform.localScale = Vector3.one;
	}
}