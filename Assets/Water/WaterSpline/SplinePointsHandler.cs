using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePointsHandler : MonoBehaviour
{
    public List<Transform> nodes;
    public SplinePath path;
    List<int> currentPoints = new List<int> { 0, 0, 0, 0 };

    public float nodeSpeed;
    public float slowDistance;
    public float changePointDistance;
    public float acceleration = 5f; // Controls how fast the velocity reaches the target

    private List<Rigidbody> nodeRigidbodies = new List<Rigidbody>();

    void Start()
    {
        // Initialize rigidbodies
        foreach (var node in nodes)
        {
            Rigidbody rb = node.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = node.gameObject.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.isKinematic = false;
            nodeRigidbodies.Add(rb);
        }
    }

    void Update()
    {
        if (!path) {return;}

        for (int i = 0; i < nodes.Count; i++)
        {
            // Check if at the end and the path doesn't loop
            if (currentPoints[i] == path.points.Count && !path.looping)
            {
                continue;
            }

            float speed = nodeSpeed;
            if (i > 0 && Vector3.Distance(nodes[i].position, nodes[i - 1].position) < slowDistance &&
                currentPoints[i] == currentPoints[i - 1])
            {
                speed *= 0.5f;
            }
            else
            {
                speed *= 1.2f;
            }

            Vector3 targetPosition = path.points[currentPoints[i]].position;
            Vector3 direction = (targetPosition - nodes[i].position).normalized;
            Vector3 targetVelocity = direction * speed;
            
            // Lerp to target velocity
            nodeRigidbodies[i].velocity = Vector3.Lerp(nodeRigidbodies[i].velocity, targetVelocity, acceleration * Time.deltaTime);

            // Check if reached target point
            if (Vector3.Distance(nodes[i].position, targetPosition) < changePointDistance)
            {
                currentPoints[i] = currentPoints[i] + 1;
                if (currentPoints[i] == path.points.Count && path.looping)
                {
                    currentPoints[i] = 0;
                }
            }
        }
    }
}
