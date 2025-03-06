using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowThroughBezier : MonoBehaviour
{
    public List<Transform> bezierPoints; // Control points (must be 3n + 1)
    
    [Range(0.0f, 1.0f)]
    public float meshLengthFactor = 0.1f; // Fraction of curve occupied by the mesh
    public float flowSpeed = 0.5f; // Speed at which the mesh moves through the curve

    private Mesh originalMesh;
    private Mesh modifiedMesh;

    Vector3[] vertices;
    Vector3[] modifiedVertices;

    private float flowOffset = 0f; // Current movement along the curve

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
        if (bezierPoints.Count < 4 || (bezierPoints.Count - 1) % 3 != 0) return; 
        // Ensure at least one cubic segment (4 points) and (3n+1) structure

        int segmentCount = (bezierPoints.Count - 1) / 3;

        // Move the offset forward over time (looping back at 1)
        flowOffset += flowSpeed * Time.deltaTime;
        flowOffset %= 1.0f + meshLengthFactor; // Keep it between 0 and 1

        for (int i = 0; i < vertices.Length; i++)
        {
            // Convert vertex position to a relative t-value within the moving section
            float localT = Mathf.InverseLerp(originalMesh.bounds.min.z, originalMesh.bounds.max.z, vertices[i].z);

            // Adjust by flowOffset so the mesh moves through the curve
            float globalT = flowOffset + (localT * meshLengthFactor);
            globalT = Mathf.Max(globalT - meshLengthFactor, 0.0f);
            globalT = Mathf.Min(globalT, 1.0f); // Wrap around if exceeding 1

            // Scale t into segment space
            float scaledT = globalT * segmentCount;
            int segmentIndex = Mathf.Min(Mathf.FloorToInt(scaledT), segmentCount - 1);
            float t = scaledT - segmentIndex; // Normalize T for this segment

            // Get the four points for this cubic Bézier segment
            int baseIndex = segmentIndex * 3;
            Vector3 P0 = bezierPoints[baseIndex].position;
            Vector3 P1 = bezierPoints[baseIndex + 1].position;
            Vector3 P2 = bezierPoints[baseIndex + 2].position;
            Vector3 P3 = bezierPoints[baseIndex + 3].position;

            // Compute new position and tangent
            Vector3 position = CubicBezier(t, P0, P1, P2, P3);
            Vector3 tangent = CubicBezierTangent(t, P0, P1, P2, P3).normalized;
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized; // Get sideways normal
            Vector3 binormal = Vector3.Cross(normal, tangent).normalized;  // Compute binormal (up direction)

            // Get original offset
            Vector3 offset = vertices[i] - originalMesh.bounds.center;

            // Transform offset to match curve orientation
            position += (normal * offset.x) + (binormal * offset.y);

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
        float u2 = u * u;
        float t2 = t * t;

        return (u2 * u * P0) +
               (3 * u2 * t * P1) +
               (3 * u * t2 * P2) +
               (t2 * t * P3);
    }

    Vector3 CubicBezierTangent(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        float u = 1 - t;
        return (3 * u * u * (P1 - P0)) +
               (6 * u * t * (P2 - P1)) +
               (3 * t * t * (P3 - P2)); // Derivative of cubic Bézier
    }
}