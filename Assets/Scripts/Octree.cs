using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct Node
{
    public int Index;
    public int CurrentDepth;
    public int FirstChild;
    public int Parent;
    public int FirstLeaf;
    public int LastLeaf;

    public Vector3 Position;
    public float Size;
    private int _value;

    public Node(int index, int currentDepth, int firstChild, int parent, int firstLeaf, int lastLeaf, Vector3 position, float size, int value)
    {
        Index = index;
        CurrentDepth = currentDepth;
        FirstChild = firstChild;
        Parent = parent;
        FirstLeaf = firstLeaf;
        LastLeaf = lastLeaf;
        Position = position;
        Size = size;
        _value = value;
    }

    public int Value { get => _value; set => this._value = value; }
}

public class VoxelOctree
{
    public Node[] Nodes;
    public List<Node> FilledNodes = new List<Node>();
    public List<Particle> particles = new List<Particle>();
    public int MaxDepth;
    public float MaxSize;
    public int NodeCount;
    public ParticleForceRegistery forceRegistery;

    public VoxelOctree(Vector3 position, float size, int maxDepth)
    {
        forceRegistery = new ParticleForceRegistery();

        MaxDepth = maxDepth;
        int s = 0;
        for (int k = 0; k <= MaxDepth; k++)
        {
            s += (int)Mathf.Pow(8, k);
        }
        NodeCount = s;
        Nodes = new Node[NodeCount];

        Node root = new Node()
        {
            Position = position,
            Size = size,
            Value = 0,
            Index = 0,
            CurrentDepth = 0,
            FirstChild = -1,
            Parent = -1,
            FirstLeaf = 0,
            LastLeaf = 0
        };

        MaxSize = size;
        if (maxDepth > 0)
        {
            int sum = 0;
            for (int k = 0; k <= MaxDepth - 1; k++)
            {
                sum += (int)Mathf.Pow(8, k);
            }
            root.FirstLeaf = sum;
            int stride = (int)Mathf.Pow(8, MaxDepth - root.CurrentDepth);
            root.LastLeaf = root.FirstLeaf + stride - 1;
            root.FirstChild = 1;
        }
        else
        {
            root.FirstLeaf = 0;
            root.LastLeaf = 0;
            root.FirstChild = -1;
        }
        
        Nodes[0] = (root);


        //BuildFullTree();
    }

    public void BuildFullTree()
    {
        Node root = Nodes[0];
        Nodes = new Node[NodeCount];
        Nodes[0] = (root);
        //int current = 1;     

        for (int i = 1; i < Nodes.Length; i++)
        {
            int a = 1;
            int b = 0;
            int index = i;
            int currentDepth = 0;
            for (int j = 1; j <= MaxDepth; j++)
            {
                a += (int)Mathf.Pow(8, j);
                b += (int)Mathf.Pow(8, j - 1);

                if (index >= b && index < a)
                {
                    currentDepth = j;
                    break;
                }
            }

            int range = (int)Mathf.Pow(8, currentDepth);

            Node node = new Node();
            node.Index = i;
            node.Parent = (i - 1) / 8;
            node.CurrentDepth = currentDepth;

            if (currentDepth == MaxDepth)
            {
                node.FirstChild = -1;
                node.FirstLeaf = -1;
                node.LastLeaf = -1;
            }
            else
            {
                int previousIndex = 0;

                previousIndex = 0;
                int s = 0;
                for (int k = 0; k < node.CurrentDepth; k++)
                {
                    s += (int)Mathf.Pow(8, k);
                }
                previousIndex = s;

                node.FirstChild = range + 1 + ((node.Index - previousIndex) * 8);
                int stride = (int)Mathf.Pow(8, MaxDepth - node.CurrentDepth);
                node.FirstLeaf = 0;
                int sum = 0;
                for (int k = 0; k <= MaxDepth - 1; k++)
                {
                    sum += (int)Mathf.Pow(8, k);
                }
                sum += (node.Index - previousIndex) * (int)Mathf.Pow(8, MaxDepth - node.CurrentDepth);
                node.FirstLeaf = sum;
                node.LastLeaf = node.FirstLeaf + stride - 1;
            }                     

            node.Size = MaxSize / Mathf.Pow(2, currentDepth);
            node.Position = Nodes[node.Parent].Position;

            if (((i % 8) & 4) == 4)
            {
                node.Position.y += node.Size / 2;
            }
            else
            {
                node.Position.y -= node.Size / 2;
            }

            if (((i % 8) & 2) == 2)
            {
                node.Position.x += node.Size / 2;
            }
            else
            {
                node.Position.x -= node.Size / 2;
            }

            if (((i % 8) & 1) == 1)
            {
                node.Position.z += node.Size / 2;
            }
            else
            {
                node.Position.z -= node.Size / 2;
            }

            node.Value = 0;

            Nodes[i] = (node);
        }
    }

