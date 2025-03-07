using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaterbending : MonoBehaviour
{
    GameObject[] puddles;
    public float bendingRange;

    public Transform waterHead;
    public Transform waterHover;

    GameObject closestPuddle;

    Coroutine movementCoroutine;

    void Start()
    {
        puddles = GameObject.FindGameObjectsWithTag("Puddle");
    }

    void Update()
    {
        if (puddles != null)
        {
            if (closestPuddle != FindClosestObject(puddles) && closestPuddle != null)
            {
                closestPuddle.GetComponent<PuddleSelector>().Deselect();
            }

            closestPuddle = FindClosestObject(puddles);

            if (Vector3.Distance(closestPuddle.transform.position, transform.position) < bendingRange)
            {
                closestPuddle.GetComponent<PuddleSelector>().Select();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left Click");
            if (movementCoroutine != null) {StopCoroutine(movementCoroutine);}
            movementCoroutine = StartCoroutine(OrbitAround(waterHead, transform));
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Click");
            if (movementCoroutine != null) {StopCoroutine(movementCoroutine);}
            movementCoroutine = StartCoroutine(MoveTo(waterHead, waterHover, loop:true));
        }
    }

    GameObject FindClosestObject(GameObject[] objects)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(currentPosition, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }

    IEnumerator MoveTo(Transform objToMove,
                        Transform target,
                        float speed = 0.1f,
                        bool loop = false
                        )
    {
        while (loop || Vector3.Distance(objToMove.position, target.position) < 0.01f)
        {
            objToMove.position = Vector3.Lerp(objToMove.position, target.position, speed);
            yield return null; // Wait for the next frame
        }
    }

    IEnumerator OrbitAround(
        Transform objToMove,
        Transform orbitReference, 
        float distanceFromObject = 5.0f,
        float speed = 5.0f, 
        float distanceVariance = 0, 
        float heightVariance = 0)
    {
        Vector3 localAxis = Vector3.right;
        while (true)
        {

            // Compute new position in local space
            Vector3 orbitDirection = new Vector3(
                Mathf.Cos(Time.time * speed),
                0,
                Mathf.Sin(Time.time * speed)
            );

            // Apply position relative to the orbit reference
            objToMove.position = Vector3.Lerp(objToMove.position, orbitReference.position + (orbitDirection*distanceFromObject), 0.1f);

            yield return null; // Wait for the next frame
        }
    }
}
