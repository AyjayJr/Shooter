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
    public float grappleRange;
    //public float grappleSpeed;
    private bool isDeployed = false;

    void Awake()
    {
        lr = GetComponentInChildren<LineRenderer>();

        Debug.Log(playerCamera.name + "   " + playerTransform.name);
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
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

        while(Vector3.Distance(gunTip.position, grappleLocation) >= 2f)
        {
            //Had to increase the interpolation value because the gravity of the player made upward grapples not work (player too thicc)
            playerTransform.position = Vector3.Lerp(playerTransform.position, grappleLocation, 0.1f);

            yield return null;
        }

        //Debug.Log("Destination reached");
        isDeployed = false;
    }

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
