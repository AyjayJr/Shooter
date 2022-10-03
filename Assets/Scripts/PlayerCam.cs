using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
	[SerializeField] private PlayerMovementAdvanced player;
	public float sensX;
	public float sensY;

	public Transform orientation;

	float xRotation;
	float yRotation;

    // Start is called before the first frame update
    void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		GameManager.Instance.onPaused += ManageCursorState;
    }

    // Update is called once per frame
    void Update()
    {
       MouseLook();
    }

	public void MouseLook()
	{
		if (GameManager.Instance.isPaused) return;

		float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX;
    	float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;

		yRotation += mouseX;
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		orientation.rotation = Quaternion.Euler(0, yRotation, 0);
	}

	private void ManageCursorState(bool isPaused)
    {
		if (isPaused)
        {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
        {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

	}
}
