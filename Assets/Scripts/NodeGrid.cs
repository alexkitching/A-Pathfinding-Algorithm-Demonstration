/*
----------------------------------------------------------/
----- Project: AStar Record 							  /
----- File:    NodeGrid.cs					              /
----------------------------------------------------------/
----- Desc: Declares the NodeGrid Class and Functions used/
-----       for Storing an Array of Nodes in either a 2D  /
-----       or 3D grid.                                   /
-----       Functions tailored for use within the A*      /
-----       Pathfinding Algorithm.                        /
----------------------------------------------------------/
----- Author: Alex Kitching								  /
----------------------------------------------------------/
*/


using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// NodeGrid for Use with A* Algorithm and other Pathfinding Algorithms.
/// </summary>
public class NodeGrid : MonoBehaviour
{
    // Singleton
    private static NodeGrid instance;
    public static NodeGrid Instance
    {
        get { return instance; }
    }

    // Horizontal
    [SerializeField]
    private int maxX = 3;
    public int MaxX
    {
        get { return maxX; }
        set { maxX = value; }
    }
    // Vertical in 3D
    [SerializeField]
    private int maxY = 3;
    public int MaxY
    {
        get { return maxY; }
        set { maxY = value; }
    }
    // Vertical in 2D, Forwards/Backwards in 3D
    [SerializeField]
    private int maxZ = 3;
    public int MaxZ
    {
        get { return maxZ; }
        set { maxZ = value; }
    }

    public int MaxSize
    {
        get { return maxX * maxY * maxZ; }
    }

    // Grid of Nodes
    private Node[,,] grid;

    // Game Object we will be using for Visual Rep of Each Node
    [SerializeField]
    private GameObject gridNodePrefab;

    // Materials
    [SerializeField]
    private Material matPath;
    [SerializeField]
    private Material matStart;
    [SerializeField]
    private Material matTarget;
    [SerializeField]
    private Material matUnWalkable;

    // Start and Target Pos
    [SerializeField]
    private Vector3 startingNodePos;
    [SerializeField]
    private Vector3 targetNodePos;

    [SerializeField]
    private Vector3[] unwalkableNodes;

    private bool getVerticalNeighbours = false;

    [SerializeField]
    private bool startPathfinding = false;

    /// <summary>
    /// Awake Function for Assigning Singleton and Creating Grid.
    /// </summary>
    void Awake()
    {
        // Setup Singleton
        if(instance != null && instance != this) // Instance already exists and is not this instance
        {
            Destroy(gameObject); // Destroy this nodegrid
        }
        else // We can assign instance
        {
            instance = this;
        }

        // Create and Fill our Grid (Called in Awake and not Start incase we attempt to access with other objects start methods.
        CreateGrid();
    }

    /// <summary>
    /// Executes Pathfinding Upon Start being True. 
    /// </summary>
    void Update()
    {
        // Start Button Pressed
        if(startPathfinding)
        {
            // Create a new Stopwatch
            Stopwatch newWatch = new Stopwatch();

            // Start Watch
            newWatch.Start();

            // Start Button no longer preseed
            startPathfinding = false;

            // Create Pathfinding Object
            AStarPathfinding path = new AStarPathfinding();

            // Get Start and Target Nodes and Assign Materials
            Node startNode = GetNode(startingNodePos);
            Node targetNode = GetNode(targetNodePos);
            startNode.WorldObject.GetComponentInChildren<MeshRenderer>().material = matStart;
            targetNode.WorldObject.GetComponentInChildren<MeshRenderer>().material = matTarget;

            // Setup Unwalkable Nodes
            if (unwalkableNodes != null)
            {
                foreach(Vector3 node in unwalkableNodes)
                {
                    grid[(int)node.x, (int)node.y, (int)node.z].Walkable = false;
                    grid[(int)node.x, (int)node.y, (int)node.z].WorldObject.GetComponentInChildren<MeshRenderer>().material = matUnWalkable;
                }
            }

            // Assign Start and Target to Pathfinding System
            path.StartNode = startNode;
            path.TargetNode = targetNode;

            // Find Path 
            List<Node> p = path.FindPath();

            // Assign Path Node Materials
            for(int i = 0; i < p.Count -1; ++i)
            {
                p[i].WorldObject.GetComponentInChildren<MeshRenderer>().material = matPath;
            }

            // Print and Stop Elapsed Time
            UnityEngine.Debug.Log("Path found in " + newWatch.ElapsedMilliseconds.ToString() + "ms.");
            newWatch.Stop();
        }

    }

