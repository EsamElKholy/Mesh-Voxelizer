using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    public class VoxelVolume
    {
        public int width, height, depth;
        public float unitSize;
        public Vector3 start;
        public bool[] grid;
        public Vector3 currentPos;
        public Vector3 currentXYZ;
        public Vector3 voxelSize;

        public VoxelVolume(int w, int h, int d, float unit)
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

    private MeshFilter meshFilter;
    private Bounds bounds;
    public float resolution = 16;

    private VoxelVolume volume;

    // Start is called before the first frame update
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        bounds = meshFilter.sharedMesh.bounds;

        Voxelize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Voxelize()
    {
        meshFilter.mesh.RecalculateBounds();

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

        volume = new VoxelVolume(xCount, yCount, zCount, unit);
        volume.start = start;

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
                        if (TriangleBoxIntersection(triangle, volume.GetCoordinate(x, y, z), volume.voxelSize) == true)
                        {
                            volume.SetVoxelValue(x, y, z, true);
                        }
                    }
                }
            }
        }

        CreateMesh(unit);
    }

    private void CreateMesh(float unitSize)
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        int filledCount = volume.GetFilledVoxelCount();

        Vector3[] verts = new Vector3[(filledCount * 24)];      
        int[] inds = new int[(filledCount * 36)];

        int count = 0;
        int count1 = 0;

        for (int x = 0; x < volume.width; x++)
        {
            for (int y = 0; y < volume.height; y++)
            {
                for (int z = 0; z < volume.depth; z++)
                {
                    if (volume.GetVoxelValue(x, y, z))
                    {
                        Vector3 pos = volume.GetCoordinate(x, y, z);
                        Vector3 size = volume.voxelSize;

                        // Face: UP             
                        verts[0 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[1 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[2 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[3 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z - (size.z / 2.0f)));

                        // Face: DOWN                                                                                      
                        verts[4 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[5 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[6 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[7 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z - (size.z / 2.0f)));

                        // Face: BACK                                                                                      
                        verts[8 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[9 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[10 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[11 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z - (size.z / 2.0f)));

                        // Face: FRONT                                                                                     
                        verts[12 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[13 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[14 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[15 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z + (size.z / 2.0f)));

                        // Face: LEFT                                                                                      
                        verts[16 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[17 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[18 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[19 + 24 * count] = (new Vector3(pos.x - (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z + (size.z / 2.0f)));

                        // Face: RIGHT                                                                                    
                        verts[20 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z + (size.z / 2.0f)));
                        verts[21 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y - (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[22 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z - (size.z / 2.0f)));
                        verts[23 + 24 * count] = (new Vector3(pos.x + (size.x / 2.0f), pos.y + (size.y / 2.0f), pos.z + (size.z / 2.0f)));

                        //Up
                        inds[0 + 36 * count] = ((0 + (4 * 0)) + (24 * (count)));
                        inds[1 + 36 * count] = ((1 + (4 * 0)) + (24 * (count)));
                        inds[2 + 36 * count] = ((2 + (4 * 0)) + (24 * (count)));

                        inds[3 + 36 * count] = ((0 + (4 * 0)) + (24 * (count)));
                        inds[4 + 36 * count] = ((2 + (4 * 0)) + (24 * (count)));
                        inds[5 + 36 * count] = ((3 + (4 * 0)) + (24 * (count)));

                        //Down                          
                        inds[6 + 36 * count] = ((2 + (4 * 1)) + (24 * (count)));
                        inds[7 + 36 * count] = ((1 + (4 * 1)) + (24 * (count)));
                        inds[8 + 36 * count] = ((0 + (4 * 1)) + (24 * (count)));

                        inds[9 + 36 * count] = ((3 + (4 * 1)) + (24 * (count)));
                        inds[10 + 36 * count] = ((2 + (4 * 1)) + (24 * (count)));
                        inds[11 + 36 * count] = ((0 + (4 * 1)) + (24 * (count)));

                        //Back                        
                        inds[12 + 36 * count] = ((2 + (4 * 2)) + (24 * (count)));
                        inds[13 + 36 * count] = ((1 + (4 * 2)) + (24 * (count)));
                        inds[14 + 36 * count] = ((0 + (4 * 2)) + (24 * (count)));

                        inds[15 + 36 * count] = ((3 + (4 * 2)) + (24 * (count)));
                        inds[16 + 36 * count] = ((2 + (4 * 2)) + (24 * (count)));
                        inds[17 + 36 * count] = ((0 + (4 * 2)) + (24 * (count)));

                        //Front                       
                        inds[18 + 36 * count] = ((0 + (4 * 3)) + (24 * (count)));
                        inds[19 + 36 * count] = ((1 + (4 * 3)) + (24 * (count)));
                        inds[20 + 36 * count] = ((2 + (4 * 3)) + (24 * (count)));

                        inds[21 + 36 * count] = ((0 + (4 * 3)) + (24 * (count)));
                        inds[22 + 36 * count] = ((2 + (4 * 3)) + (24 * (count)));
                        inds[23 + 36 * count] = ((3 + (4 * 3)) + (24 * (count)));

                        //Left                          
                        inds[24 + 36 * count] = ((2 + (4 * 4)) + (24 * (count)));
                        inds[25 + 36 * count] = ((1 + (4 * 4)) + (24 * (count)));
                        inds[26 + 36 * count] = ((0 + (4 * 4)) + (24 * (count)));

                        inds[27 + 36 * count] = ((3 + (4 * 4)) + (24 * (count)));
                        inds[28 + 36 * count] = ((2 + (4 * 4)) + (24 * (count)));
                        inds[29 + 36 * count] = ((0 + (4 * 4)) + (24 * (count)));

                        //Right                         
                        inds[30 + 36 * count] = ((0 + (4 * 5)) + (24 * (count)));
                        inds[31 + 36 * count] = ((1 + (4 * 5)) + (24 * (count)));
                        inds[32 + 36 * count] = ((2 + (4 * 5)) + (24 * (count)));

                        inds[33 + 36 * count] = ((0 + (4 * 5)) + (24 * (count)));
                        inds[34 + 36 * count] = ((2 + (4 * 5)) + (24 * (count)));
                        inds[35 + 36 * count] = ((3 + (4 * 5)) + (24 * (count)));

                        count++;
                    }

                    count1++;
                }
            }
        }
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mf.mesh.Clear();
        mf.mesh.vertices = verts;
        mf.mesh.triangles = inds;       
        mf.mesh.Optimize();
        mf.mesh.RecalculateNormals();
        //mf.mesh.RecalculateBounds();
        //mf.mesh.RecalculateTangents();
    }

    private bool TriangleBoxIntersection(Vector3[] triangle, Vector3 boxCenter, Vector3 boxSize)
    {     
        Vector3 triangleEdge0 = triangle[1] - triangle[0];
        Vector3 triangleEdge1 = triangle[2] - triangle[1];
        Vector3 triangleEdge2 = triangle[0] - triangle[2];

        Vector3 triangleNormal = Vector3.Cross(triangleEdge0, triangleEdge1);
        
        Vector3 boxNormal0 = new Vector3(1, 0, 0);
        Vector3 boxNormal1 = new Vector3(0, 1, 0);
        Vector3 boxNormal2 = new Vector3(0, 0, 1);
       
        Vector3[] test =
        {
            boxNormal0,
            boxNormal1,
            boxNormal2,

            triangleNormal,

            //Vector3.Cross(boxNormal0, triangleEdge0),
            //Vector3.Cross(boxNormal0, triangleEdge1),
            //Vector3.Cross(boxNormal0, triangleEdge2),
            //Vector3.Cross(boxNormal1, triangleEdge0),
            //Vector3.Cross(boxNormal1, triangleEdge1),
            //Vector3.Cross(boxNormal1, triangleEdge2),
            //Vector3.Cross(boxNormal2, triangleEdge0),
            //Vector3.Cross(boxNormal2, triangleEdge1),
            //Vector3.Cross(boxNormal2, triangleEdge2),

            new Vector3(0, -triangleEdge0.z, triangleEdge0.y),
            new Vector3(0, -triangleEdge1.z, triangleEdge1.y),
            new Vector3(0, -triangleEdge2.z, triangleEdge2.y),
            new Vector3(triangleEdge0.z, 0, -triangleEdge0.x),
            new Vector3(triangleEdge1.z, 0, -triangleEdge1.x),
            new Vector3(triangleEdge2.z, 0, -triangleEdge2.x),
            new Vector3(-triangleEdge0.y, triangleEdge0.x, 0),
            new Vector3(-triangleEdge1.y, triangleEdge1.x, 0),
            new Vector3(-triangleEdge2.y, triangleEdge2.x, 0)
        };
        
        for (int i = 0; i < test.Length; i++)
        {           
            Vector3 axis = test[i];            

            if (!OverlapAxis(triangle, boxCenter, boxSize, axis))
            {
                return false;
            }
        }
        
        return true;
    }

    private Vector2 GetBoxInterval(Vector3 boxCenter, Vector3 boxSize, Vector3 axis)
    {
        Vector2 interval = Vector2.zero;

        Bounds b = new Bounds(boxCenter, boxSize);

        Vector3 p1 = boxCenter + boxSize ;
        Vector3 p2 = boxCenter - boxSize ;

        Vector3 min = b.min;
        Vector3 max = b.max;

        Vector3[] vertex = 
        {
            new Vector3(min.x, max.y, max.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(min.x, min.y, min.z),
            new Vector3(max.x, max.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(max.x, min.y, min.z)
        };

        float mn = float.MaxValue, mx = float.MinValue;

        for (int i = 0; i < vertex.Length; i++)
        {
            float projection = Vector3.Dot(axis, vertex[i]);

            mn = Mathf.Min(mn, projection);
            mx = Mathf.Max(mx, projection);
        }

        interval.x = mn;
        interval.y = mx;

        return interval;
    }

    private Vector2 GetTriangleInterval(Vector3[] triangle, Vector3 axis)
    {
        Vector2 interval = Vector2.zero;

        float min = float.MaxValue;
        float max = float.MinValue;

        for (int i = 0; i < 3; i++)
        {
            float projection = Vector3.Dot(triangle[i], axis);

            min = Mathf.Min(min, projection);
            max = Mathf.Max(max, projection);
        }       

        interval.x = min;
        interval.y = max;

        return interval;
    }

    private bool OverlapAxis(Vector3[] triangle, Vector3 boxCenter, Vector3 boxSize, Vector3 axis)
    {
        var a = GetBoxInterval(boxCenter, boxSize, axis);
        var b = GetTriangleInterval(triangle, axis);

        return ((b.x <= a.y) && (a.x <= b.y));
    }   
}
