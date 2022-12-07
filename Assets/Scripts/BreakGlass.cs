using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakGlass : MonoBehaviour
{
    [SerializeField] private GameObject shattered;

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player")
        {
            Instantiate(shattered, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
