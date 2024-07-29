using UnityEngine.U2D.IK;

public class PathNode{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int G{get; private set;}
    public int H{get; private set;}
    public int F{get; private set;}
    public bool isWalkable;

    public PathNode Parent {get; private set;}
    public PathNode(Grid<PathNode> grid, int x, int y){
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
    public PathNode(PathNode copy){
        this.grid = copy.grid;
        this.x = copy.x;
        this.y = copy.y;
        G = copy.G;
        H = copy.H;
    }
    public void setParent(PathNode n) => Parent = n;
    public void CalculateFCost() => F = G + H;
    public void setH(int h) => H = h;
    public void setG(int g) => G = g;
}
