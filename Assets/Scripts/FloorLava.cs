using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLava : MonoBehaviour
{
    public Transform lava;
    public float height = 5f;
    private float amountToIncrease;

    // Start is called before the first frame update
    void Start()
    {
        lava.localScale = new Vector3(1, height, 1);
    }

    // Update is called once per frame
    void Update()
    {
        // amountToIncrease += Time.deltaTime;
        // height += amountToIncrease/60f;
        // lava.localScale = new Vector3(1, height, 1);
    }
}
