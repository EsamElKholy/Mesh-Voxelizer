using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class VoxelManager : MonoBehaviour
{
    public class Kernel
    {
        public int Index { get { return index; } }
        public uint ThreadX { get { return threadX; } }
        public uint ThreadY { get { return threadY; } }
        public uint ThreadZ { get { return threadZ; } }

        int index;
        uint threadX, threadY, threadZ;

        public Kernel(ComputeShader shader, string key)
        {
            index = shader.FindKernel(key);
            if (index < 0)
            {
                Debug.LogWarning("Can't find kernel");
                return;
            }
            shader.GetKernelThreadGroupSizes(index, out threadX, out threadY, out threadZ);
        }
    }

    string voxelCount = "VoxelCount";
    string unitSize = "UnitSize";
    string start = "Start";
    string width = "Width";
    string height = "Height";
    string depth = "Depth";
    string vertexBuffer = "VertexBuffer";
    string indexBuffer = "IndexBuffer";
    string indexCount = "IndexCount";
    string voxels = "Voxels";
    string filledCount = "FilledVoxelCount";
    string fillVolumeKernel = "FillVolume";

    public int resolution = 16;
    public ComputeShader voxelShader;

    // Start is called before the first frame update
    void Start()
    {
        Voxelize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Voxelize()
    {
        //var volume0 = CPUVoxelizer.Voxelize(gameObject, resolution);
        //MeshFactory.CreateVoxelObject(name + "_Voxelized1", volume0);

        var volume = CPUVoxelizer.CreateVoxelVolume(gameObject, resolution);
        volume.InitGrid();

        MeshFilter filter = GetComponent<MeshFilter>();

        var verts = filter.mesh.vertices;
        int vCount = verts.Length;
        int vSize = Marshal.SizeOf(typeof(Vector3));
        var vertBuffer = new ComputeBuffer(vCount, vSize);
        vertBuffer.SetData(verts);

        var inds = filter.mesh.triangles;
        int indCount = inds.Length;
        int indSize = sizeof(int);
        var indBuffer = new ComputeBuffer(indCount, indSize);
        indBuffer.SetData(inds);

        voxelShader.SetInt(voxelCount, volume.width * volume.height * volume.depth);

        voxelShader.SetInt(width, volume.width);
        voxelShader.SetInt(height, volume.height);
        voxelShader.SetInt(depth, volume.depth);

        voxelShader.SetFloat(unitSize, volume.unitSize);

        voxelShader.SetVector(start, volume.start);

        voxelShader.SetBuffer(voxelShader.FindKernel(fillVolumeKernel), vertexBuffer, vertBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(fillVolumeKernel), indexBuffer, indBuffer);
        voxelShader.SetInt(indexCount, indCount);

        voxelShader.SetBuffer(voxelShader.FindKernel(fillVolumeKernel), voxels, volume.gridBuffer);

        voxelShader.Dispatch(voxelShader.FindKernel(fillVolumeKernel), volume.width, volume.height, volume.depth);

        volume.RetrieveGrid();
        MeshFactory.CreateVoxelObject(name + "_Voxelized", volume);
    }
}
