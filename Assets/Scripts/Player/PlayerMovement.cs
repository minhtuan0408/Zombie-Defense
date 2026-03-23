using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public JoystickController joystick;
	public Transform cameraTransform;
	public Animator animator;
	public AutoAim autoAim;

	public float speed = 5f;

	void Update()
	{
		Vector2 input = joystick.Direction;

		float moveAmount = input.magnitude;

		Vector3 camForward = cameraTransform.forward;
		Vector3 camRight = cameraTransform.right;

		camForward.y = 0;
		camRight.y = 0;

		camForward.Normalize();
		camRight.Normalize();

		Vector3 moveDir = camForward * input.y + camRight * input.x;

		transform.position += moveDir * speed * Time.deltaTime;

		animator.SetInteger("Speed", moveAmount > 0.1f ? 1 : 0);

		// chỉ xoay khi KHÔNG có target
		if (moveDir.magnitude > 0.1f && autoAim.target == null)
		{
			transform.forward = moveDir;
		}
	}
}