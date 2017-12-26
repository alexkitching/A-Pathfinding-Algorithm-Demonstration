/*
----------------------------------------------------------/
----- Project: AStar Record 							  /
----- File:    Node.cs						              /
----------------------------------------------------------/
----- Desc: Declares the Node Class and Functions used    /
-----       within the A* Pathfinding Algorithm.          /
----------------------------------------------------------/
----- Author: Alex Kitching								  /
----------------------------------------------------------/
*/

using UnityEngine;

/// <summary>
/// Base Node Class
/// <para>Contains Varibles and Functions required for Nodes to be used within the A* pathfinding Algorithm.</para>
/// <para>Implements IHeapItem from Heap for use within the Heap Datastructure.</para>
/// </summary>
public class Node : IHeapItem<Node>
{
    // Position
    private int posX;
    public int PosX
    {
        get { return posX; }
        set { posX = value; }
    }

    private int posY;
    public int PosY
    {
        get { return posY; }
        set { posY = value; }
    }

    private int posZ;
    public int PosZ
    {
        get { return posZ; }
        set { posZ = value; }
    }

    // Pathfinding Variables
    // hCost - Cost from Current Position to Destination
    private float hCost;
    public float HCost
    {
        get { return hCost; }
        set { hCost = value; }
    }

    // gCost - Cost from Starting Position to Current Position
    private float gCost;
    public float GCost
    {
        get { return gCost; }
        set { gCost = value; }
    }

    // fCost - gCost + hCost (Overall Total Cost)
    public float FCost
    {
        get { return hCost + gCost; }
    }

    // Whether node is walkable
    private bool walkable = true;
    public bool Walkable
    {
        get { return walkable; }
        set { walkable = value; }
    }

    // Previous node (Used for later retracing path)
    private Node previousNode;
    public Node PreviousNode
    {
        get { return previousNode; }
        set { previousNode = value; }
    }

    // World Object Assigned to Node
    private GameObject worldObject;
    public GameObject WorldObject
    {
        get { return worldObject; }
        set { worldObject = value; }
    }

    int heapIndex;
    public int HeapIndex
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    // Return 1 if Current Item has higher priority
    public int CompareTo(Node a_nodetoCompare)
    {
        // Int/Float CompareTo() returns 1 if comparison is higher
        int compareVal = FCost.CompareTo(a_nodetoCompare.FCost);

        if(compareVal == 0) // F Cost is Equal
        {
            // Compare H cost for tiebreaker
            compareVal = hCost.CompareTo(a_nodetoCompare.hCost);
        }

        // Return 1 if its lower not higher so reverse values
        return -compareVal;
    }
}
