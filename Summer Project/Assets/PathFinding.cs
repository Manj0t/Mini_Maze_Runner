using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class NumberExtension{
    public static bool Approx(this float f1, float f2) => Mathf.Approximately(f1, f2);
}

// public class  {
//     public Node parent = null;
//     public Vector3 position;
//     public float G = float.MaxValue;
//     public float RHS = float.MaxValue;
//     public bool unblocked;
//     public Node(Vector3 position, bool unblocked) {
//         this.position = position;
//         this.unblocked = unblocked;
//     }
// }

public struct Key {
    float k1;
    float k2;

    public Key(float k1, float k2) {
        this.k1 = k1;
        this.k2 = k2;
    }

    public static bool operator <(Key a, Key b) => a.k1 < b.k1 || (a.k1.Approx(b.k1) && a.k2 < b.k2);
    public static bool operator >(Key a, Key b) => a.k1 > b.k1 || (a.k1.Approx(b.k1) && a.k2 > b.k2);
    public static bool operator ==(Key a, Key b) => a.k1.Approx(b.k1) && a.k2.Approx(b.k2);
    public static bool operator !=(Key a, Key b) => !(a == b);

    public override bool Equals(object obj) {
        if (obj is Key other) {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode() {
        return k1.GetHashCode() ^ k2.GetHashCode();
    }
}


class PathFinding {
    private const int STRAIGHT_COST = 10;
    private const int DIAG_COST = 14;
    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;


    public PathFinding(int width, int height){
        grid = new Grid<PathNode>(width, height, 0.16f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY){
        PathNode startNode = grid.GetGridObjectject(startX, startY);
        PathNode endNode = grid.GetGridObjectject(endX, endY);
        openList = new List<PathNode>{ startNode };
        closedList = new List<PathNode>();

        for(int x = 0; x < grid.GetWidth(); x++){
            for(int y = 0; y < grid.GetHeight(); y++){
                PathNode pathNode = grid.GetGridObjectject(x, y);
                pathNode.g = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.parent = null;
            }
        }
        startNode.g = 0;
        startNode.h = CalculateHueristic(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0) {
            PathNode currentNode = 
        }
    }

    private int CalculateHueristic(PathNode a, PathNode b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return DIAG_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }
}