using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WayPointSystem : MonoBehaviour
{
    public WayPoint[] wayPoints;
    // Start is called before the first frame update
    void Start()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();
        wayPoints = new WayPoint[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            wayPoints[i] = new WayPoint(transforms[i], i);
        }

        foreach (WayPoint w in wayPoints)
        {
            WayPoint[] closest = wayPoints.OrderBy((t) => (t.transform.position - w.transform.position).sqrMagnitude).ToArray();
            WayPoint[] neighbors = new WayPoint[4];
            for (int j = 0; j < 4; j++)
            {
                neighbors[j] = closest[j];
            }
            w.neighboirs = neighbors;
        }
    }

}

public class WayPoint
{
    public Transform transform;
    public WayPoint[] neighboirs;
    public int id;

    public WayPoint(Transform transform, int id)
    {
        this.transform = transform;
        this.id = id;
    }
}