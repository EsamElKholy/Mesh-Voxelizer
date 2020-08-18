
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxelizer : MonoBehaviour
{  
    public float resolution = 16;

    // Start is called before the first frame update
    void Start()
    {
        var volume = CPUVoxelizer.Voxelize(gameObject, resolution);
        MeshFactory.CreateVoxelObject(name + "_Voxelized", volume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
