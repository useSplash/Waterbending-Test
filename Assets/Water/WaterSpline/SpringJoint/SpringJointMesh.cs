using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringJointMesh : MonoBehaviour
{
    public int numberOfPoints;
    List<Transform> points = new List<Transform>();

    private Mesh originalMesh;
    private Mesh modifiedMesh;

    Vector3[] vertices;
    Vector3[] modifiedVertices;

    
    void Start()
    {
        originalMesh = GetComponent<MeshFilter>().mesh;
        modifiedMesh = Instantiate(originalMesh);
        GetComponent<MeshFilter>().mesh = modifiedMesh;

        vertices = originalMesh.vertices;
        modifiedVertices = new Vector3[vertices.Length];

        for (int i = 0; i < numberOfPoints; i++)
        {
            // Create the point GameObject
            GameObject point = new GameObject($"Point_{i}");
            point.transform.position = new Vector3(0, 0, i); // Adjust spacing as needed

            // Add Rigidbody component
            Rigidbody rb = point.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 10f;
            rb.angularDrag = 0.05f;
            rb.useGravity = false; // Set to false if you don't want gravity

            // If this isn't the first point, add a SpringJoint and connect it to the previous point
            if (i > 0)
            {
                SpringJoint spring = point.AddComponent<SpringJoint>();
                spring.connectedBody = points[i - 1].GetComponent<Rigidbody>(); // Connect to previous Rigidbody
                spring.spring = 100f;  // Adjust stiffness
                spring.damper = 0.2f;    // Adjust damping
                spring.minDistance = 0.05f;
                spring.maxDistance = 0.1f;
                spring.autoConfigureConnectedAnchor = false;
            }
            else
            {
                rb.isKinematic = true;
            }

            // Add the point's Transform to the list
            points.Add(point.transform);
        }
    }

    void Update()
    {
        if (points != null && points.Count <= 1) { return;}

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
            Vector3 position = Vector3.Lerp(P1.position, P2.position, localT);

            // Calculate tangent (direction of the path)
            Vector3 tangent = (P2.position - P1.position).normalized;

            // Estimate normal (perpendicular to the tangent in XZ plane)
            Vector3 normal = Vector3.Cross(tangent, Vector3.up).normalized;
            
            // Compute binormal (perpendicular to both tangent and normal)
            Vector3 binormal = Vector3.Cross(normal, tangent).normalized;

            // Get original offset from mesh center
            Vector3 offset = vertices[i] - originalMesh.bounds.center;

            // Transform offset to match curve orientation
            position += (normal * offset.x) + (binormal * offset.y);

            // Apply transformation to modified vertex
            modifiedVertices[i] = position;
        }

        // Apply changes
        modifiedMesh.vertices = modifiedVertices;
        modifiedMesh.RecalculateNormals();
        modifiedMesh.RecalculateBounds();
    }
}
