using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
	public static LoadingPanel Instance;

	public GameObject root;
	public Animator animator;


	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		// 👇 Lần đầu vào: luôn ở trạng thái đã fade xong (ẩn UI)
		root.SetActive(true);
		animator.Play("EndLoading", 0, 1f);
	}

	// 👉 Gọi khi muốn load scene
	public void LoadScene(string sceneName)
	{

		StartCoroutine(LoadRoutine(sceneName));
	}

	IEnumerator LoadRoutine(string sceneName)
	{
		//root.SetActive(true);

		// 1. Fade đen
		SharedUI.Instance.BG.SetActive(true);
		animator.Play("StartLoading", 0, 0f);

		// đợi anim fade in xong (set đúng thời gian anim của bạn)
		yield return new WaitForSeconds(1f);

		// 2. Load scene
		SceneManager.LoadScene(sceneName);

		// 3. Chờ 1 frame cho scene load xong
		yield return null;

		// 4. Fade ra
		animator.Play("EndLoading", 0, 0f);
		SharedUI.Instance.BG.SetActive(false);
		// đợi fade out xong
		yield return new WaitForSeconds(1f);
		

		//root.SetActive(false);
	}
}