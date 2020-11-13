using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    public GameObject[] Neighbors;

    public bool Active;

    private Material[] OriginalMaterials;

    // Start is called before the first frame update
    void Start()
    {
        this.OriginalMaterials = this.GetComponent<MeshRenderer>().materials;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        var active = !this.Active;
        this.ToggleActive(active);
        var neighbors = this.GetComponent<NavNode>().Neighbors;
        foreach (var neighbor in neighbors)
        {
            neighbor.GetComponent<NavNode>().ToggleActive(active);
        }
    }

    public void ToggleActive(bool active)
    {
        if (active)
        {
            this.GetComponent<MeshRenderer>().materials = Array.Empty<Material>();
        }
        else
        {
            this.GetComponent<MeshRenderer>().materials = this.OriginalMaterials;
        }


        this.Active = active;
    }
}
