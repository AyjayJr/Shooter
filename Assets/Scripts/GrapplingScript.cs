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
    public float range = 20;

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

    [HideInInspector] public Coroutine rotate; 

    void Awake()
    {
        lr = hand.GetComponent<LineRenderer>();
        player = PlayerManager.Instance.player.GetComponent<Rigidbody>();
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
            grapplePull.force = (pCamera.forward * cameraForceValue) + (Vector3.Normalize(grapplePoint - hand.position) * (pullForceValue));
            checkIfFacingGrapple();
        }

        //to fix when joint isnt deleted
        if(!isDeployed && joint != null)
        {
            Destroy(joint);
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

        if(Physics.Raycast(pCamera.position, pCamera.forward, out hit, range, whatIsGrappleable))
        {
            direction = pCamera.forward;
            isDeployed = true;
            SoundManager.Instance.PlaySFXOnce(SoundManager.GameSounds.GrappleShoot);

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

            StartCoroutine(PullTowards());
        }
    }

    public void EndGrapple()
    {
        StopCoroutine(PullTowards());
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

    public IEnumerator PullTowards()
    {
        while(Vector3.Distance(hand.position, grapplePoint) >= 0.1f && isDeployed)
        {
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
            limb.rotation = Quaternion.Lerp(limb.rotation, pCamera.rotation, 0.06f);
            yield return null;
        }
    }
}
