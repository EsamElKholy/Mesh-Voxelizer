using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Octree
{
    public class Node
    {
        public Node Parent;

        public Vector3 Position;
        public Node[] Children;
        public float Size;
        public int CurrentDepth;
        public int MaxDepth;
        public bool Filled;

        public Node(Vector3 pos, float size, int depth, int maxDepth, Node parent)
        {
            Position = pos;
            Size = size;
            Parent = parent;
            CurrentDepth = depth;
            Filled = false;
            MaxDepth = maxDepth;

            //if (depth < maxDepth)
            //{
            //    Subdivide();
            //}
        }

        public void Subdivide()
        {
            Children = new Node[8];

            Bounds bounds = new Bounds(Position, Size * Vector3.one);
            float halfUnit = Size / 2;

            var start = bounds.min + new Vector3(halfUnit / 2, halfUnit / 2, halfUnit / 2);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        Vector3 pos = start + new Vector3(i, j, k) * Size / 2;

                        Children[i + 2 * (j + 2 * k)] = new Node(pos, Size / 2, CurrentDepth + 1, MaxDepth, this);
                    }
                }
            }
        }

        public bool IsLeaf()
        {
            return Children == null;
        }

        public void CheckTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            if (!Filled)
            {
                int isIntersecting = GeometryUtils.TriangleBoxIntersection(new Vector3[] { v0, v1, v2 }, Position, new Vector3(Size, Size, Size));
                
                if (isIntersecting == 1 && CurrentDepth != MaxDepth)
                {
                    if (IsLeaf())
                    {
                        Subdivide();
                    }

                    for (int i = 0; i < Children.Length; i++)
                    {
                        Children[i].CheckTriangle(v0, v1, v2);
                    }
                }

                if (isIntersecting == 1 && CurrentDepth == MaxDepth)
                {
                    Filled = true;
                }
            }            
        }

        public void GetFilledNodes(ref List<Node> nodes)
        {
            if (Filled && IsLeaf())
            {
                nodes.Add(this);
            }
            else
            {
                if (!IsLeaf())
                {
                    for (int i = 0; i < Children.Length; i++)
                    {
                        Children[i].GetFilledNodes(ref nodes);
                    }
                }
            }
        }

        public void GetAllNodes(ref List<Node> nodes)
        {
            if (IsLeaf())
            {
                nodes.Add(this);
            }
            else
            {
                if (!IsLeaf())
                {
                    for (int i = 0; i < Children.Length; i++)
                    {
                        Children[i].GetAllNodes(ref nodes);
                    }
                }
            }
        }
    }

    public Node Root;
    public int MaxDepth;

    public Octree(Vector3 pos, float size, int maxDepth)
    {
        Root = new Node(pos, size, 0, maxDepth, null);
        MaxDepth = maxDepth;
    }

    public void CheckTriangle(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Root.CheckTriangle(v0, v1, v2);
    }

    public List<Node> GetFilledNodes()
    {
        List<Node> filledNodes = new List<Node>();

        Root.GetFilledNodes(ref filledNodes);

        return filledNodes;
    }

    public List<Node> GetAllNodes()
    {
        List<Node> allNodes = new List<Node>();

        Root.GetAllNodes(ref allNodes);

        return allNodes;
    }

    ////////OCTREE VISUALIZATION//////
    public void DrawGizmos()
    {
        DrawNode(Root);
    }

    private Color minColor = new Color(1, 1, 1, 1f);
    private Color maxColor = new Color(0, 0.5f, 1, 0.25f);

    private void DrawNode(Node node, int nodeDepth = 0)
    {
        if (!node.IsLeaf())
        {
            foreach (var subnode in node.Children)
            {
                DrawNode(subnode, nodeDepth + 1);
            }
        }
        Gizmos.color = Color.Lerp(minColor, maxColor, nodeDepth / (float)MaxDepth);
        Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
    }
}

public class VoxelOctree
{
    public class Node
    {
        public int Index;
        public int CurrentDepth;
        public int FirstChild;

        public Vector3 Position;
        public float Size;
        public int Value;
    }

    public List<Node> Nodes = new List<Node>();
    public List<Node> FilledNodes = new List<Node>();
    public int MaxDepth;

    public VoxelOctree(Vector3 position, float size, int maxDepth)
    {
        Node root = new Node() { Position = position, Size = size, Value = 0, Index = 0, CurrentDepth = 0, FirstChild = -1};
        Nodes.Add(root);

        MaxDepth = maxDepth;

        BuildFullTree();
    }

