using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FollowPositionMesh : MonoBehaviour
{
    // Mesh Points
    public int numberOfPoints;
    List<Transform> points = new List<Transform>();
    public Transform head;

    // Position Tracking
    Vector3[] positions; // Fixed-size buffer
    public int delay; // Frames between each point
    
    [Range(0.1f, 2)]
    public float size = 0.1f;
    int numberOfPositions;
    int index = 0;

    private Mesh originalMesh;
    private Mesh modifiedMesh;

    Vector3[] vertices;
    Vector3[] modifiedVertices;

    public bool canCollapse;
    public bool isCollaped;

    int ballSize;
    
    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().mesh;
        modifiedMesh = Instantiate(originalMesh);
        GetComponent<MeshFilter>().mesh = modifiedMesh;

        vertices = originalMesh.vertices;
        modifiedVertices = new Vector3[vertices.Length];

        points.Add(head);

        for (int i = 0; i < numberOfPoints; i++)
        {
            // Create the point GameObject
            GameObject point = new GameObject($"Point_{i}");
            point.transform.parent = transform.parent;
            point.transform.position = head.position;
            points.Add(point.transform);
        }
        
        numberOfPositions = 1000;
        positions = new Vector3[numberOfPositions];
    }

    void Update()
    {
        if (points != null && points.Count <= 1) {return;}

        if (canCollapse || Vector3.Distance(positions[index], points[0].localPosition) > 0.01f)
        {
            // For tracking positions
            index = (index + 1) % positions.Length; // Circular indexing
            positions[index] = points[0].localPosition;  // The first point will be the Head_Point
        }

        ballSize = 0;
        isCollaped = false;

        for (int i = 1; i < points.Count; i++)
        {
            int localIndex = index - (i * delay);
            if (localIndex < 0)
            {
                localIndex += numberOfPositions;
            }
            points[i].transform.localPosition = positions[localIndex];
            if (Vector3.Distance(positions[localIndex], head.localPosition) < 0.1f)
            {
                ballSize += 1;
            };
        }

        if (ballSize == points.Count - 1)
        {
            isCollaped = true;
        }

        head.localScale = Vector3.Lerp(head.localScale, Vector3.one * ballSize * 0.25f * size, 0.5f);

        // For mesh point allocating
        float segmentLength = 1f / (points.Count - 1);
        for (int i = 0; i < vertices.Length; i++)
        {
            float globalT = Mathf.InverseLerp(originalMesh.bounds.min.z, originalMesh.bounds.max.z, vertices[i].z);

            // Determine segment indices
            int segmentIndex = Mathf.Clamp(Mathf.FloorToInt(globalT / segmentLength), 0, points.Count - 2);
            float segmentStartT = segmentIndex * segmentLength;
            float localT = (globalT - segmentStartT) / segmentLength;

            Transform P1 = points[points.Count - segmentIndex - 1];
            Transform P2 = points[points.Count - segmentIndex - 2];

            // Interpolate between P1 and P2
            Vector3 position = Vector3.Lerp(P1.localPosition, P2.localPosition, localT);

            // Calculate tangent (direction of the path)
            Vector3 tangent = (P2.localPosition - P1.localPosition).normalized;

            // Estimate normal (perpendicular to the tangent in XZ plane)
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
            
            // Compute binormal (perpendicular to both tangent and normal)
            Vector3 binormal = Vector3.Cross(normal, tangent).normalized;

            // Get original offset from mesh center
            Vector3 offset = vertices[i] - originalMesh.bounds.center;

            // Transform offset to match curve orientation
            position += (normal * offset.x * size) + (binormal * offset.y * size);

            // Apply transformation to modified vertex
            modifiedVertices[i] = position;
        }

        // Apply changes
        modifiedMesh.vertices = modifiedVertices;
        modifiedMesh.RecalculateNormals();
        modifiedMesh.RecalculateBounds();
    }
}
