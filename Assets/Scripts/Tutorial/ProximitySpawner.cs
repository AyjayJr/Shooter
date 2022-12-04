using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class ProximitySpawner : MonoBehaviour
{
    [SerializeField] private GameObject objToSpawn;
    [SerializeField] private Transform spawnedObjEndPos;
    [SerializeField] private Animator animator;
    [SerializeField] private float finalScale = 0.2f;

    private bool oneAndDone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !oneAndDone)
        {
            if (GetComponent<AudioSource>())
                GetComponent<AudioSource>().Play();
            animator.SetTrigger("Entered");
            oneAndDone = true;
            StartCoroutine(Triggered());
        }
    }

    private IEnumerator Triggered()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject go = Instantiate(objToSpawn, transform);
        Tween.Position(go.transform, spawnedObjEndPos.position, 1f, 0.0f, Tween.EaseInOutStrong);
        Tween.LocalScale(go.transform, new Vector3(finalScale, finalScale, finalScale), 0.6f, 0.05f);
        Destroy(this, 0.5f);
    }
}
