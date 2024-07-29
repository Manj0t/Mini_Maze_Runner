using System.Collections.Generic;


class PathNodeComparer : IComparer<PathNode> {
    public int Compare(PathNode a, PathNode b) {
        if (a.F != b.F) {
            return a.F.CompareTo(b.F);
        }
        return a.H.CompareTo(b.H);
    }
}


public class PriorityQueue {
    List<PathNode> queue;

    public PriorityQueue(){
        queue = new List<PathNode>();
    }
    public bool Contains(PathNode node){
        return queue.Contains(node);
    }
    public void Enqueue(PathNode node){

        int index = queue.BinarySearch(node, new PathNodeComparer());
        if (index < 0) {
            index = ~index;
        }
        queue.Insert(index, node);

    }
    public PathNode Dequeue(){
        if(isEmpty()) return null;
        PathNode front = queue[0];
        queue.RemoveAt(0);
        return front;
    }
    public bool isEmpty(){
        return queue.Count == 0;
    }
}
