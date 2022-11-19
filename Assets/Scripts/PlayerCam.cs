using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pixelplacement;
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
	float maxVignetteIntesity = 0.5f;

	private void Start()
    {
		PlayerManager.Instance.onPlayerDamaged += UpdateVignetteDamaged;
		PlayerManager.Instance.onPlayerRegen += UpdateVignetteHealed;
		postProcessVolume = GetComponent<PostProcessVolume>();
		vignette = postProcessVolume.profile.GetSetting<Vignette>();
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

	private void UpdateVignetteDamaged(float damage)
    {
		float maxHealth = PlayerManager.Instance.MaxHealth;
		float vignetteValue = (damage) / (maxHealth) * (maxVignetteIntesity);
		if (vignette.intensity <= maxVignetteIntesity)
        {
			vignette.intensity.value += Mathf.Clamp(vignetteValue, 0.0f, 0.5f);
		}	
    }

	private void UpdateVignetteHealed(float amountHealed)
	{
		float maxHealth = PlayerManager.Instance.MaxHealth;
		float maxVignetteIntesity = 0.5f;
		float vignetteValue = (amountHealed) / (maxHealth) * (maxVignetteIntesity);
		if (vignette.intensity > 0.0f)
			vignette.intensity.value -= vignetteValue;
	}
}
