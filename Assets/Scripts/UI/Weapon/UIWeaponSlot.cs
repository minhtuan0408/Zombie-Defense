using UnityEngine;
using UnityEngine.EventSystems;

public class UIWeaponSlot : MonoBehaviour, IPointerClickHandler
{
	public PlayerWeaponHandler player;
	public int slotIndex;



	public void OnPointerClick(PointerEventData eventData)
	{
		player.HandleSlotClick(slotIndex);
	}
}