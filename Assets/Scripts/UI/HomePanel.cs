using UnityEngine;

public class HomePanel : MonoBehaviour
{
	public GameObject root;

	public void Show()
	{
		root.SetActive(true);
	}

	public void Hide()
	{
		root.SetActive(false);
	}

	// Gọi từ button Start
	public void OnClickStart()
	{
		SharedUI.Instance.ShowLoading();
	}
}