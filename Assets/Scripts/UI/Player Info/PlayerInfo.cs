using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
	public PlayerStats player;
	
	public Slider hpSlider;
	public Slider mpSlider;
	void Update()
	{
		hpSlider.value = (float)player.GetCurrentHP() / player.GetMaxHP();
		mpSlider.value = (float)player.GetCurrentMP() / player.GetMaxMP();
	}
}
