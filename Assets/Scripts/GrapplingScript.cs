using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingScript : MonoBehaviour
{
    public Rigidbody player;
    public Transform pCamera;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;

    [Header("Values of Joint")]
    public float spring;
    public float damper;
    public float massScale;
        
    private Vector3 grapplePoint;
    private LineRenderer lr;
    private SpringJoint joint;
    private Vector3 direction;
    private ConstantForce grapplePull;
    private bool isDeployed = false;
    
    public float pullForceValue;
    public float cameraForceValue;


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
            EndGrapple();
        }

        if(isDeployed)
        {
            grapplePull.force = (pCamera.forward * cameraForceValue) + (Vector3.Normalize(grapplePoint - gunTip.position) * (pullForceValue));
            //grapplePull.force = Vector3.Normalize(grapplePoint - gunTip.position) * 40f;
            //grapplePull.relativeForce = pCamera.forward * cameraForceValue;

            checkIfFacingGrapple();
        }
    }

    public bool Deployed()
    {
        return isDeployed;
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

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;

            StartCoroutine(PullTowards(joint.minDistance));
        }
    }

    public void EndGrapple()
    {
        StopCoroutine(PullTowards(1));
        isDeployed = false;
        grapplePull.force = Vector3.zero;
        Destroy(joint);
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

    public void checkIfFacingGrapple()
    {
        float angle = Mathf.Abs(Vector3.Angle(pCamera.forward, grapplePoint - gunTip.position));

        if(isDeployed && angle >= 150)
        {
            Debug.Log("Broke Grapple");
            EndGrapple();
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

    IEnumerator RotateGun()
    {
        while(transform.rotation != pCamera.rotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, pCamera.rotation, 0.06f);

            yield return null;
        }
    }
}