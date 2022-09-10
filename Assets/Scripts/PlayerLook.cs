using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
	private float xRotation = 0f;
	private float yRotation = 0f;

	public Transform player;

	[Header("Camera Sensitivity")]
	public float xSens = 1f;
	public float ySens = 1f; 

	[Header("Weapon Sway")]
	[SerializeField] private float smooth;
	[SerializeField] private float swayMultiplier;
	[SerializeField] private GameObject gun;

	public void ProcessLook(Vector2 input)
	{
		// map inputs from actions to variables
		float mouseX = input.x;
		float mouseY = input.y;

		// calculate camera rotation for looking up and down
		xRotation -= (mouseY * ySens);
		yRotation += (mouseX * xSens);
		xRotation = Mathf.Clamp(xRotation, -80f, 80f);

		// apply this to camera and player transform
		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		player.rotation = Quaternion.Euler(0, yRotation, 0);

		// rotate player to look left and right
		transform.Rotate(Vector3.up * (mouseX * xSens));

		WeaponSway(input);
	}

	public void WeaponSway(Vector2 input)
	{
		float mouseX = input.x * swayMultiplier;
		float mouseY = input.y * swayMultiplier;


		Quaternion rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
		Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

		Quaternion targetRotation = rotationX * rotationY;

		gun.transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
}
