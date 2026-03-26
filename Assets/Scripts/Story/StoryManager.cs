using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
	public static StoryManager instance;
	public BaseStory[] stories;
	public GameObject CanvasGame;
	public GameObject Player;
	public Camera mainCamera;
	private void Awake()
	{
		instance = this;

	}
	//public void PlayStory(int i)
	//{
	//	BaseStory story = stories[i];


	//	story.OnStoryFinished += OnStoryFinished;

	//	if (CanvasGame != null)
	//		CanvasGame.SetActive(false);
	//	if (Player != null)
	//		Player.SetActive(false);
	//	mainCamera.enabled = false;
	//	story.gameObject.SetActive(true);
	//	story.StartStory();
	//}
	public void PlayStory(int i)
	{
		BaseStory story = stories[i];
		story.OnStoryFinished += OnStoryFinished;
		GamePlayController.Instance.OnStoryStart(); 
		if (CanvasGame != null)
		{
			CanvasGame.GetComponent<CanvasGroup>().alpha = 0;
			CanvasGame.GetComponent<CanvasGroup>().blocksRaycasts = false;
		}


		if (Player != null)
			Player.SetActive(false);

		mainCamera.enabled = false;

		story.gameObject.SetActive(true);
		story.StartStory();
	}
	void OnStoryFinished(BaseStory story)
	{
		Debug.Log("Story Finished");
		mainCamera.enabled = true;
		if (CanvasGame != null)
		{
			CanvasGame.GetComponent<CanvasGroup>().alpha = 1;
			CanvasGame.GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
		if (Player != null)
			Player.SetActive(true);
		story.OnStoryFinished -= OnStoryFinished;

		// báo cho GameManager
		GamePlayController.Instance.OnStoryFinished();
	}
}
