using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
[System.Serializable]
public class DialogLine
{
	public string speaker;
	public string text;
	public Sprite avatar; 
}
[System.Serializable]
public class StoryStep
{
	public Animator animator;
	public string animationName;

	public DialogLine[] dialogs;
}

public class BaseStory : MonoBehaviour
{
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogText;
	public GameObject dialogPanel;

	public Image Avatar;

	public Camera storyCamera;

	public StoryStep[] steps;

	int currentStep = 0;
	public Action<BaseStory> OnStoryFinished;
	void Start()
	{

		if (dialogPanel != null)
			dialogPanel.SetActive(false);
	}
	public void StartStory()
	{
		currentStep = 0;


		if (storyCamera != null)
			storyCamera.enabled = true;

		PlayStep();
	}

	void PlayStep()
	{
		if (currentStep >= steps.Length)
		{
			EndStory();
			return;
		}

		StoryStep step = steps[currentStep];

		if (step.animator != null && !string.IsNullOrEmpty(step.animationName))
		{
			step.animator.Play(step.animationName);
		}
		else
		{
			OnAnimationFinished();
		}
	}

	// gọi từ Animation Event
	public void OnAnimationFinished()
	{
		StoryStep step = steps[currentStep];

		if (step.dialogs != null && step.dialogs.Length > 0)
		{
			StartCoroutine(PlayDialog(step.dialogs));
		}
		else
		{
			NextStep();
		}
	}

	IEnumerator PlayDialog(DialogLine[] dialogs)
	{
		dialogPanel.SetActive(true);

		foreach (var line in dialogs)
		{
			nameText.text = line.speaker;
			dialogText.text = line.text;

			// 👇 thêm phần set avatar
			if (Avatar != null)
			{
				Avatar.sprite = line.avatar;
				Avatar.gameObject.SetActive(line.avatar != null);
			}

			yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
		}

		dialogPanel.SetActive(false);

		NextStep();
	}

	void NextStep()
	{
		currentStep++;
		PlayStep();
	}

	void EndStory()
	{
		if (storyCamera != null)
			storyCamera.enabled = false;




		if (dialogPanel != null)
			dialogPanel.SetActive(false);

		OnStoryFinished?.Invoke(this);

		gameObject.SetActive(false);
	}
}