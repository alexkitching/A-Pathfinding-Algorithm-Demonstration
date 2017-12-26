/*
----------------------------------------------------------/
----- Project: AStar Record 							  /
----- File:    AStarPathfinding.cs			              /
----------------------------------------------------------/
----- Desc: Declares the AStarPathfinding Class and       /
-----       Functions used for Demonstrating the Use of   /
-----       the A* Algorithm.                             /
----------------------------------------------------------/
----- Author: Alex Kitching								  /
----------------------------------------------------------/
*/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AStarPathfinding Class used for Finding a Path from a Start Position to a Target Position.
/// </summary>
public class AStarPathfinding
{
    // Reference to grid
    private NodeGrid nodeGrid;

    // Start and End Position
    private Node startNode;
    public Node StartNode
    {
        get { return startNode; }
        set { startNode = value; }
    }

    private Node targetNode;
    public Node TargetNode
    {
        get { return targetNode; }
        set { targetNode = value; }
    }


    /// <summary>
    /// Returns List of Nodes in order of Path to Follow
    /// </summary>
    /// <returns></returns>
    public List<Node> FindPath()
    {
        if(NodeGrid.Instance != null)
        {
            nodeGrid = NodeGrid.Instance;
            return PathFind(startNode, targetNode);
        }
        else // No Grid Present
        {
            Debug.Log("No Grid Present in Scene");
            return null;
        }
    }

    /// <summary>
    /// Calculates Path from Start Node to Target Node and Returns List of Nodes within Path.
    /// </summary>
    /// <param name="a_startNode"></param>
    /// <param name="a_targetNode"></param>
    /// <returns></returns>
    private List<Node> PathFind(Node a_startNode, Node a_targetNode)
    {
        // Algorithm Begins
        List<Node> returnPath = new List<Node>();

        // 2 Lists Required
        // Heap
        Heap<Node> openSet = new Heap<Node>(nodeGrid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        // Add Start Node to Open Set
        openSet.Add(a_startNode);

        // Run as Long as we have unvisited nodes
        while(openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if(currentNode.Equals(a_targetNode))
            {
                // Destination Found, Retrace Path Here
                returnPath = RetracePath(a_startNode, currentNode);
                break;
            }

            // Still Havent Reached Target
            foreach (Node neighbour in NodeGrid.Instance.GetNeighbours(currentNode))
            {
                // Check if Neighbour is walkable or hasn't already been checked
                if(closedSet.Contains(neighbour))
                {
                    continue;
                }

                // Create new movement cost
                float MovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);

                if(MovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
                {
                    // Calculate new costs
                    neighbour.GCost = MovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, a_targetNode);

                    // Assign Previous Node
                    neighbour.PreviousNode = currentNode;
                    
                    // Add neighbour to open set
                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                    else
                    {
                        // Hash Set Update Item
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return returnPath;
    }

    /// <summary>
    /// Retraces Path by Compiling a List of Visited Nodes and Organising them from Start to Finish
    /// </summary>
    /// <param name="a_startNode"></param>
    /// <param name="a_endNode"></param>
    /// <returns></returns>
    private List<Node> RetracePath(Node a_startNode, Node a_endNode)
    {
        List<Node> path = new List<Node>();

        Node currentNode = a_endNode;

        // Retrace path, going from end node to start node
        while (currentNode != a_startNode)
        {
            path.Add(currentNode);
            // by taking the parent nodes we assigned
            currentNode = currentNode.PreviousNode;
        }

        // Then reverse the list
        path.Reverse();

        return path;
    }

    /// <summary>
    ///  Returns Distance between two Nodes used in Pathfinding.
    /// </summary>
    /// <param name="a_posA"></param>
    /// <param name="a_posB"></param>
    /// <returns></returns>
    private int GetDistance(Node a_posA, Node a_posB)
    {
        // We find the distance between each node, needs to be positive therefore use absolute
        int distX = Mathf.Abs(a_posA.PosX - a_posB.PosX);
        int distY = Mathf.Abs(a_posA.PosY - a_posB.PosY);
        int distZ = Mathf.Abs(a_posA.PosZ - a_posB.PosZ);

        // We use 10 and 14 because distance to move diagonally is the square root of 2 which is around 1.4 times the cost of moving either vertically or horizontally
        if (distX > distZ) // Vertical Distance is greater than horizontal distance
        {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        } // Else we do opposite
        return 14 * distX + 10 * (distZ - distX) + 10 * distY;
    }
}
