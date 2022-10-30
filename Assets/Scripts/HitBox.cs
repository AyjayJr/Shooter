using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public int health = 10;

    public void onRayCastHit(int damage)
    {
        health -= damage;
    }
}
