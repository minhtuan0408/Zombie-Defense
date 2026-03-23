using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
	PlayerWeaponHandler weaponHandler;

	void Start()
	{
		weaponHandler = GetComponent<PlayerWeaponHandler>();
	}

	// gọi khi nhấn nút bắn
	public void OnShootButton()
	{
		weaponHandler.Shoot();
		SoundManager.Instance.PlaySFX("Gun");
	}

	//// gọi khi nhấn reload
	//public void OnReloadButton()
	//{
	//	weaponHandler.Reload();
	//}
}