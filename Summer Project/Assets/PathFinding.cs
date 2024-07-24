using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Tilemaps;

class PathFinding {
    private const int STRAIGHT_COST = 10;
    private const int DIAG_COST = 14;
    private Grid<PathNode> grid;
    private PriorityQueue openList;
    private List<PathNode> closedList;

    public TilemapCollider2D tilemapCollider;
    private Tilemap tilemap;



    public PathFinding(int width, int height, Tilemap tilemap){
        grid = new Grid<PathNode>(width, height, 0.16f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        Debug.Log(grid.GetHeight());
        this.tilemap = tilemap;
        if (tilemapCollider == null)
        {
            tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
        }
        InitializeGrid();
    }

    private T GetComponent<T>()
    {
        throw new NotImplementedException();
    }

    private void InitializeGrid(){
    for(int x = 0; x < grid.GetWidth(); x++){
        for(int y = 0; y < grid.GetHeight(); y++){
            PathNode pathNode = grid.GetGridObjectject(x, y);
            Vector3 worldPosition = grid.GetWorldPosition(x, y);
            Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

            // Check if the cell has a collider
            TileBase tile = tilemap.GetTile(cellPosition);

            // Check for collisions in the area of the cell
            Collider2D collider = Physics2D.OverlapCircle(worldPosition, 0.16f / 2);
            if (tile != null) {
                grid.GetGridObjectject(x, y).isWalkable = false;
            } else {
                grid.GetGridObjectject(x, y).isWalkable = true;
            }

            pathNode.g = int.MaxValue;
            pathNode.CalculateFCost();
            pathNode.parent = null;
        }
    }
}
    public List<PathNode> FindPath(int startX, int startY, int endX, int endY){
        PathNode startNode = grid.GetGridObjectject(startX, startY);
        if (startNode == null) {
            Debug.LogError($"Start node is null at ({startX}, {startY})");
            return null;
        }
        PathNode endNode = grid.GetGridObjectject(endX, endY);
        if (endNode == null) {
            Debug.LogError($"End node is null at ({endX}, {endY})");
            return null;
        }
        openList = new PriorityQueue();
        closedList = new List<PathNode>();
        openList.Enqueue(startNode);

        startNode.g = 0;
        startNode.h = CalculateHueristic(startNode, endNode);
        startNode.CalculateFCost();

        while(!openList.isEmpty()) {
            PathNode currentNode = openList.Dequeue();
            if (currentNode == null) {
                Debug.LogError("Current node is null when dequeuing from open list");
                continue;
            }
            closedList.Add(currentNode);
            if(currentNode == endNode){
                List<PathNode> path = new List<PathNode>();
                while(currentNode.parent != null){
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }
                path.Reverse();
                return path;
            }

            FindNeighbors(currentNode, endNode);
        }
        return null;
    }

    private void FindNeighbors(PathNode currentNode, PathNode endNode){
        int x = currentNode.x;
        int y = currentNode.y;
        for(int i = -1; i <= 1; i++){
            for(int j = -1; j <= 1; j++){
                if(x + i >= grid.GetWidth() || x + i < 0 || y + j > grid.GetHeight() || y + j < 0) continue;

                PathNode newNode = grid.GetGridObjectject(x + i, y + j);
                if (newNode == null) {
                    Debug.LogError($"New node is null at ({x + i}, {y + j})");
                    continue;
                }
                if(!newNode.isWalkable){
                    continue;
                }
                // if (closedList.Contains(newNode)){
                //     Debug.Log("CONTINUNING");
                //     continue;
                // }
                int tentativeGCost = currentNode.g + CalculateHueristic(currentNode, newNode);
                if (tentativeGCost < newNode.g) {
                    newNode.parent = currentNode;
                    newNode.g = tentativeGCost;
                    newNode.h = CalculateHueristic(newNode, endNode);
                    newNode.CalculateFCost();
                    openList.Enqueue(newNode);
                }
            }
        }
    }

    private int CalculateHueristic(PathNode a, PathNode b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return DIAG_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }
}
