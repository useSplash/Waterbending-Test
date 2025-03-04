using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBending : MonoBehaviour
{
    public Transform StartPoint; 
    public Transform EndPoint; 

     [Range(0f, 1f)]
    public float offset;

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

        Debug.Log(originalMesh.bounds);
    }

    void Update()
    {
        float maxZ = originalMesh.bounds.extents.z;
        float centerZ = originalMesh.bounds.center.z;

        float meshLength = Mathf.Abs(centerZ - maxZ) * 2;

        for (int i = 0; i < vertices.Length; i++)
        {
            float t = Mathf.InverseLerp(maxZ - meshLength, maxZ, vertices[i].z);
            
            modifiedVertices[i] = Vector3.Lerp(EndPoint.position, StartPoint.position, t);
        }

        // Apply changes
        modifiedMesh.vertices = modifiedVertices;
        modifiedMesh.RecalculateNormals();
        modifiedMesh.RecalculateBounds();
    }
}
