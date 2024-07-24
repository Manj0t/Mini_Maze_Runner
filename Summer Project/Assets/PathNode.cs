using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class PathNode{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int g, h, f;
    public bool isWalkable;

    public PathNode parent;
    public PathNode(Grid<PathNode> grid, int x, int y){
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
    public PathNode(PathNode copy){
        this.grid = copy.grid;
        this.x = copy.x;
        this.y = copy.y;
        g = copy.g;
        h = copy.h;
        f = copy.f;
    }

    public void CalculateFCost(){
        f = g + h;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
