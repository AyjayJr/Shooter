using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerCam : MonoBehaviour
{
	[SerializeField] private PlayerMovementAdvanced player;
	public float sensX;
	public float sensY;

	public Transform orientation;

	float xRotation;
	float yRotation;
	private PostProcessVolume postProcessVolume;
	private Vignette vignette;

	private void Start()
    {
		PlayerManager.Instance.onPlayerDamaged += UpdateVignette;
		postProcessVolume = GetComponent<PostProcessVolume>();
    }

    // Update is called once per frame
    void Update()
    {
       MouseLook();
    }

	public void MouseLook()
	{
		if (GameManager.Instance.IsPaused) return;

		float mouseX = Input.GetAxisRaw("Mouse X") * Time.fixedDeltaTime * sensX;
    	float mouseY = Input.GetAxisRaw("Mouse Y") * Time.fixedDeltaTime * sensY;

		yRotation += mouseX;
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, -90f, 90f);

		transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		orientation.rotation = Quaternion.Euler(0, yRotation, 0);
	}

	private void UpdateVignette()
    {
		vignette = postProcessVolume.profile.GetSetting<Vignette>();
		if (vignette.intensity <= 0.5f)
			vignette.intensity.value += 0.1f;
    }
}
