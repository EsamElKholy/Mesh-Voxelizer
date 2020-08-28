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

    public Material geomMat;

    string voxelCount = "VoxelCount";
    string maxDepth = "MaxDepth";
    string maxSize = "MaxSize";
    string vertexBuffer = "VertexBuffer";
    string indexBuffer = "IndexBuffer";
    string indexCount = "IndexCount";
    string voxelOctreeBuffer = "VoxelOctreeBuffer";
    string fillTreeKernel = "FillTree";
    string filledVoxelPositionsBuffer = "FilledVoxelPositionsBuffer";
    string filledVoxelsCount = "FilledVoxelsCount";
    string getFilledVoxelsKernel = "GetFilledVoxels";
    string outVertexBuffer = "OutVerticesBuffer";
    string outIndexBuffer = "OutIndicesBuffer";
    string _filledVoxelPositionsBuffer = "_FilledVoxelPositionsBuffer";
    string constructMeshKernel = "ConstructMesh";
    string constructMeshKernel1 = "ConstructMesh1";
    string buildTreeKernel = "BuildTree";

    public int resolution = 16;
    [Range(1, 8)]
    public int voxelTreeDepth = 1;
    public ComputeShader voxelShader;
    VoxelOctree2 voxelOctree2;
    int currentDepth = 0;

    // Start is called before the first frame update
    void Start()
    {
        Voxelize();
        currentDepth = voxelTreeDepth;
    }
    private void Update()
    {
        if (currentDepth != voxelTreeDepth)
        {
            currentDepth = voxelTreeDepth;
            Destroy(transform.GetChild(0).gameObject);
            Voxelize();
        }
    }

    public void Voxelize()
    {    
        MeshFilter filter = GetComponent<MeshFilter>();

        float maxSize1 = Mathf.Max(filter.mesh.bounds.size.x, filter.mesh.bounds.size.y, filter.mesh.bounds.size.z);
        voxelOctree2 = new VoxelOctree2(filter.mesh.bounds.center, maxSize1*1, voxelTreeDepth);

        var voxelBuffer = new ComputeBuffer(voxelOctree2.Nodes.Length, Marshal.SizeOf(typeof(Node)));
        voxelBuffer.SetData(voxelOctree2.Nodes);

        voxelShader.SetFloat(maxSize, voxelOctree2.MaxSize);
        voxelShader.SetInt(maxDepth, voxelOctree2.MaxDepth);
        voxelShader.SetInt(voxelCount, voxelOctree2.Nodes.Length);

        voxelShader.SetBuffer(voxelShader.FindKernel(buildTreeKernel), voxelOctreeBuffer, voxelBuffer);
        voxelShader.Dispatch(voxelShader.FindKernel(buildTreeKernel), voxelOctree2.Nodes.Length/64 + 1, 1, 1);

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

        voxelShader.SetBuffer(voxelShader.FindKernel(fillTreeKernel), vertexBuffer, vertBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(fillTreeKernel), indexBuffer, indBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(fillTreeKernel), voxelOctreeBuffer, voxelBuffer);
        voxelShader.Dispatch(voxelShader.FindKernel(fillTreeKernel), indCount, 1, 1);
     

        var filledVoxelsBuffer = new ComputeBuffer(voxelOctree2.NodeCount, Marshal.SizeOf(typeof(Node)), ComputeBufferType.Append);
        filledVoxelsBuffer.SetCounterValue(0);
        voxelShader.SetBuffer(voxelShader.FindKernel(getFilledVoxelsKernel), voxelOctreeBuffer, voxelBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(getFilledVoxelsKernel), filledVoxelPositionsBuffer, filledVoxelsBuffer);
        voxelShader.Dispatch(voxelShader.FindKernel(getFilledVoxelsKernel), voxelOctree2.NodeCount, 1, 1);

        int[] counter = new int[1] { 0 };
        ComputeBuffer appendBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.IndirectArguments);
        appendBuffer.SetData(counter);
        ComputeBuffer.CopyCount(filledVoxelsBuffer, appendBuffer, 0);
        appendBuffer.GetData(counter);
        Node[] nodes = new Node[counter[0]];
        filledVoxelsBuffer.GetData(nodes);

        ComputeBuffer filledVoxels = new ComputeBuffer(counter[0], Marshal.SizeOf(typeof(Node)));
        filledVoxels.SetData(nodes);
        //filledVoxelsBuffer.Dispose();

        indBuffer = new ComputeBuffer(counter[0] * 3, sizeof(int));
        var ind = new int[counter[0] * 3];
        //indBuffer1.SetData(ind);
        vertBuffer = new ComputeBuffer(counter[0], Marshal.SizeOf(typeof(Vector3)));
        var v = new Vector3[counter[0]];
        //vertBuffer1.SetData(v);
        voxelShader.SetInt(filledVoxelsCount, counter[0]);
        voxelShader.SetBuffer(voxelShader.FindKernel(constructMeshKernel1), outVertexBuffer, vertBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(constructMeshKernel1), outIndexBuffer, indBuffer);
        voxelShader.SetBuffer(voxelShader.FindKernel(constructMeshKernel1), _filledVoxelPositionsBuffer, filledVoxels);
        voxelShader.Dispatch(voxelShader.FindKernel(constructMeshKernel1), counter[0], 1, 1);
        //var nodes1 = voxelOctree2.GetFilledNodes();

        //MeshFactory.CreateVoxelObject(name + "_Voxelized1", nodes);

        GameObject go = new GameObject(name + "_Voxelized1");
        go.AddComponent<Deform>();
        go.transform.SetParent(transform);

        GetComponent<Renderer>().enabled = false;

        var filter1 = go.AddComponent<MeshFilter>();
        var renderer1 = go.AddComponent<MeshRenderer>();
        renderer1.material = geomMat;
        geomMat.SetFloat("_VoxelSize", voxelOctree2.MaxSize / Mathf.Pow(2, voxelOctree2.MaxDepth));
        filter1.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        filter1.mesh.Clear();
        vertBuffer.GetData(v);
        indBuffer.GetData(ind);
        filter1.mesh.vertices = v;
        filter1.mesh.triangles = ind;
        filter1.mesh.Optimize();
        filter1.mesh.RecalculateNormals();
        filter1.GetComponent<Renderer>().bounds.Expand(1000);
        filledVoxels.Dispose();
        filledVoxelsBuffer.Dispose();
        indBuffer.Dispose();
        vertBuffer.Dispose();
        voxelBuffer.Dispose();
    }

    //private void OnDrawGizmos()
    //{
    //    //if (voxelOctree2 != null)
    //    //{
    //    //    voxelOctree2.DrawTree();
    //    //}
    //}
}
