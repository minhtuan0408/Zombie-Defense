using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
	public float requiredTime = 10f;

	private float stayTimer = 0f;
	private bool isPlayerInside = false;
	private bool isCompleted = false;

	[Header("UI")]
	public TextMeshProUGUI progressText;
	public Image progressFill;   // 👉 image fill
	public Image progressBG;     // 👉 image đổi màu khi full

	public GameObject BossFollow;
	public GameObject Boss;
	public GameObject Obsstacle;

	public GameObject Tent;

	private void Update()
	{
		if (isCompleted) return;

		if (isPlayerInside)
		{
			stayTimer += Time.deltaTime;

			// update UI
			UpdateUI();

			if (stayTimer >= requiredTime)
			{
				CompleteMission();
			}
		}
		else
		{
			stayTimer = 0f;
			UpdateUI();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerInside = true;

			if (progressText != null)
				progressText.gameObject.SetActive(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerInside = false;

			if (progressText != null)
				progressText.gameObject.SetActive(false);
		}
	}

	void UpdateUI()
	{


		float progress = stayTimer / requiredTime;

		if (progressFill != null)
		{
			progressFill.fillAmount = progress;
		}

		// 👉 khi đầy thì đổi màu xanh
		if (progressBG != null)
		{
			if (progress >= 1f)
				progressBG.color = Color.green;
			else
				progressBG.color = Color.white; // màu mặc định
		}
	}

	void CompleteMission()
	{
		isCompleted = true;
		GameManager.Instance.doneMission = true;
		Tent.SetActive(true );
		if (progressText != null)
			progressText.text = "Hoàn thành!";
	
		if (PlayerProgress.Instance.PlayingLevel == 0)
		{
			BaseStory story = StoryManager.instance.stories[1];
			story.OnStoryFinished += OnStoryAfterMission;
			StoryManager.instance.PlayStory(1);
		}
		else if (PlayerProgress.Instance.PlayingLevel == 1)
		{
			BaseStory story = StoryManager.instance.stories[3];
			story.OnStoryFinished += OnStoryAfterMission2;
			StoryManager.instance.PlayStory(3);
		}


		Debug.Log("Mission Completed!");

		//gameObject.SetActive(false);
	}

	void OnStoryAfterMission(BaseStory story)
	{
		story.OnStoryFinished -= OnStoryAfterMission;
		BossFollow.SetActive(true);
		Debug.Log("Boss xuất hiện sau story!");
	}

	void OnStoryAfterMission2(BaseStory story)
	{
		story.OnStoryFinished -= OnStoryAfterMission2;
		Boss.SetActive(true);
		Obsstacle.SetActive(true);
	}
}