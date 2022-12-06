using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    public float distance = 10f;
    public float angle = 30f;
    public float height = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 50;
    float scanTimer;
    float scanInterval;
    public LayerMask layers;
    public LayerMask occlusionLayers;

    public List<GameObject> objects = new List<GameObject>();
    Collider[] colliders = new Collider[50];
    int count;

    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1 / scanFrequency;
    }

    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0)
        {
            scanTimer += scanInterval;
            Scan();
        }
    }
    private void Scan()
    {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        objects.Clear();

        for (int i = 0; i < count; i++)
        {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj))
            {
                objects.Add(obj);
            }
        }

    }
    public bool IsInSight(GameObject obj)
    {
        Vector3 origin = transform.position;
        Vector3 dest = obj.transform.position;
        Vector3 direction = dest - origin;
        if (direction.y < 0 || direction.y > height)
        {
            return false;
        }

        direction.y = 0;

        float deltaAngle = Vector3.Angle(direction, transform.forward);
        if ( deltaAngle > angle)
        {
            return false;
        }
        origin.y += height / 2;
        dest.y = origin.y;
        if ( Physics.Linecast(origin,dest, occlusionLayers))
        {
            return false;
        }

        return true;
    }

    public Vector3 GetRandomPoint()
    {
        return new Vector3(
            Random.Range(-distance, distance),
            0,
            Random.Range(-distance, distance)
           );
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int numTriangle = 8;
        int numVertices = numTriangle * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];


        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        int vert = 0;

        // left
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;
        // far

        vertices[vert++] = bottomLeft;
        vertices[vert++] = bottomRight;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = topLeft;
        vertices[vert++] = bottomLeft;

        //top

        vertices[vert++] = topCenter;
        vertices[vert++] = topLeft;
        vertices[vert++] = topRight;
        //bottom


        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomLeft;

        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles; 
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        mesh = CreateWedgeMesh();
    }
    private void OnDrawGizmos()
    {
        if (mesh)
        {
            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        }
    }
}
