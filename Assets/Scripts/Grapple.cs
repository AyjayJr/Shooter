/*
Currently I have implemented a scoogie ass / halfass fix to a bug where if you look around while grappling so far that gunTip doesnt come close enough to contactPoint, you wont
disconnect. I temporarily fixed it right now by referencing the player script to manually change the sensitivity so you "shouldnt" be able to flick around and bug it out

Another possible better longterm fix could be to have the gun rotate towards the contact point and "follow" the grapple line, that way theoretically it will never bug out since
the gunTip will always be directed towards the contactPoint             IMPLEMENT THIS

Nvm new bug: sometimes grapple glitches and stays on the smaller sensitivity
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    private LineRenderer lr;
    [SerializeField] private Transform playerTransform;
    private Vector3 grappleLocation;
    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform _head;
    //private CPMPlayer moveScript;
    public float grappleRange;
    //public float grappleSpeed;
    private bool isDeployed = false;

    void Awake()
    {
        //moveScript = GetComponentInParent<CPMPlayer>();

        lr = GetComponentInChildren<LineRenderer>();
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(1) && !isDeployed)
        {
            ShootGrapple();
        }
    }

    void LateUpdate()
    {
        DrawGrapple();
    }

    public void ShootGrapple()
    {
        RaycastHit hit;

        if(Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, grappleRange))
        {
            //Debug.Log(hit.point);
            StartCoroutine(DeployGrapple(hit));
        }
    }

    IEnumerator DeployGrapple(RaycastHit hit)
    {
        isDeployed = true;

        grappleLocation = hit.point;

        //Loop to pull player to grapple location and have gun always look at the grapple location
        while(Vector3.Distance(gunTip.position, grappleLocation) >= 2f)
        {
            //SMG looks at grapple point so gunTip never gets flicked around
            transform.LookAt(grappleLocation);

            playerTransform.position = Vector3.Lerp(playerTransform.position, grappleLocation, 0.12f);

            yield return null;
        }

        //Stop drawing line after being pulled
        isDeployed = false;

        //Loop to reset the gun to face in the direction the player is facing
        while(transform.rotation != playerCamera.rotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, playerCamera.rotation, 0.06f);

            yield return null;
        }
    }

    // OLD IMPLEMENTATION WHERE I SLOWED DOWN SENSITIVITY SO THEY COULDNT FLICK AND GLITCH OUT GRAPPLE
    // IEnumerator DeployGrapple(RaycastHit hit)
    // {
    //     float temp = moveScript.xMouseSensitivity;
    //     moveScript.xMouseSensitivity = temp/3;
    //     moveScript.yMouseSensitivity = temp/3;
        
    //     isDeployed = true;

    //     grappleLocation = hit.point;

    //     while(Vector3.Distance(gunTip.position, grappleLocation) >= 2f)
    //     {
    //         //playerCamera.LookAt(grappleLocation);

    //         //Had to increase the interpolation value because the gravity of the player made upward grapples not work (player too thicc)
    //         playerTransform.position = Vector3.Lerp(playerTransform.position, grappleLocation, 0.12f);
    //         //playerCamera.LookAt(grappleLocation, Vector3.up);

    //         yield return null;
    //     }

    //     moveScript.xMouseSensitivity = temp;
    //     moveScript.yMouseSensitivity = temp;
    //     isDeployed = false;
    // }

    void DrawGrapple()
    {
        if(isDeployed)
        {
            lr.positionCount = 2;

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grappleLocation);
        }
        else
        {
            lr.positionCount = 0;
        }
    }

    // public void fireHook()
    // {
    //     if(!isDeployed)
    //     {
    //         isDeployed = true;
    //         if(Physics.Raycast(transform.position, transform.forward, out hit, 50))
    //         {
    //             Debug.DrawLine(transform.position, hit.point, Color.blue, 20);
    //             Debug.Log(hit.point);

    //             StartCoroutine(Swing());
    //         }
    //     }
    //     else
    //     {
    //         Debug.Log("Retrieving hook");
    //         isDeployed = false;
    //         return;
    //     }
    // }

    // IEnumerator Swing()
    // {
    //     // for(float i = 0; i < 1; i = Time.deltaTime * grappleSpeed)
    //     // {
    //     //     player.position = Vector3.Lerp(transform.position, hit.point, 0.1f);
    //     // }

    //     Debug.Log("Swing");

    //     yield return null;
    // }
}