    public List<Node> CheckRay(Ray ray, Transform transform)
    {
        List<Node> hitNodes = new List<Node>();
        int min = 0;

        for (int i = 0; i <= MaxDepth - 1; i++)
        {
            min += (int)Mathf.Pow(8, i);
        }

        for (int i = min; i < Nodes.Length;)
        {
            int parentIndex = Nodes[i].Parent;
            int lastSkip = 0;
            while (parentIndex != -1)
            {
                var parent = Nodes[parentIndex];
                Bounds bounds = new Bounds(transform.TransformPoint(parent.Position), Vector3.one * parent.Size);
                int intersecting = bounds.IntersectRay(ray) ? 1 : 0;

                if (intersecting == 1)
                {
                    break;
                }
                else
                {
                    lastSkip = parent.LastLeaf;
                    parentIndex = parent.Parent;
                }
            }

            if (lastSkip == 0)
            {
                for (int j = i; j < i + 8; j++)
                {
                    Bounds bounds = new Bounds(transform.TransformPoint(Nodes[j].Position), Vector3.one * Nodes[j].Size);
                    float distance = 0;
                    int intersecting = bounds.IntersectRay(ray, out distance) ? 1 : 0;
                    if (intersecting == 1)
                    {
                        var node = Nodes[j];

                        if (node.Value > 0 && distance > node.Size * 1.5f)
                        {
                            hitNodes.Add(node);
                        }
                    }
                }

                i += 8;
            }
            else
            {
                i = lastSkip + 1;
            }
        }

        return hitNodes;
    }

    public List<Node> CastSphere(Vector3 center, float raduis) 
    {
        List<Node> nodes = new List<Node>();

        float maxDistance = raduis + FilledNodes[0].Size / 2;

        for (int i = 0; i < FilledNodes.Count; i++)
        {
            float distance = Vector3.Distance(center, FilledNodes[i].Position);

            if (distance <= maxDistance)
            {
                nodes.Add(FilledNodes[i]);
            }
        }

        return nodes;
    }

    public List<Node> GetNode(Vector3 position) 
    {
        List<Node> result = new List<Node>();        

        int min = 0;

        for (int i = 0; i <= MaxDepth - 1; i++)
        {
            min += (int)Mathf.Pow(8, i);
        }

        for (int i = min; i < Nodes.Length;)
        {
            int parentIndex = Nodes[i].Parent;
            int lastSkip = 0;
            while (parentIndex != -1)
            {
                var parent = Nodes[parentIndex];
                Bounds bounds = new Bounds(parent.Position, Vector3.one * parent.Size);
                int intersecting = bounds.Contains(position) ? 1 : 0;

                if (intersecting == 1)
                {
                    break;
                }
                else
                {
                    lastSkip = parent.LastLeaf;
                    parentIndex = parent.Parent;
                }
            }

            if (lastSkip == 0)
            {
                for (int j = i; j < i + 8; j++)
                {
                    Bounds bounds = new Bounds(Nodes[j].Position, Vector3.one * Nodes[j].Size);
                   
                    int intersecting = bounds.Contains(position) ? 1 : 0;
                    if (intersecting == 1)
                    {
                        var node = Nodes[j];

                        if (node.Value == 0)
                        {                            
                            {
                                result.Add(node);
                            }
                        }
                    }
                }

                i += 8;
            }
            else
            {
                i = lastSkip + 1;
            }
        }

        return result;
    }

    public void CheckTriangles(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        int min = 0;

        for (int i = 0; i <= MaxDepth - 1; i++)
        {
            min += (int)Mathf.Pow(8, i);
        }

        for (int i = min; i < Nodes.Length;)
        {
            int parentIndex = Nodes[i].Parent;
            int lastSkip = 0;
            while (parentIndex != -1)
            {
                var parent = Nodes[parentIndex];
                int intersecting = GeometryUtils.TriangleBoxIntersection(new Vector3[] { v0, v1, v2 }, parent.Position, Vector3.one * parent.Size);

                if (intersecting == 1)
                {
                    break;
                }
                else
                {
                    lastSkip = parent.LastLeaf;
                    parentIndex = parent.Parent;
                }
            }

            if (lastSkip == 0)
            {
                for (int j = i; j < i + 8; j++)
                {
                    int intersecting = GeometryUtils.TriangleBoxIntersection(new Vector3[] { v0, v1, v2 }, Nodes[j].Position, Vector3.one * Nodes[j].Size);

                    if (intersecting == 1)
                    {
                        var node = Nodes[j];
                        if (node.Value == 0)
                        {
                            node.Value = 1;
                            FilledNodes.Add(node);
                        }

                        node.Value = 1;
                        Nodes[j] = node;
                    }
                }

                i += 8;
            }
            else
            {
                i = lastSkip + 1;
            }
        }
    }

    public List<Node> GetFilledNodes(bool recalculate = false)
    {
        if (recalculate)
        {
            List<Node> filledNodes = new List<Node>();
            for (int i = 0; i < Nodes.Length; i++)
            {
                if (Nodes[i].Value == 1)
                {
                    filledNodes.Add(Nodes[i]);
                }
            }

            FilledNodes = filledNodes;

            return FilledNodes;
        }

        return FilledNodes;
    }

