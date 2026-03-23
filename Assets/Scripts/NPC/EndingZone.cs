using System.Collections;
using UnityEngine;

public class EndingZone : MonoBehaviour
{
	public GameObject EndingPanelOBJ;
	public EndingPanel EndingPanel;
	public GameObject BossFollow;
	private bool hasTriggered = false;

	private void OnCollisionEnter(Collision collision)
	{
		if (hasTriggered) return;
		if (!collision.gameObject.CompareTag("Player")) return;

		if (GamePlayController.Instance.isInStory) return;

		if (GameManager.Instance.AreAllQuestsCompleted())
		{
			hasTriggered = true;
			BossFollow.SetActive(false);

			if (PlayerProgress.Instance.PlayingLevel == 0)
			{
				BaseStory story = StoryManager.instance.stories[2];
				StoryManager.instance.PlayStory(2);
				story.OnStoryFinished += OnStoryAfterMission;
			}
			else if (PlayerProgress.Instance.PlayingLevel == 1)
			{
				StartCoroutine(ShowEndingPanelDelay());
			}
			else
			{
				StartCoroutine(ShowEndingPanelDelay());
			}
		}
		else
		{
			Debug.Log("❌ Chưa hoàn thành hết nhiệm vụ");
		}
	}

	void OnStoryAfterMission(BaseStory story)
	{
		story.OnStoryFinished -= OnStoryAfterMission;

		StartCoroutine(ShowEndingPanelDelay());
	}

	IEnumerator ShowEndingPanelDelay()
	{
		yield return new WaitForSeconds(1f); 

		EndingPanelOBJ.SetActive(true);
		EndingPanel.ShowEnding();

	}
}