using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneBootstrapper
{
	// CHANGE THIS PATH to your actual Scene A path
	const string SceneAPath = "Assets/Scenes/Loading.unity";

	[MenuItem("Tools/Enable Always Start From Scene A")]
	public static void EnableBoot()
	{
		var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneAPath);

		if (sceneAsset != null)
		{
			EditorSceneManager.playModeStartScene = sceneAsset;
			Debug.Log($"<color=green>Enabled:</color> Play Mode will always start from {SceneAPath}");
		}
		else
		{
			Debug.LogError($"Could not find scene at {SceneAPath}. Check the string path.");
		}
	}

	[MenuItem("Tools/Disable Always Start From Scene A")]
	public static void DisableBoot()
	{
		EditorSceneManager.playModeStartScene = null;
		Debug.Log("<color=yellow>Disabled:</color> Play Mode will use the current active scene.");
	}
}