using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelVolume
{
    public int width, height, depth;
    public float unitSize;
    public Vector3 start;
    public int[] grid;
    public Vector3 currentPos;
    public Vector3 currentXYZ;
    protected int filledCount;
    public ComputeBuffer gridBuffer;

    public VoxelVolume(int w, int h, int d, float unit, Vector3 start)
    {
        width = w;
        height = h;
        depth = d;
        unitSize = unit;
        grid = new int[w * h * d];
        filledCount = 0;
        this.start = start;
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

    public void SetVoxelValue(int x, int y, int z, int val)
    {        
        grid[x + width * (y + height * z)] = val;
    }

    public int GetVoxelValue(int x, int y, int z)
    {
        return grid[x + width * (y + height * z)];
    }

    public int GetFilledVoxelCount()
    {
        return filledCount;
    }

    public int _GetFilledVoxelCount()
    {
        int result = 0;

        for (int i = 0; i < grid.Length; i++)
        {
            if (grid[i] == 1)
            {
                result++;
            }
        }

        return result;
    }    

    public void InitGrid()
    {
        if (grid.Length == 0)
        {
            grid = new int[width * height * depth];
        }

        gridBuffer = new ComputeBuffer(width * height * depth, sizeof(int));

        gridBuffer.SetData(grid);
    }

    public void RetrieveGrid()
    {
        if (grid.Length == 0)
        {
            grid = new int[width * height * depth];
        }

        gridBuffer.GetData(grid);
    }

    public void Dispose()
    {
        if (gridBuffer.IsValid())
        {
            gridBuffer.Release();
        }
    }

    public Vector3 GetVolumeSize()
    {
        return new Vector3(unitSize, unitSize, unitSize);
    }
}
