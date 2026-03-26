using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponHandler : MonoBehaviour
{
	public Weapon[] weapons = new Weapon[2];
	public int currentSlot = 0;

	public Transform handWeapon;
	public AutoAim autoAim;
	public Weapon CurrentWeapon => weapons[currentSlot];

	[Header("Skill")]
	public GameObject skillBulletPrefab;

	public float skillFireRate = 0.7f;   // tốc độ bắn khi skill active
	public int skillManaCost = 50;

	public float skillDuration = 5f;
	public float skillCooldown = 10f;

	[Header("Skill UI")]
	public Image skillFillImage;   // image fill (radial)
	public GameObject canUseImage; // icon sáng khi dùng được
	private bool skillActive = false;
	private float lastSkillShotTime;
	private float lastSkillUseTime;

	public PlayerStats playerStats;

	private void OnEnable()
	{
		// reset skill UI
		if (skillFillImage != null)
			skillFillImage.fillAmount = 0f;

		if (canUseImage != null)
			canUseImage.SetActive(true);

		skillActive = false;
	}
	void Start()
	{
		if (weapons[0] != null)
		{
			EquipWeapon(0, weapons[0]);
			SwitchWeapon(0);
			UIWeapon.Instance.SetSelected(currentSlot);
			UIWeapon.Instance.UpdateAmmo(currentSlot, CurrentWeapon);
		}
	}
	void Update()
	{
		if (isReloading && CurrentWeapon != null)
		{
			UIWeapon.Instance.UpdateAmmo(currentSlot, CurrentWeapon);
		}
		bool canUse =
			Time.time - lastSkillUseTime >= skillCooldown &&
			playerStats.GetCurrentMP() >= skillManaCost;
		if (canUse && !skillActive)
		{
			canUseImage.SetActive(canUse && !skillActive);
		}
		
	}
	public void PickWeapon(Weapon weapon)
	{
		// 1. tìm slot trống
		for (int i = 0; i < weapons.Length; i++)
		{
			if (weapons[i] == null)
			{
				EquipWeapon(i, weapon);
				return;
			}
		}

		// 2. nếu đầy → thay slot còn lại
		int replaceSlot = currentSlot == 0 ? 1 : 0;

		DropWeapon(weapons[replaceSlot]);

		EquipWeapon(replaceSlot, weapon);
		SwitchWeapon(replaceSlot);
	}
	void DropWeapon(Weapon weapon)
	{
		weapon.transform.SetParent(null);

		weapon.transform.position =
			transform.position + transform.forward * 1.5f;

		weapon.gameObject.SetActive(true);
	}
	void EquipWeapon(int slot, Weapon weapon)
	{
		weapons[slot] = weapon;

		weapon.transform.SetParent(handWeapon);
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localRotation = Quaternion.identity;

		weapon.gameObject.SetActive(slot == currentSlot);

		UIWeapon.Instance.SetWeapon(slot, weapon);
	}
	public void HandleSlotClick(int slot)
	{
		if (weapons[slot] == null) return;

		// 👉 click lại slot đang cầm
		if (slot == currentSlot)
		{
			// nếu đang reload → bỏ qua (không cancel)
			if (isReloading) return;

			// nếu không reload → reload
			Reload();
			return;
		}

		// 👉 click slot khác → switch
		SwitchWeapon(slot);
	}
	public void SwitchWeapon(int slot)
	{
		if (weapons[slot] == null) return;

		// 👉 nếu đang reload thì cancel (vì đổi súng)
		if (isReloading)
		{
			StopAllCoroutines();
			isReloading = false;
			UIWeapon.Instance.HideReload(currentSlot);
		}

		// ẩn súng hiện tại
		if (currentSlot != -1 && weapons[currentSlot] != null)
		{
			weapons[currentSlot].gameObject.SetActive(false);
		}

		currentSlot = slot;

		// hiện súng mới
		weapons[slot].gameObject.SetActive(true);

		// 👉 update UI highlight
		UIWeapon.Instance.SetSelected(slot);
	}
	void SkillShotgunShoot(Vector3 shootDir)
	{
		SoundManager.Instance.PlaySFX("ShotGun");
		int bulletCount = 5;
		float spreadAngle = 30f;

		// 👉 ép hướng bắn về mặt phẳng ngang (không cho lên/xuống)
		shootDir.y = 0f;
		shootDir.Normalize();

		for (int i = 0; i < bulletCount; i++)
		{
			float angle = Mathf.Lerp(-spreadAngle / 2f, spreadAngle / 2f, (float)i / (bulletCount - 1));

			// 👉 chỉ xoay quanh trục Y (trái/phải)
			Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
			Vector3 dir = rotation * shootDir;

			GameObject bullet = Instantiate(
				skillBulletPrefab,
				CurrentWeapon.firePoint.position,
				Quaternion.LookRotation(dir)
			);

			bullet.GetComponent<Rigidbody>().velocity = dir * 35f;
		}
	}
	public void Shoot()
	{
		if (CurrentWeapon == null) return;

		Vector3 shootDir;

		if (autoAim != null && autoAim.target != null)
			shootDir = (autoAim.target.position - CurrentWeapon.firePoint.position).normalized;
		else
			shootDir = CurrentWeapon.firePoint.forward;

		// ===== SKILL SHOOT (ƯU TIÊN TRƯỚC) =====
		if (skillActive)
		{
			if (Time.time - lastSkillShotTime < skillFireRate) return;

			lastSkillShotTime = Time.time;

			SkillShotgunShoot(shootDir);
			CameraShake.Instance.Shake(0.3f, 0.2f);

			return; // ❗ chặn luôn không chạy xuống dưới
		}

		// ===== RELOAD CHECK =====
		if (isReloading)
		{
			if (CurrentWeapon.currentAmmo > 0)
			{
				StopAllCoroutines();
				isReloading = false;
				UIWeapon.Instance.HideReload(currentSlot);
			}
			else
			{
				return;
			}
		}

		// ===== NORMAL SHOOT =====
		CurrentWeapon.Shoot(shootDir);

		UIWeapon.Instance.UpdateAmmo(currentSlot, CurrentWeapon);
	}
	public void UseSkill()
	{
		if (Time.time - lastSkillUseTime < skillCooldown)
			return;

		if (!playerStats.UseMana(skillManaCost))
			return;

		lastSkillUseTime = Time.time;

		canUseImage.SetActive(false); // ❗ tắt icon khi bắt đầu dùng

		StartCoroutine(SkillRoutine());
	}
	bool isReloading = false;
	public void Reload()
	{
		if (CurrentWeapon == null) return;
		if (isReloading) return;

		// check giống Weapon để tránh gọi thừa
		if (CurrentWeapon.currentAmmo == CurrentWeapon.magazineSize) return;
		if (CurrentWeapon.reserveAmmo <= 0) return;

		StartCoroutine(ReloadRoutine());
	}
	IEnumerator ReloadRoutine()
	{
		isReloading = true;

		float timer = 0f;
		float reloadTime = CurrentWeapon.reloadTime;

		while (timer < reloadTime)
		{
			timer += Time.deltaTime;

			float progress = timer / reloadTime;

			UIWeapon.Instance.UpdateReloadProgress(currentSlot, progress);

			yield return null;
		}
		SoundManager.Instance.PlaySFX("Reload");
		// 👉 reload thật
		CurrentWeapon.Reload();

		UIWeapon.Instance.UpdateAmmo(currentSlot, CurrentWeapon);
		UIWeapon.Instance.HideReload(currentSlot);

		isReloading = false;
	}
	public void ActivateSkill()
	{
		skillActive = true;
	}

	public void DeactivateSkill()
	{
		skillActive = false;
	}
	IEnumerator SkillRoutine()
	{
		skillActive = true;

		float timer = 0f;

		while (timer < skillDuration)
		{
			timer += Time.deltaTime;

			float progress = timer / skillDuration;

			// 👉 fill giảm dần (1 → 0)
			skillFillImage.fillAmount = 1f - progress;

			yield return null;
		}

		skillFillImage.fillAmount = 0f;

		skillActive = false;
	}
	IEnumerator SkillDuration(float time)
	{
		skillActive = true;

		yield return new WaitForSeconds(time);

		skillActive = false;
	}
	public void AddAmmo(int amount)
	{
		if (CurrentWeapon == null) return;

		CurrentWeapon.reserveAmmo += amount;

		// 👉 update UI luôn
		UIWeapon.Instance.UpdateAmmo(currentSlot, CurrentWeapon);
	}
}