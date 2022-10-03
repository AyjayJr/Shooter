using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingScript : MonoBehaviour
{
    public Rigidbody player;
    public Transform pCamera;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
        
    private Vector3 grapplePoint;
    private LineRenderer lr;
    private SpringJoint joint;
    private Vector3 direction;
    private ConstantForce grapplePull;
    private bool isDeployed = false;


    void Awake()
    {
        lr = GetComponentInChildren<LineRenderer>();
        grapplePull = player.GetComponent<ConstantForce>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            Grapple();
        }
        else if(Input.GetMouseButtonUp(1))
        {
            StopCoroutine(PullTowards(1));
            isDeployed = false;
            grapplePull.force = Vector3.zero;
            Destroy(joint);
            StartCoroutine(RotateGun());
        }

        if(isDeployed)
        {
            grapplePull.force = Vector3.Normalize(grapplePoint - gunTip.position) * 40f;
        }
    }

    void LateUpdate()
    {
        DrawGrapple();
    }

    public void Grapple()
    {
        RaycastHit hit;

        if(Physics.Raycast(pCamera.position, pCamera.forward, out hit, 30, whatIsGrappleable))
        {
            direction = pCamera.forward;
            isDeployed = true;

            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(gunTip.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.1f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 1f;

            lr.positionCount = 2;

            StartCoroutine(PullTowards(joint.minDistance));
        }
    }

    IEnumerator PullTowards(float minDistance)
    {
        while(Vector3.Distance(gunTip.position, grapplePoint) >= 2f && isDeployed)
        {
            //player.AddForce(direction * 5f, ForceMode.Acceleration);

            transform.LookAt(grapplePoint);
            joint.maxDistance = Vector3.Distance(grapplePoint, player.gameObject.transform.position);

           yield return null;
        }

        isDeployed = false;
        grapplePull.force = Vector3.zero;

        StartCoroutine(RotateGun());
    }

    void DrawGrapple()
    {
        if(isDeployed)
        {
            lr.positionCount = 2;

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }
        else
        {
            lr.positionCount = 0;
        }
    }

    IEnumerator RotateGun()
    {
        while(transform.rotation != pCamera.rotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, pCamera.rotation, 0.06f);

            yield return null;
        }
    }
}
