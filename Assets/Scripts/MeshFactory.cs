using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFactory : MonoBehaviour
{
    private static void CreateVoxelMesh(ref MeshFilter filter, VoxelVolume volume)
    {
        if (filter == null)
        {
            return;
        }

        int filledCount = volume._GetFilledVoxelCount();

        Vector3[] verts = new Vector3[(filledCount * 24)];
        int[] inds = new int[(filledCount * 36)];

        int count = 0;

        for (int x = 0; x < volume.width; x++)
        {
            for (int y = 0; y < volume.height; y++)
            {
                for (int z = 0; z < volume.depth; z++)
                {
                    if (volume.GetVoxelValue(x, y, z) == 1)
                    {
                        Vector3 pos = volume.GetCoordinate(x, y, z);
                        Vector3 size = volume.GetVolumeSize();

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
                }
            }
        }
        filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        filter.mesh.Clear();
        filter.mesh.vertices = verts;
        filter.mesh.triangles = inds;
        filter.mesh.Optimize();
        filter.mesh.RecalculateNormals();
        //mf.mesh.RecalculateBounds();
        //mf.mesh.RecalculateTangents();
    }

    private static void CreateVoxelMesh(ref MeshFilter filter, List<VoxelOctree.Node> nodes)
    {
        if (filter == null)
        {
            return;
        }

        Vector3[] verts = new Vector3[(nodes.Count * 24)];
        int[] inds = new int[(nodes.Count * 36)];

        for (int count = 0; count < nodes.Count; count++)
        {
            Vector3 pos = nodes[count].Position;
            Vector3 size = nodes[count].Size * Vector3.one;

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
        }

     
        filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        filter.mesh.Clear();
        filter.mesh.vertices = verts;
        filter.mesh.triangles = inds;
        filter.mesh.Optimize();
        filter.mesh.RecalculateNormals();
        //mf.mesh.RecalculateBounds();
        //mf.mesh.RecalculateTangents();
    }

    private static void CreateVoxelMesh(ref MeshFilter filter, List<Node> nodes)
    {
        if (filter == null)
        {
            return;
        }

        Vector3[] verts = new Vector3[(nodes.Count * 24)];
        int[] inds = new int[(nodes.Count * 36)];

        for (int count = 0; count < nodes.Count; count++)
        {
            Vector3 pos = nodes[count].Position;
            Vector3 size = nodes[count].Size * Vector3.one;

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
        }


        filter.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        filter.mesh.Clear();
        filter.mesh.vertices = verts;
        filter.mesh.triangles = inds;
        filter.mesh.Optimize();
        filter.mesh.RecalculateNormals();
        //mf.mesh.RecalculateBounds();
        //mf.mesh.RecalculateTangents();
    }

    public static GameObject CreateVoxelObject(string name, VoxelVolume volume)
    {
        GameObject go = new GameObject(name);

        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Diffuse"));

        CreateVoxelMesh(ref filter, volume);

        return go;
    }

    public static GameObject CreateVoxelObject(string name, List<VoxelOctree.Node> nodes)
    {
        GameObject go = new GameObject(name);

        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Diffuse"));

        CreateVoxelMesh(ref filter, nodes);

        return go;
    }

    public static GameObject CreateVoxelObject(string name, List<Node> nodes)
    {
        GameObject go = new GameObject(name);

        var filter = go.AddComponent<MeshFilter>();
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Diffuse"));

        CreateVoxelMesh(ref filter, nodes);

        return go;
    }
}
