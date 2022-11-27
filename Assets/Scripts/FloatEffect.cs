using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatEffect : MonoBehaviour
{
    public float amplitude;          //Set in Inspector 
    public float speed;                  //Set in Inspector 
    private float tempVal;
    private Vector3 startPos;
    private Vector3 destination;

    void Start()
    {
        startPos = transform.position;
        destination = startPos;
    }

    void Update()
    {
        float distance = (destination - transform.position).magnitude;
        
        if (distance < 0.1)
        {
            float rand = Random.Range(-1f, 1f);
            destination.y = startPos.y + amplitude * rand;
        }
        
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime );
    }
}
