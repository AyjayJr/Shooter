using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapTemp : MonoBehaviour
{
    public GameObject pistol;
    public GameObject gaussCannon;

    private GrapplingScript pistolScript;
    private GaussCannon gaussScript;

    private bool pistolBool = true;
    private bool gaussBool = false;

    // Start is called before the first frame update
    void Awake()
    {
        pistol.SetActive(pistolBool);
        gaussCannon.SetActive(gaussBool);

        pistolScript = pistol.GetComponentInChildren<GrapplingScript>();
        gaussScript = gaussCannon.GetComponentInChildren<GaussCannon>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            SwapGuns();
        }
    }

    void SwapGuns()
    {
        if(pistolBool)
        {
            if(pistolScript.pull != null)
            {
                StopCoroutine(pistolScript.pull);

                Destroy(pistolScript.joint);
                pistolScript.isDeployed = false;
                pistolScript.lr.positionCount = 0;
                pistolScript.player.GetComponent<ConstantForce>().force = Vector3.zero;

                pistolScript.limb.rotation = pistolScript.pCamera.rotation;
            }
            else if(pistolScript.rotate != null)
            {
                StopCoroutine(pistolScript.rotate);

                pistolScript.limb.rotation = pistolScript.pCamera.rotation;
            }
        }
        else
        {
            if(gaussScript.uncharge != null)
            {
                StopCoroutine(gaussScript.uncharge);

                gaussScript.charge = 0;
                gaussScript.charging = false;
                gaussScript.chargeMeter.localScale = new Vector3(1, 0, 1);
            }
            else if(gaussScript.laserShot != null)
            {
                StopCoroutine(gaussScript.laserShot);

                gaussScript.laser.enabled = false;
            }

            Debug.Log("Stop the gauss cannon coroutines");
        }

        pistolBool = !(pistol.activeSelf);
        gaussBool = !(gaussCannon.activeSelf);

        pistol.SetActive(pistolBool);
        gaussCannon.SetActive(gaussBool);

        if(gaussBool)
        {
            gaussScript.laser.enabled = true;
        }
    }
}
