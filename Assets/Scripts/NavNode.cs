using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class NavNode : MonoBehaviour
{
    public GameObject[] Neighbors;

    public NavNodeCickEvent Activated = new NavNodeCickEvent();

    public int X;

    public int Y;

    public int Floor;

    public bool Active;

    private Material[] OriginalMaterials;

    private Pathfinder Pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        this.OriginalMaterials = this.GetComponent<MeshRenderer>().materials;
        this.Pathfinder = GameObject.Find("Lifecycle").GetComponent<Pathfinder>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.Pathfinder.PathStart = null;
        }
    }

    private void OnMouseDown()
    {
        this.Activated.Invoke(this);
        return;

        
        return;

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
