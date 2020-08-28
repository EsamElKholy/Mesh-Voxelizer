using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GridMesh : MonoBehaviour
{
    public Material mat;
    public int GridSize;
    int currentSize;

    void Awake()
    {
        CreateGrid();
        currentSize = GridSize;
    }

    private void Update()
    {
        if (currentSize != GridSize)
        {
            CreateGrid();
            currentSize = GridSize;
        }
    }

    void CreateGrid()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();
        for (int i = 0; i <= GridSize; i++)
        {
            verticies.Add(new Vector3(i, 0, 0));
            verticies.Add(new Vector3(i, 0, GridSize));

            indicies.Add(4 * i + 0);
            indicies.Add(4 * i + 1);

            verticies.Add(new Vector3(0, 0, i));
            verticies.Add(new Vector3(GridSize, 0, i));

            indicies.Add(4 * i + 2);
            indicies.Add(4 * i + 3);
        }

        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (mat)
        {
            meshRenderer.sharedMaterial = mat;
        }
        else
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Custom/Grid"));
        }
    }
}