    /// <summary>
    /// Initialises Grid with Empty Nodes
    /// </summary>
    private void CreateGrid()
    {
        // Initialise Grid
        grid = new Node[maxX, maxY, maxZ];

        if (maxY > 1) // 3D Grid
        {
            getVerticalNeighbours = true;
        }

        // Fill Grid Starting X - Y - Z
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int z = 0; z < maxZ; z++)
                {
                    // Grid Obj Creation
                    GameObject nodeObject = Instantiate(gridNodePrefab, new Vector3(x, y, z), Quaternion.identity) as GameObject;
                    // Rename it accordingly
                    nodeObject.transform.name = x.ToString() + ", " + y.ToString() + ", " + z.ToString();
                    // Parent it underneath NodeGrids Transform
                    nodeObject.transform.parent = transform;

                    // Node Creation
                    Node node = new Node();
                    // Set Pos
                    node.PosX = x;
                    node.PosY = y;
                    node.PosZ = z;

                    // Assign World Object
                    node.WorldObject = nodeObject;

                    // Place Node in Grid
                    grid[x, y, z] = node;
                }
            }
        }
    }

    /// <summary>
    ///  Retrieves Node from Grid via Position
    /// </summary>
    /// <param name="a_iX"></param>
    /// <param name="a_iY"></param>
    /// <param name="a_iZ"></param>
    /// <returns></returns>
    public Node GetNode(int a_iX , int a_iY, int a_iZ)
    {
        // Check whether within grid
        if (a_iX < maxX && a_iX >= 0 &&
           a_iY < maxY && a_iY >= 0 &&
           a_iZ < maxZ && a_iZ >= 0)
        {
            return grid[a_iX, a_iY, a_iZ];
        }
        else // Not Within Grid
        {
            return null;
        }
    }

    /// <summary>
    /// Override for Retrieving Node from Grid using Vector
    /// </summary>
    /// <param name="a_vPos"></param>
    /// <returns></returns>
    public Node GetNode(Vector3 a_vPos)
    {
        // Grid is 1x1 Spacing so to avoid errors we round Passed in Vector
        int x = Mathf.RoundToInt(a_vPos.x);
        int y = Mathf.RoundToInt(a_vPos.y);
        int z = Mathf.RoundToInt(a_vPos.z);

        // Call above Get Node Function
        return GetNode(x, y, z);
    }

    /// <summary>
    /// Returns a List of Valid Neighbouring Nodes of a Passed in Node
    /// </summary>
    /// <param name="a_node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbours(Node a_node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int yIndex = y;

                    if(!getVerticalNeighbours) // If dont want 3D A* and only working in 2D then dont search Y
                    {
                        y = 0;
                    }

                    if (x == 0 && y == 0 && z == 0) // Node is equal to centre node so we skip
                        continue; 

                    // Store Neighbours Position
                    Node CheckPos = new Node();
                    CheckPos.PosX = a_node.PosX + x;
                    CheckPos.PosY = a_node.PosY + yIndex;
                    CheckPos.PosZ = a_node.PosZ + z;

                    Node newNode = GetNeighbourNode(CheckPos, a_node);

                    if(newNode != null)
                    {
                        neighbours.Add(newNode);
                    }
                }
            }
        }
        // Return Valid neighbour list
        return neighbours;
    }

    /// <summary>
    /// Attempts to Retrieve a Valid Neighbouring Node using a Passed in Current Node and Adj Node Position
    /// </summary>
    /// <param name="a_adjPos"></param>
    /// <param name="a_currentNode"></param>
    /// <returns></returns>
    private Node GetNeighbourNode(Node a_adjPos, Node a_currentNode)
    {
        Node retNode = null;

        Node node = GetNode(a_adjPos.PosX, a_adjPos.PosY, a_adjPos.PosZ);

        if(node != null && node.Walkable)
        {
            retNode = node;
        }
        else if(getVerticalNeighbours) // We are using 3D Grid
        {
            // Look for adjacent node underneath
            a_adjPos.PosY -= 1;
            Node bottomNode = GetNode(a_adjPos.PosX, a_adjPos.PosY, a_adjPos.PosZ);

            // If not null (within boundaries) and walkable
            if(bottomNode != null && bottomNode.Walkable)
            {
                retNode = bottomNode;
            }
            else
            {
                // Look above
                a_adjPos.PosY += 2;
                Node topNode = GetNode(a_adjPos.PosX, a_adjPos.PosY, a_adjPos.PosZ);
                // if not null and walkable
                if(topNode != null && topNode.Walkable)
                {
                    retNode = topNode; // We can return
                }
            }
        }

        // Check if Node is Valid
        if (!ValidNodeCheck(a_currentNode, a_adjPos))
            retNode = null;  // Return Null if Invalid

        return retNode;
    }

    /// <summary>
    /// Checks to See whether a node is Valid based upon Implemented Checks, Currently Implemented: Vertical Ascent Check and Diagonal Checks to Prevent Clipping
    /// </summary>
    /// <param name="a_currentNode"></param>
    /// <param name="a_adjNode"></param>
    /// <returns></returns>
    private bool ValidNodeCheck(Node a_currentNode, Node a_adjNode)
    {
        // Diagonal and Vertical Handling Checks
        int originalX = a_adjNode.PosX - a_currentNode.PosX;
        int originalY = a_adjNode.PosY - a_currentNode.PosY;
        int originalZ = a_adjNode.PosZ - a_currentNode.PosZ;

        // Checks both neighbours to see whether they either exist or are walkable, if either one matches criteria, diagonal isn't possible
        if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1)
        {
            Node neighbour1 = GetNode(a_currentNode.PosX + originalX, a_currentNode.PosY, a_currentNode.PosZ);
            if (neighbour1 == null || !neighbour1.Walkable)
            {
                return false;
            }

            Node neighbour2 = GetNode(a_currentNode.PosX, a_currentNode.PosY, a_currentNode.PosZ + originalZ);
            if (neighbour2 == null || !neighbour2.Walkable)
            {
                return false;
            }
        }

        if (originalY != 0) // Adj is Vertical Change
        {
            if (Mathf.Abs(originalX) == 1 && Mathf.Abs(originalZ) == 1) // Diagonal
            {
                // Checks Neighbour below attempted ascent
                Node neighbour = GetNode(a_currentNode.PosX + originalX, a_currentNode.PosY, a_currentNode.PosZ + originalZ);
                if (neighbour == null || !neighbour.Walkable)
                {
                    return false;
                }
            }
            else if (Mathf.Abs(originalX) == 1 || Mathf.Abs(originalZ) == 1) // Horizontal
            {
                Node neighbour1 = GetNode(a_currentNode.PosX + originalX, a_currentNode.PosY, a_currentNode.PosZ);
                if (neighbour1 == null || !neighbour1.Walkable)
                {
                    return false;
                }

                Node neighbour2 = GetNode(a_currentNode.PosX, a_currentNode.PosY, a_currentNode.PosZ + originalZ);
                if (neighbour2 == null || !neighbour2.Walkable)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

