using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshSlicer : MonoBehaviour
{
    public SlicingPlane slicingPlane;

    private new Renderer renderer;
    private Vector4 planeEquation;
    private Material[] materials = new Material[0];

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        if (renderer)
        {
            materials = renderer.sharedMaterials;
        }

        if (slicingPlane)
        {
            if (transform.parent.GetComponent<MeshSlicer>())
            {
                slicingPlane.transform.position = transform.parent.GetComponent<MeshSlicer>().slicingPlane.transform.position;
            }
            else
            {
                slicingPlane.ResetPlanePosition(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (slicingPlane)
        {
            slicingPlane.meshToSlice = gameObject;
        }

        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        if (slicingPlane)
        {
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
            }

            if (renderer)
            {
                if (materials.Length == 0)
                {
                    materials = renderer.sharedMaterials;
                }
            }

            if (materials.Length > 0)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    {
                        materials[i].SetVector("_SlicingPlane", slicingPlane.GetEquation());
                    }
                }

                slicingPlane.UpdateEquation();
            }            
        }
    }
}
