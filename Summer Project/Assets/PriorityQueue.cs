using System.Collections;
using System.Collections.Generic;
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

    
}
