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
    string maxDepth = "MaxDepth";
    string vertexBuffer = "VertexBuffer";
    string indexBuffer = "IndexBuffer";
    string indexCount = "IndexCount";
    string voxelOctreeBuffer = "VoxelOctreeBuffer";
    string fillTreeKernel = "FillTree";

    public int resolution = 16;
    public int voxelTreeDepth = 1;
    public ComputeShader voxelShader;
    Octree octree;
    VoxelOctree voxelOctree;
    VoxelOctree2 voxelOctree2;

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
        //voxelOctree2 = CPUVoxelizer.BuildVoxelTreeS2(gameObject, voxelTreeDepth);
        //var nodes = voxelOctree2.GetFilledNodes();
        //MeshFactory.CreateVoxelObject(name + "_Voxelized1", nodes);

        MeshFilter filter = GetComponent<MeshFilter>();
        float maxSize = Mathf.Max(filter.mesh.bounds.size.x, filter.mesh.bounds.size.y, filter.mesh.bounds.size.z);
        voxelOctree2 = new VoxelOctree2(filter.mesh.bounds.center, maxSize, voxelTreeDepth);

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

        voxelShader.SetInt(indexCount, indCount);
        voxelShader.SetInt(voxelCount, voxelOctree2.Nodes.Length);
        voxelShader.SetInt(maxDepth, voxelOctree2.MaxDepth);

        var voxelBuffer = new ComputeBuffer(voxelOctree2.Nodes.Length, Marshal.SizeOf(typeof(Node)));
        voxelBuffer.SetData(voxelOctree2.Nodes);

        voxelShader.SetBuffer(voxelShader.FindKernel(fillTreeKernel), vertexBuffer, vertBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(fillTreeKernel), indexBuffer, indBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(fillTreeKernel), voxelOctreeBuffer, voxelBuffer);
        voxelShader.Dispatch(voxelShader.FindKernel(fillTreeKernel), indCount, 1, 1);
        voxelBuffer.GetData(voxelOctree2.Nodes);
        var nodes1 = voxelOctree2.GetFilledNodes();

        MeshFactory.CreateVoxelObject(name + "_Voxelized1", nodes1);
    }

    private void OnDrawGizmos()
    {
        //if (voxelOctree2 != null)
        //{
        //    voxelOctree2.DrawTree();
        //}
    }
}