    public void BuildFullTree()
    {
        Node root = Nodes[0];
        Nodes = new List<Node>();
        Nodes.Add(root);

        Queue<int> nodes = new Queue<int>();
        nodes.Enqueue(0);
        
        while (nodes.Count > 0)
        {
            var node = Nodes[nodes.Dequeue()];

            if (node.CurrentDepth == MaxDepth)
            {
                continue;
            }
            else
            {
                if (node.FirstChild == -1)
                {
                    node.FirstChild = Nodes.Count;
                    Bounds bounds = new Bounds(node.Position, node.Size * Vector3.one);
                    float halfUnit = node.Size / 2;

                    var start = bounds.min + new Vector3(halfUnit / 2, halfUnit / 2, halfUnit / 2);

                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                Vector3 pos = start + new Vector3(i, j, k) * node.Size / 2;

                                Node child = new Node();
                                child.Index = node.FirstChild + i + 2 * (j + 2 * k);
                                child.FirstChild = -1;
                                child.CurrentDepth = node.CurrentDepth + 1;
                                child.Position = pos;
                                child.Size = node.Size / 2;
                                child.Value = 0;

                                Nodes.Add(child);
                                nodes.Enqueue(child.Index);
                            }
                        }
                    }
                }
            }
        }
    }
    int max = -99999999;
    public void CheckTriangles(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Queue<int> nodes = new Queue<int>();
        nodes.Enqueue(0);

        while (nodes.Count > 0)
        {
            if (nodes.Count >= max)
            {
                max = nodes.Count;

            }
            var node = Nodes[nodes.Dequeue()];

            int intersecting = GeometryUtils.TriangleBoxIntersection(new Vector3[] { v0, v1, v2 }, node.Position, node.Size * Vector3.one);

            if (intersecting == 0)
            {
                continue;
            }
            else
            {
                if (node.FirstChild == -1)
                {
                    if (node.CurrentDepth == MaxDepth)
                    {
                        node.Value = 1;
                        FilledNodes.Add(node);
                    }                              
                }
                else
                {
                    for (int i = 0; i < 8; i++)
                    {
                        intersecting = GeometryUtils.TriangleBoxIntersection(new Vector3[] { v0, v1, v2 }, Nodes[Nodes[node.FirstChild + i].Index].Position, Nodes[Nodes[node.FirstChild + i].Index].Size * Vector3.one);
                        if (intersecting == 1)
                        {
                            nodes.Enqueue(Nodes[node.FirstChild + i].Index);
                        }
                    }
                }
            }
        }
    }

    public List<Node> GetFilledNodes()
    {
        //List<Node> filledNodes = new List<Node>();

        //for (int i = 0; i < Nodes.Count; i++)
        //{
        //    if (Nodes[i].Value == 1)
        //    {
        //        filledNodes.Add(Nodes[i]);
        //    }
        //}

        Debug.Log(max);
        return FilledNodes;
    }

    public void DrawTree()
    {
        Color minColor = new Color(1, 1, 1, 1f);
        Color maxColor = new Color(0, 0.5f, 1, 0.25f);

        for (int i = 0; i < Nodes.Count; i++)
        {
            Node node = Nodes[i];

            Gizmos.color = Color.Lerp(minColor, maxColor, node.CurrentDepth / (float)MaxDepth);
            Gizmos.DrawWireCube(node.Position, Vector3.one * node.Size);
        }
    }
}

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

public class VoxelOctree2
{
    public Node[] Nodes;// = new List<Node>();
    public List<Node> FilledNodes = new List<Node>();
    public int MaxDepth;
    public float MaxSize;
    public int NodeCount;

    public VoxelOctree2(Vector3 position, float size, int maxDepth)
    {
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


        BuildFullTree();
    }

    public void BuildFullTree()
    {
        Node root = Nodes[0];
        Nodes = new Node[NodeCount];
        Nodes[0] = (root);
        int current = 1;
        for (int i = 1; i <= MaxDepth; i++)
        {
            int currentDepth = i;
            int range = (int)Mathf.Pow(8, currentDepth);
            int limit = 0;
            for (int k = 0; k <= i; k++)
            {
                limit += (int)Mathf.Pow(8, k);
            }

            for (int j = current; j < limit; j++, current++)
            {
                Node node = new Node();
                node.Index = j;
                node.Parent = (j - 1) / 8;
                node.CurrentDepth = i;

                if (i == MaxDepth)
                {
                    node.FirstChild = -1;
                    node.FirstLeaf = -1;
                    node.LastLeaf = -1;
                }
                else
                {
                    int previousIndex = 0;

                    if (node.Index == 0)
                    {
                        previousIndex = 0;
                    }
                    else if (Nodes[node.Index - 1].CurrentDepth != currentDepth)
                    {
                        previousIndex = node.Index;
                    }
                    else
                    {
                        previousIndex = 0;
                        int s = 0;
                        for (int k = 0; k < currentDepth; k++)
                        {
                            s += (int)Mathf.Pow(8, k);
                        }
                        previousIndex = s;
                    }

                    node.FirstChild = range + 1 + ((node.Index - previousIndex) * 8);
                    int stride = (int)Mathf.Pow(8, MaxDepth - node.CurrentDepth);
                    node.FirstLeaf = 0;
                    int sum = 0;
                    for (int k = 0; k <= MaxDepth - 1; k++)
                    {
                        sum += (int)Mathf.Pow(8, k);
                    }
                    sum += (node.Index - previousIndex) * (int)Mathf.Pow(8, MaxDepth - currentDepth);
                    node.FirstLeaf = sum;
                    node.LastLeaf = node.FirstLeaf + stride - 1;
                }

                node.Size = MaxSize / Mathf.Pow(2, i);
                node.Position = Nodes[node.Parent].Position;

                if (((j % 8) & 4) == 4)
                {
                    node.Position.y += node.Size / 2;
                }
                else
                {
                    node.Position.y -= node.Size / 2;
                }

                if (((j % 8) & 2) == 2)
                {
                    node.Position.x += node.Size / 2;
                }
                else
                {
                    node.Position.x -= node.Size / 2;
                }

                if (((j % 8) & 1) == 1)
                {
                    node.Position.z += node.Size / 2;
                }
                else
                {
                    node.Position.z -= node.Size / 2;
                }

                node.Value = 0;

                Nodes[j] = (node);
            }
        }
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

    public List<Node> GetFilledNodes()
    {
        List<Node> filledNodes = new List<Node>();
        for (int i = 0; i < Nodes.Length; i++)
        {
            if (Nodes[i].Value == 1)
            {
                filledNodes.Add(Nodes[i]);
            }
        }
        return filledNodes;
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
