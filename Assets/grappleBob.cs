using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappleBob : MonoBehaviour
{
    public WeaponController holster;
    public GameObject grapple;
    [Header("Gun Bob")]
    [SerializeField] private Transform grappleHolder;
    
    [System.Serializable]
    public struct BobOverride
    {
        public float minSpeed;
        public float maxSpeed;

        [Header("X Settings")]
        public float speedX;
        public float intenstityX;
        public AnimationCurve bobX;

        [Header("Y Settings")]
        public float speedY;
        public float intenstityY;
        public AnimationCurve bobY;
    }
    public BobOverride[] bobOverrides;
    private float currentTimeX;
    private float currentTimeY;
    private float xPos;
    private float yPos;
    private Vector3 smoothV;
    public float swayIntensityX;
    public float swayIntensityY;
    public float maxSway;
    public float minSway;
    public float currentSpeed;
    void FixedUpdate()
    {
        Vector3 target = new Vector3(xPos, yPos, 0);
        Vector3 desiredPos = Vector3.SmoothDamp(grappleHolder.localPosition, target, ref smoothV, 0.1f);
        grappleHolder.localPosition = desiredPos;
    }

    void Update()
    {
        // if pistol selected; enable grapple -> else; disable
        if (holster.selectedWeapon == WeaponController.Weapons.Pistol)
        {
            grapple.SetActive(true);
        }
        else
        {
            grapple.SetActive(false);
        }

        // now we need to capture the gun bob and sway
        // ended up just copy and pasting the logic smh
        Gunbob();
    }
    void Gunbob()
    {
        foreach (BobOverride bob in bobOverrides)
        {
            if (currentSpeed >= bob.minSpeed && currentSpeed <= bob.maxSpeed)
            {
                float bobMultiplier = (currentSpeed == 0) ? 1 : currentSpeed;

                currentTimeX += bob.speedX / 10 * Time.deltaTime * bobMultiplier;
                currentTimeY += bob.speedY / 10 * Time.deltaTime * bobMultiplier;

                xPos = bob.bobX.Evaluate(currentTimeX) * bob.intenstityX;
                yPos = bob.bobY.Evaluate(currentTimeY) * bob.intenstityY;
            }
        }

        float xSway = -Input.GetAxis("Mouse X") * swayIntensityX;
        float ySway = -Input.GetAxis("Mouse Y") * swayIntensityY;

        xSway = Mathf.Clamp(xSway, minSway, maxSway);
        ySway = Mathf.Clamp(ySway, minSway, maxSway);

        xPos += xSway;
        yPos += ySway;
    }
}