using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Sway")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    [Header("Idle Sway")]
    [SerializeField] private Transform weapon;

    [SerializeField] private float idleA = 1;
    [SerializeField] private float idleB = 2;
    [SerializeField] private float swayScale = 600;
    [SerializeField] private float swayLerpSpeed = 14;

    [SerializeField] private float swayTime;
    [SerializeField] private Vector3 swayPosition;
    

    // Update is called once per frame
    void Update()
    {
        WeaponSway();
        CalculateIdleSway();
    }

    void WeaponSway()
    {
       float mouseX = Input.GetAxisRaw("Mouse X") * multiplier; 
       float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier; 

       Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
       Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
       Quaternion targetRotation = rotationX * rotationY;

       transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    void CalculateIdleSway()
    {
        var targetPosition = LissajousCurve(swayTime, idleA, idleB) / swayScale;
        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;

        if (swayTime > 6.3f)
        {
            swayTime = 0;
        }

        weapon.localPosition = swayPosition;
    }

    Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }
}
