using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringJointMesh : MonoBehaviour
{
    public List<Transform> points; 

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
    }

    void Update()
    {

        for (int i = 0; i < vertices.Length; i++)
        {
            // Do smthn
        }

        // Apply changes
        modifiedMesh.vertices = modifiedVertices;
        modifiedMesh.RecalculateNormals();
        modifiedMesh.RecalculateBounds();
    }
}
