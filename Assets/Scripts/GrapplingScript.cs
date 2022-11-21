using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingScript : MonoBehaviour
{
    public Rigidbody player;
    public Transform pCamera;
    public Transform limb;
    public Transform hand;
    public LayerMask whatIsGrappleable;

    [Header("Values of Joint")]
    public float spring;
    public float damper;
    public float massScale;
        
    private Vector3 grapplePoint;
    [HideInInspector] public LineRenderer lr;
    [HideInInspector] public SpringJoint joint;
    private Vector3 direction;
    private ConstantForce grapplePull;
    [HideInInspector] public bool isDeployed = false;
    
    public float pullForceValue;
    public float cameraForceValue;

    [HideInInspector] public Coroutine pull;
    [HideInInspector] public Coroutine rotate; 

    void Awake()
    {
        lr = hand.GetComponent<LineRenderer>();
        grapplePull = player.GetComponent<ConstantForce>();

        Debug.Log(limb.gameObject.name + "   " + hand.gameObject.name);
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
            grapplePull.force = (pCamera.forward * cameraForceValue) + (Vector3.Normalize(grapplePoint - hand.position) * (pullForceValue));
            //grapplePull.force = Vector3.Normalize(grapplePoint - limb.position) * 40f;
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

            float distanceFromPoint = Vector3.Distance(hand.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.1f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            lr.positionCount = 2;

            pull = StartCoroutine(PullTowards(joint.minDistance));
        }
    }

    public void EndGrapple()
    {
        StopCoroutine(pull);
        isDeployed = false;
        grapplePull.force = Vector3.zero;
        Destroy(joint);
        rotate = StartCoroutine(RotateGun());
    }

    void DrawGrapple()
    {
        if(isDeployed)
        {
            lr.positionCount = 2;

            lr.SetPosition(0, hand.position);
            lr.SetPosition(1, grapplePoint);
        }
        else
        {
            lr.positionCount = 0;
        }
    }

    public void checkIfFacingGrapple()
    {
        float angle = Mathf.Abs(Vector3.Angle(pCamera.forward, grapplePoint - hand.position));

        if(isDeployed && angle >= 150)
        {
            Debug.Log("Broke Grapple");
            EndGrapple();
        }
    }

    IEnumerator PullTowards(float minDistance)
    {
        while(Vector3.Distance(hand.position, grapplePoint) >= 2f && isDeployed)
        {
            //player.AddForce(direction * 5f, ForceMode.Acceleration);

            //transform.LookAt(grapplePoint);
            limb.LookAt(grapplePoint);
            joint.maxDistance = Vector3.Distance(grapplePoint, player.gameObject.transform.position);

           yield return null;
        }

        isDeployed = false;
        grapplePull.force = Vector3.zero;

        rotate = StartCoroutine(RotateGun());
    }

    IEnumerator RotateGun()
    {
        while(limb.rotation != pCamera.rotation)
        {
            
            //transform.rotation = Quaternion.Lerp(transform.rotation, pCamera.rotation, 0.06f);
            limb.rotation = Quaternion.Lerp(limb.rotation, pCamera.rotation, 0.06f);

            yield return null;
        }
    }
}
