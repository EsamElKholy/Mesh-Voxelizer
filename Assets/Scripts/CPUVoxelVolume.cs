using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUVoxelVolume
{
    public int width, height, depth;
    public float unitSize;
    public Vector3 start;
    public bool[] grid;
    public Vector3 currentPos;
    public Vector3 currentXYZ;
    public Vector3 voxelSize;

    public CPUVoxelVolume(int w, int h, int d, float unit)
    {
        width = w;
        height = h;
        depth = d;
        unitSize = unit;
        grid = new bool[w * h * d];
        voxelSize = Vector3.one * unitSize;
    }

    public Vector3 GetCoordinate(int x, int y, int z)
    {
        currentPos.x = 0;
        currentPos.y = 0;
        currentPos.z = 0;

        currentXYZ.x = x;
        currentXYZ.y = y;
        currentXYZ.z = z;

        currentPos = start + currentXYZ * unitSize;

        return currentPos;
    }

    public void SetVoxelValue(int x, int y, int z, bool val)
    {
        grid[x + width * (y + height * z)] = val;
    }

    public bool GetVoxelValue(int x, int y, int z)
    {
        return grid[x + width * (y + height * z)];
    }

    public int GetFilledVoxelCount()
    {
        int result = 0;

        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i])
            {
                result++;
            }
        }

        return result;
    }
}
