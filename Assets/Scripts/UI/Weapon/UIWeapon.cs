using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour
{
	public static UIWeapon Instance;

	public RawImage slot1Icon;
	public TextMeshProUGUI slot1Ammo;
	public Image slot1Selected;
	public Image slot1ReloadBar;

	public RawImage slot2Icon;
	public TextMeshProUGUI slot2Ammo;
	public Image slot2Selected;
	public Image slot2ReloadBar;
	private void OnEnable()
	{
		// reset reload UI (tránh bị kẹt thanh)
		HideReload(0);
		HideReload(1);

		// reset scale nếu có animation trước đó
		transform.localScale = Vector3.one;
	}
	public void UpdateReloadProgress(int slot, float value)
	{
		if (slot == 0)
		{
			slot1ReloadBar.fillAmount = value;
			slot1ReloadBar.gameObject.SetActive(true);
		}
		else
		{
			slot2ReloadBar.fillAmount = value;
			slot2ReloadBar.gameObject.SetActive(true);
		}
	}

	public void HideReload(int slot)
	{
		if (slot == 0)
			slot1ReloadBar.gameObject.SetActive(false);
		//else
		//	slot2ReloadBar.gameObject.SetActive(false);
	}

	void Awake()
	{
		Instance = this;
	}

	public void SetWeapon(int slot, Weapon weapon)
	{
		if (slot == 0)
		{
			slot1Icon.texture = weapon.weaponData.icon;
			slot1Icon.enabled = true;
			UpdateAmmo(0, weapon);
		}
		else
		{
			slot2Icon.texture = weapon.weaponData.icon;
			slot2Icon.enabled = true;
			UpdateAmmo(1, weapon);
		}
	}

	public void UpdateAmmo(int slot, Weapon weapon)
	{
		string ammo = weapon.currentAmmo + " / " + weapon.reserveAmmo;

		if (slot == 0)
			slot1Ammo.text = ammo;
		else
			slot2Ammo.text = ammo;
	}

	public void SetSelected(int slot)
	{
		slot1Selected.gameObject.SetActive(slot == 0);
		//slot2Selected.gameObject.SetActive(slot == 1);
	}
}