using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaterbending : MonoBehaviour
{
    GameObject[] puddles;
    public float bendingRange;

    void Start()
    {
        puddles = GameObject.FindGameObjectsWithTag("Puddle");
    }

    void Update()
    {
        if (puddles != null)
        {
            GameObject closestPuddle = FindClosestObject(puddles);
            if (Vector3.Distance(closestPuddle.transform.position, transform.position) < bendingRange)
            {
                foreach (GameObject puddle in puddles)
                {
                    puddle.GetComponent<PuddleSelector>().Deselect();
                }
                closestPuddle.GetComponent<PuddleSelector>().Select();
            }
            else
            {
                foreach (GameObject puddle in puddles)
                {
                    puddle.GetComponent<PuddleSelector>().Deselect();
                }
            }
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

}