    public void SetParticles()
    {
        particles = new List<Particle>();
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (Nodes[i].Value == 1)
            {
                Particle particle = new Particle(Nodes[i].Position, Random.Range(10, 25));
                particle.Damping = 0.95f;
                //AnchorSpring anchor = new AnchorSpring(Nodes[i].Position, 20, Nodes[i].Position.magnitude);
                //forceRegistery.Add(ref particle, anchor);
                ParticleGravity gravity = new ParticleGravity(new Vector3(Random.Range(-4f, 4f), -5f, Random.Range(-4f, 4f)));
                forceRegistery.Add(ref particle, gravity);
                //ParticleDrag drag = new ParticleDrag(1.05f * 35, 0);
                //forceRegistery.Add(ref particle, drag);

                Buoyancy buoyancy = new Buoyancy(2.65f, 3, 0, 1.6f);
                forceRegistery.Add(ref particle, buoyancy);

                particles.Add(particle);
            }
        }
    }

    public void UpdateParticles(float dt)
    {
        GetFilledNodes();

        forceRegistery.UpdateForces(dt);

        for (int i = 0; i < particles.Count; i++)
        {
            //particles[i].AddForce(new Vector3(Random.Range(-15, 15), 2, Random.Range(-15, 15)));
            //particles[i].AddForce(new Vector3(0, -5, 0));

            particles[i].Integrate(dt);
            var node = FilledNodes[i];
            node.Position = particles[i].Position;
            FilledNodes[i] = node;
        }
    }

    public void DrawTree()
    {
        Color minColor = new Color(1, 1, 1, 1f);
        Color maxColor = new Color(0, 0.5f, 1, 0.25f);

        for (int i = 0; i < Nodes.Length; i++)
        {
            Node node = Nodes[i];

            Gizmos.color = Color.Lerp(minColor, maxColor, node.CurrentDepth / (float)MaxDepth);
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        }
    }

    public int GetNodeSize()
    {
        int size = 0;
        size += sizeof(int) * 7;
        size += sizeof(float);
        size += Marshal.SizeOf(typeof(Vector3));

        return size;
    }
}
/*
 
    node ->
        index -> node buffer
        current depth
        index -> first child -> -1 if none
        position
        size
        value -> filled or not filled
        
    root -> 
        index = 0
        current depth = 0
        first child index = -1
                                        queue->
                                            root

    while queue !empty
        pop
        check intersetion 
            false -> continue
            true ->
                has children?
                    false ->
                        leaf? || max depth
                            false ->
                                first child = node buffer size
                                add 8 new nodes to node buffer
                                each child's depth = current depth +1
                                add the 8 children to the queue 
                            true ->
                                pos buffer [index] = filled
                    true ->
                        add the 8 children to queue


    
     
     
     
     0 -> root
     1 -> 8 children
     2 -> 8 * 8 children
     

    ////Construction////
     current = 1
     for i = 1 to i = max depth
        current depth = i
        range = 8^(current depth)
        for j = current to j = range
            node = new node()
            node.Index = j
            node.Parent = j / 8
            i == max depth?
                true ->
                    node.FirstChild = -1
                    FirstLeaf=-1
                    LastLeaf =-1
                false ->
                    FirstLeaf=index*8^currentDepth + 1
                    LastLeaf =first leaf + 8^(maxDepth - currentDepth)
                    node.FirstChild = range + j*8
            node.CurrentDepth = i
            node.size = 2^i
            node.pos = Parent.Pos
                if (j % 8) & 4 == 4
                    node.pos.y += node.size / 2
                else
                    node.pos.y -= node.size / 2
                if (j % 8) & 2 == 2
                    node.pos.x += node.size / 2
                else
                    node.pos.x -= node.size / 2
                if (j % 8) & 1 == 1
                    node.pos.z += node.size / 2
                else
                    node.pos.z -= node.size / 2
            current++
     ////Traversal////
     for i=8^(maxDepth-1)+1 to i=8^maxDepth; i+=8
        parentIndex=parent
        lastskip = 0
        while parentIndex != -1
            parentNode=nodes[parentIndex]
            intersecting with parent?
            true?
                break
            false
                lastskip = parentNode.lastchild
                parentIndex = parent.parent
        
        lastskip == 0:
            true ->
                for j = i to j=i+8
                    check intersection with each node 
            false->
                i+=lastskip            
     
     
     
     
     
    if index = 0
        previous index = 0
    else if node[index - 1].current != current
        previous index = index
    else 
        previous index = index - 1
    
    range = 8^current

    first child = range + 1 + (index - previous index) * 8

    first leaf :
        sum = 0
        for i = 0 to i = max-1
            sum += 8^i
        sum += (index - previous index) * 8^(max - current)
        first leaf = sum

    stride = 8^(max - current)
    last = first + stride
     
     
    
^    0->     0                                                                                                         1
|    1->     1                          2                           3                                                  8 + 1
|    2->     9 10 11 12 13 14 15 16     17 18 19 20 21 22 23 24     25 26 27 28 29 30 31 32                            64 + 8 + 1
     3->     64+8+1                                                                                                    512 + 64 + 8 + 1
     



     */
