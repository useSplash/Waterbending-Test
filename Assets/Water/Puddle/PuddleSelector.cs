using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleSelector : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public void Select()
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Selected", 1.0f); // Bool as 1 or 0
        rend.SetPropertyBlock(propBlock);
    }

    public void Deselect()
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Selected", 0.0f); // Bool as 1 or 0
        rend.SetPropertyBlock(propBlock);
    }
}
