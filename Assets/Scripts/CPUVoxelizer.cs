using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUVoxelizer : MonoBehaviour
{
    public static VoxelVolume Voxelize(GameObject source, int resolution)
    {
        var meshFilter = source.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            return null;
        }

        meshFilter.mesh.RecalculateBounds();
        Bounds bounds = meshFilter.mesh.bounds;

        if (resolution % 2 != 0)
        {
            resolution++;
        }

        float maxLength = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        float minLength = Mathf.Min(bounds.size.x, Mathf.Min(bounds.size.y, bounds.size.z));
        float unit = Mathf.Min(maxLength / resolution, minLength);
        float halfUnit = unit / 2;

        var start = bounds.min + new Vector3(halfUnit, halfUnit, halfUnit);
        var end = bounds.max + new Vector3(halfUnit, halfUnit, halfUnit);
        var size = end - start;

        int xCount = Mathf.CeilToInt(size.x / unit);
        int yCount = Mathf.CeilToInt(size.y / unit);
        int zCount = Mathf.CeilToInt(size.z / unit);

        var volume = new VoxelVolume(xCount, yCount, zCount, unit, start);

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

            for (int x = 0; x < volume.width; x++)
            {
                for (int y = 0; y < volume.height; y++)
                {
                    for (int z = 0; z < volume.depth; z++)
                    {
                        if (GeometryUtils.TriangleBoxIntersection(triangle, volume.GetCoordinate(x, y, z), volume.GetVolumeSize()) == 1)
                        {
                            volume.SetVoxelValue(x, y, z, 1);
                        }
                    }
                }
            }
        }

        return volume;
    }

    public static VoxelVolume CreateVoxelVolume(GameObject source, int resolution)
    {
        var meshFilter = source.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            return null;
        }

        meshFilter.mesh.RecalculateBounds();
        Bounds bounds = meshFilter.mesh.bounds;

        if (resolution % 2 != 0)
        {
            resolution++;
        }

        float maxLength = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        float minLength = Mathf.Min(bounds.size.x, Mathf.Min(bounds.size.y, bounds.size.z));
        float unit = Mathf.Min(maxLength / resolution, minLength);
        float halfUnit = unit / 2;

        var start = bounds.min + new Vector3(halfUnit, halfUnit, halfUnit);
        var end = bounds.max + new Vector3(halfUnit, halfUnit, halfUnit);
        var size = end - start;

        int xCount = Mathf.CeilToInt(size.x / unit);
        int yCount = Mathf.CeilToInt(size.y / unit);
        int zCount = Mathf.CeilToInt(size.z / unit);

        var volume = new VoxelVolume(xCount, yCount, zCount, unit, start);

        return volume;
    }
}
