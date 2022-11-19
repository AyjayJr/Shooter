using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public void DamagePlayer(float damage)
    {
        PlayerManager.Instance.LoseHealth(damage);
    }
}
