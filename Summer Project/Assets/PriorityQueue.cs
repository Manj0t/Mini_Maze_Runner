using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

class PriorityQueueComparer : IComparer<PathNode> {
    public int Compare(PathNode a, PathNode b) {
        if(a.f > b.f) return 1;
        else if(a.f < b.f) return -1;
        else return 0;
    }
}


public class PriorityQueue {
    // Start is called before the first frame update
    SortedSet<PathNode> queue;

    public PriorityQueue(){
        queue = new SortedSet<PathNode>(new PriorityQueueComparer());
    }
    public bool Contains(PathNode node){
        return queue.Contains(node);
    }
    public void Enqueue(PathNode node){
        queue.Add(node);
    }
    public PathNode Dequeue(){
        if(isEmpty()) return null;
        PathNode front = queue.Min;
        queue.Remove(front);
        return front;
    }
    public bool isEmpty(){
        return queue.Count == 0;
    }
}
