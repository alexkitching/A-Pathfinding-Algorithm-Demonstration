/*
----------------------------------------------------------/
----- Project: AStar Record 							  /
----- File:    Heap.cs						              /
----------------------------------------------------------/
----- Desc: Declares the Heap Data Structure Class and    /
-----       Functions tailored for use within the A*      /
-----       Pathfinding Algorithm.                        /
----------------------------------------------------------/
----- Author: Alex Kitching								  /
----------------------------------------------------------/
*/

using System;

/// <summary>
/// Heap Data Structure for use with A* Pathfinding Algorithm.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Heap<T> where T : IHeapItem<T>
{
    T[] items; // Items in Heap
    int currentItemCount;

    /// <summary>
    /// Used to Initialise Heap to Maximum Size.
    /// </summary>
    /// <param name="a_iMaxHeapSize"></param>
    public Heap(int a_iMaxHeapSize)
    {
        items = new T[a_iMaxHeapSize]; // Initialise Array to Max Heap Size
    }

    /// <summary>
    /// Adds an Item to the Heap and Sorts the Data Accordingly.
    /// </summary>
    /// <param name="a_item"></param>
    public void Add(T a_item)
    {
        // Assign Heap index to Current Item Count
        a_item.HeapIndex = currentItemCount;
        // Assign Item to Heap
        items[currentItemCount] = a_item;
        // Sorts Against Existing Items in Heap
        SortAscending(a_item);
        // New Item Added Increment
        currentItemCount++;
    }

    /// <summary>
    /// Removes and Returns the first Item in the Heap and Sorts the Data Accordingly.
    /// </summary>
    /// <returns></returns>
    public T RemoveFirst()
    {
        // Get First Item in heap
        T firstItem = items[0];
        // One Less Item In Heap
        currentItemCount--;
        // Set Last Item in Heap to First Item
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        // Sort Descending
        SortDescending(items[0]);
        return firstItem;
    }

    /// <summary>
    /// Updates an Item in the Heap using its new Values.
    /// <para>Will need to do this during A* Pathfinding for updating F costs</para>
    /// </summary>
    /// <param name="a_item"></param>
    public void UpdateItem(T a_item)
    {
        SortAscending(a_item);
    }

    /// <summary>
    ///  Returns the Heaps current item count.
    /// </summary>
    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }

    /// <summary>
    ///  Returns True if Heap Contains Passed in Item.
    /// </summary>
    /// <param name="a_item"></param>
    /// <returns></returns>
    public bool Contains(T a_item)
    {
        return Equals(items[a_item.HeapIndex], a_item);
    }

    /// <summary>
    /// Sorts Heap Item in Ascending order.
    /// </summary>
    /// <param name="a_item"></param>
    void SortAscending(T a_item)
    {
        // To find parent is Index - 1 / 2
        int parentIndex = (a_item.HeapIndex - 1) / 2;

        while (true) // Loop to ensure all we don't just compare once
        {
            // Get Parent
            T parentItem = items[parentIndex];
            
            if (parentItem != null) // Parent is Present
            {
                // Compare Current Item to Parent Item using the Comparable interface declared below
                if (a_item.CompareTo(parentItem) > 0)
                {
                    // If item is greater than 0 - Item is above parent therefore we need to swap
                    Swap(a_item, parentItem);
                    // Assign New Parent Index
                    parentIndex = (a_item.HeapIndex - 1) / 2;
                }
                else // Not Higher than Parent item
                {
                    // Item in Correct Position
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Sorts Heap Item in Descending order.
    /// </summary>
    /// <param name="a_item"></param>
    void SortDescending(T a_item)
    {
        while (true)
        {
            // Get Child Nodes
            int childIndexLeft = a_item.HeapIndex * 2 + 1;
            int childIndexRight = a_item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            
            if(childIndexLeft < currentItemCount) // Left Child Present (Within Bounds)
            {
                swapIndex = childIndexLeft; // Default Swap to Left Child

                if(childIndexRight < currentItemCount) // Right Child Present (Within Bounds)
                {
                    // Check which has higher priority
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)  // if Right child is less than 0 - item is below left child
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if(a_item.CompareTo(items[swapIndex]) < 0) // Compare Parent with highest priority child
                {
                    Swap(a_item, items[swapIndex]); // Parent Precedes Child so swap
                }
                else
                {
                    // Parent must have higher priority than both of its children its in correct position
                    // Exit loop
                    return;
                }
            }
            else // Parent has no children
            {
                return;
            }
        }
    }

    /// <summary>
    ///  Swaps two Items within the Heap
    /// </summary>
    /// <param name="a_item1"></param>
    /// <param name="a_item2"></param>
    void Swap (T a_item1, T a_item2)
    {
        // Swap Items
        items[a_item1.HeapIndex] = a_item2;
        items[a_item2.HeapIndex] = a_item1;
        // Swap Indexes
        int item1Index = a_item1.HeapIndex;
        a_item1.HeapIndex = a_item2.HeapIndex;
        a_item2.HeapIndex = item1Index;
    }
}

/// <summary>
/// Interface Required for Items to be contained within a Heap.
/// <para>Requires CompareTo Method to be Implemented as well as HeapIndex.</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}