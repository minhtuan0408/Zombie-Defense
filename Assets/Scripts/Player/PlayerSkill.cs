using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
	PlayerStats stats;

	void Start()
	{
		stats = GetComponent<PlayerStats>();
	}

	public void UseSkill()
	{
		if (stats.UseMana(30))
		{
			SoundManager.Instance.PlaySFX("Cooldown");
			Debug.Log("Skill used!");
		}
		else
		{
			SoundManager.Instance.PlaySFX("Wrong");
			Debug.Log("Not enough mana");
		}
	}
}