using UnityEngine;

public class WeaponPickup : MonoBehaviour
{

	public GameObject pickupUIPrefab;
	private GameObject pickupUIInstance;
	public Transform pickupUICanvas;
	PlayerWeaponHandler playerWeapon;

	public Weapon weapon;
	private bool firstTimeEquip = true;
	public GameObject firstTimeEquipGameObject;
	public void Start()
	{
		weapon = GetComponent<Weapon>();
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerWeapon = other.GetComponent<PlayerWeaponHandler>();
			pickupUIInstance = Instantiate(pickupUIPrefab, pickupUICanvas);
			
			
			pickupUIInstance.GetComponent<PickupUI>().SetPickup(this);
		}
	}

	private void OnTriggerExit(Collider other)
	{

		if (other.CompareTag("Player"))
		{
			if (pickupUIInstance != null)
				Destroy(pickupUIInstance);
		}
	}

	public void PickWeapon()
	{
		if (firstTimeEquip) 
		{
			firstTimeEquip = false;
			firstTimeEquipGameObject.SetActive(false);
		}

		if (pickupUIInstance != null)
			Destroy(pickupUIInstance);
		playerWeapon.PickWeapon(weapon);


	}
}