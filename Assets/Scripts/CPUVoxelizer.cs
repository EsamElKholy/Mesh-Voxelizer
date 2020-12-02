using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUVoxelizer : MonoBehaviour
{
    public static VoxelOctree BuildVoxelTree(GameObject source, int res)
    {
        var meshFilter = source.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            float maxSize = Mathf.Max(meshFilter.mesh.bounds.size.x, meshFilter.mesh.bounds.size.y, meshFilter.mesh.bounds.size.z);
            VoxelOctree octree = new VoxelOctree(meshFilter.mesh.bounds.center, maxSize, res);
            octree.BuildFullTree();

            for (int i = 0; i < meshFilter.sharedMesh.triangles.Length; i += 3)
            {
                int i0 = meshFilter.sharedMesh.triangles[i + 0];
                int i1 = meshFilter.sharedMesh.triangles[i + 1];
                int i2 = meshFilter.sharedMesh.triangles[i + 2];

                Vector3 v0 = meshFilter.sharedMesh.vertices[i0];
                Vector3 v1 = meshFilter.sharedMesh.vertices[i1];
                Vector3 v2 = meshFilter.sharedMesh.vertices[i2];

                Vector3[] triangle =
                {
                    v0,
                    v1,
                    v2
                };

                octree.CheckTriangles(v0, v1, v2);
            }

            return octree;
        }

        return null;
    }
}
