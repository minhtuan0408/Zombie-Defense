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
			Debug.Log("Skill used!");
		}
		else
		{
			Debug.Log("Not enough mana");
		}
	}
}