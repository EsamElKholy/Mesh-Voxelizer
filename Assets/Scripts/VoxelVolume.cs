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
    public int filledCount;
    public int[] gpuFilledCount;
    public ComputeBuffer gridBuffer;
    public ComputeBuffer filledCountBuffer;
    public ComputeBuffer argBuffer;

    public VoxelVolume(int w, int h, int d, float unit, Vector3 start)
    {
        width = w;
        height = h;
        depth = d;
        unitSize = unit;
        grid = new int[w * h * d];
        filledCount = 0;
        gpuFilledCount = new int[] { 0 };
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
        if (GetVoxelValue(x, y, z) == 0 && val == 1)
        {
            filledCount++;
        }

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

        filledCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Counter);
        filledCountBuffer.SetCounterValue(0);
        argBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
    }

    public void RetrieveGrid()
    {
        if (grid.Length == 0)
        {
            grid = new int[width * height * depth];
        }

        gridBuffer.GetData(grid);

        //filledCountBuffer.GetData(filledCount);
    }

    public int RetrieveGPUFilledCount()
    {        
        ComputeBuffer.CopyCount(filledCountBuffer, argBuffer, 0);
        argBuffer.GetData(gpuFilledCount);

        return gpuFilledCount[0];
    }

    public void Dispose()
    {
        if (gridBuffer.IsValid())
        {
            gridBuffer.Release();
        }

        if (filledCountBuffer.IsValid())
        {
            filledCountBuffer.Release();
        }
    }

    public Vector3 GetVolumeSize()
    {
        return new Vector3(unitSize, unitSize, unitSize);
    }

    public int GetTotalVoxelCount()
    {
        return width * height * depth;
    }
}
