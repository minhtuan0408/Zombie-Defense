using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamageable
{
	[Header("HP")]
	public int maxHP = 100;
	public int currentHP;

	[Header("MP")]
	public int maxMP = 100;
	public int currentMP;
	bool isDead = false;
	public HitScreenUI hitUI;

	public GameObject EndingPanelOBJ;
	public EndingPanel EndingPanel;
	void Start()
	{
		currentHP = maxHP;
	}
	[Header("Hit Screen FX")]
	public float hitAlpha = 1f;
	public float fadeSpeed = 5f;

	private Material hitMat;
	private float currentAlpha = 0f;
	// ===== HP =====
	public void TakeDamage(int damage)
	{
		currentHP -= damage;

		hitUI.Trigger(); // 👈 thêm dòng này
		SoundManager.Instance.PlaySFX("Hit");
		if (currentHP <= 0)
		{
			SoundManager.Instance.PlaySFX("PlayerGetDame");
			Die();
		}
	}
	IEnumerator ShowEndingPanelDelay()
	{
		yield return null;

		EndingPanelOBJ.SetActive(true);
		EndingPanel.ShowEnding();

	}
	void Die()
	{
		if (isDead) return; // 👈 chặn gọi nhiều lần
		isDead = true;

		DisablePlayer();

		StartCoroutine(ShowEndingPanelDelay()); // 👈 gọi 1 lần duy nhất
		
	}
	public ParticleSystem HPUpVFX;


	public ParticleSystem MPUpVFX;

	void DisablePlayer()
	{
		GetComponent<PlayerMovement>().enabled = false;
		
	}
	public void Heal(int amount)
	{
		currentHP += amount;
		currentHP = Mathf.Clamp(currentHP, 0, maxHP);

		
		HPUpVFX.Play();

	}

	// ===== Mana =====
	public void AddMana(int amount)
	{
		currentMP += amount;
		currentMP = Mathf.Clamp(currentMP, 0, maxMP);

		MPUpVFX.Play();
	}

	public bool UseMana(int amount)
	{
		if (currentMP < amount)
			return false;

		currentMP -= amount;
		return true;
	}

	// ===== Getter cho UI =====
	public int GetCurrentHP()
	{
		return currentHP;
	}
	public int GetMaxHP()
	{
		return maxHP;
	}

	public int GetCurrentMP()
	{
		return currentMP;
	}
	public int GetMaxMP()
	{
		return maxMP;
	}

}