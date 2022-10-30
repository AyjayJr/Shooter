using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbScript : MonoBehaviour
{

    /*
    public Transform pCamera;
    private Quaternion desired;
    
    // Start is called before the first frame update
    void Start()
    {
        desired = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            StartCoroutine(RaiseArm());
        }

        if(Input.GetMouseButtonUp(1))
        {
            StopAllCoroutines();
            StartCoroutine(LowerArm());
        }
    }

    IEnumerator LowerArm()
    {
        while(transform.rotation != desired)
        {
            Quaternion.Lerp(transform.rotation, desired, 0.08f);

            yield return null;
        }
    }

    IEnumerator RaiseArm()
    {
        while(transform.rotation != pCamera.rotation)
        {
            Quaternion.Lerp(transform.rotation, pCamera.rotation, 0.08f);

            yield return null;
        }
    }
    */
}
