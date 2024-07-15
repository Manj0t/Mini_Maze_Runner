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

    public PathNode parent;
    public PathNode(Grid<PathNode> grid, int x, int y){
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost(){
        f = g + h;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
