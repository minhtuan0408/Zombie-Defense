using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Ammo")]
	public int magazineSize = 30;
	public int currentAmmo;

	public int reserveAmmo = 90;

	[Header("Fire")]
	public float fireRate = 0.15f;
	float fireTimer;

	[Header("VFX")] public ParticleSystem muzzleFlash;
	public Transform firePoint;
	public GameObject bulletPrefab;

	public WeaponData weaponData;
	void Start()
	{
		currentAmmo = magazineSize;
	}

	void Update()
	{
		fireTimer -= Time.deltaTime;
	}

	[Header("Reload")]
	public float reloadTime = 2f;

	public void Shoot(Vector3 direction)
	{
		if (currentAmmo <= 0) return;
		if (fireTimer > 0) return;

		fireTimer = fireRate;
		currentAmmo--;

		SoundManager.Instance.PlaySFX("Gun");

		Quaternion rot = Quaternion.LookRotation(direction);
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
	}
	public void Reload()
	{
		if (currentAmmo == magazineSize) return;
		if (reserveAmmo <= 0) return;

		int neededAmmo = magazineSize - currentAmmo;

		if (reserveAmmo >= neededAmmo)
		{
			currentAmmo += neededAmmo;
			reserveAmmo -= neededAmmo;
		}
		else
		{
			currentAmmo += reserveAmmo;
			reserveAmmo = 0;
		}
	}


}