using UnityEngine;

public class MeshBending : MonoBehaviour
{
    public Transform StartPoint;  // P0
    public Transform MidPoint1;   // P1
    public Transform MidPoint2;   // P2
    public Transform EndPoint;    // P3

    private Mesh originalMesh;
    private Mesh modifiedMesh;

    Vector3[] vertices;
    Vector3[] modifiedVertices;

    public float bias = 1.0f;

    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().mesh;
        modifiedMesh = Instantiate(originalMesh);
        GetComponent<MeshFilter>().mesh = modifiedMesh;

        vertices = originalMesh.vertices;
        modifiedVertices = new Vector3[vertices.Length];
    }

    void Update()
    {
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        // Find min and max Z values
        foreach (var v in vertices)
        {
            if (v.z < minZ) minZ = v.z;
            if (v.z > maxZ) maxZ = v.z;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            float t = Mathf.InverseLerp(minZ, maxZ, vertices[i].z);

            float offsetX = vertices[i].x - originalMesh.bounds.center.x;
            float offsetY = vertices[i].y - originalMesh.bounds.center.y;

            // Cubic Bezier Interpolation
            Vector3 position = CubicBezier(t, EndPoint.position, MidPoint2.position, MidPoint1.position, StartPoint.position);

            // Maintain X and Y offsets
            position += new Vector3(offsetX, offsetY, 0);

            modifiedVertices[i] = position;
        }

        // Apply changes
        modifiedMesh.vertices = modifiedVertices;
        modifiedMesh.RecalculateNormals();
        modifiedMesh.RecalculateBounds();
    }

    Vector3 CubicBezier(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
    float u = 1 - t;
    float u2 = Mathf.Pow(u, bias); // Apply bias to (1 - t)
    float t2 = Mathf.Pow(t, bias); // Apply bias to t

    return (u2 * u2 * u) * P0 +
           (3 * u2 * u * t2) * P1 +
           (3 * u * t2 * t2) * P2 +
           (t2 * t2 * t) * P3;
    }

}