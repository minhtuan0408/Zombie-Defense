using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupUI : MonoBehaviour
{
	WeaponPickup weaponPickup;
	public TextMeshProUGUI Name;
	public TextMeshProUGUI currentBullet;
	public RawImage RawImage;
	public void SetPickup(WeaponPickup pickup)
	{
		weaponPickup = pickup;
		Name.text = pickup.weapon.weaponData.name;
		RawImage.texture = pickup.weapon.weaponData.icon;
	}

	public void OnClickPick()
	{
		weaponPickup.PickWeapon();

		Debug.Log("Đã bấm");
	}
}