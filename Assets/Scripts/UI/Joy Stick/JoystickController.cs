using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public RectTransform joystickBG;
	public RectTransform joystickHandle;

	public float radius = 100f;

	private Vector2 inputVector;

	public Vector2 Direction => inputVector;

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 pos;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			joystickBG,
			eventData.position,
			eventData.pressEventCamera,
			out pos
		);

		pos = pos / radius;

		inputVector = Vector2.ClampMagnitude(pos, 1);

		joystickHandle.anchoredPosition = inputVector * radius;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		inputVector = Vector2.zero;
		joystickHandle.anchoredPosition = Vector2.zero;
	}
}