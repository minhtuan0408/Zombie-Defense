using UnityEngine;
using UnityEngine.EventSystems;

public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public PlayerWeaponHandler weaponHandler;

	bool isHolding = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		isHolding = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isHolding = false;
	}

	void Update()
	{
		if (isHolding)
		{
			weaponHandler.Shoot();
		}
	}
}