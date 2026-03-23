using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
	public int levelID;

	[Header("Unlock")]
	public bool isUnlocked;


}