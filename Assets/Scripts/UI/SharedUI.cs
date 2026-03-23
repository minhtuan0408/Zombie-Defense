using UnityEngine;

public class SharedUI : MonoBehaviour
{
	public static SharedUI Instance;

	public HomePanel homePanel;
	public LoadingPanel loadingPanel;
	public GameObject BG;
	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		ShowHome();
	}

	public void ShowHome()
	{
		homePanel.Show();
		//loadingPanel.Hide();
	}

	public void ShowLoading()
	{
		homePanel.Hide();
		//loadingPanel.Show();
	}
}