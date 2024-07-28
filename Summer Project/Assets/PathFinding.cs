using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class PathFinding {
    private const int STRAIGHT_COST = 10;
    private const int DIAG_COST = 14;
    public Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public Tilemap tilemap;
    int i = 0;

    public PathFinding(int width, int height, Tilemap tilemap){
        grid = new Grid<PathNode>(width, height, 0.16f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        this.tilemap = tilemap;
        InitializeGrid();
    }

    private void InitializeGrid(){
        for(int x = 0; x < grid.GetWidth(); x++){
            for(int y = 0; y < grid.GetHeight(); y++){
                PathNode pathNode = grid.GetGridObjectject(x, y);
                Vector3 worldPosition = grid.GetWorldPosition(x, y);
                Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

                TileBase tile = tilemap.GetTile(cellPosition);
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
        PathNode endNode = grid.GetGridObjectject(endX, endY);

        if (startNode == null || endNode == null) {
            Debug.LogError($"Start or End node is null (start: {startX},{startY}, end: {endX},{endY})");
            return null;
        }

        if (!endNode.isWalkable) {
            endNode = grid.GetGridObjectject(endX, endY - 1);
            if (endNode == null) {
                Debug.LogError("No walkable end node found.");
                return null;
            }
        }

        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        openList.Add(startNode);

        startNode.g = 0;
        startNode.h = CalculateDistance(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0 && i != 1000) {
            i++;

            PathNode currentNode = openList[0];
            foreach (var t in openList)
                if(t.f < currentNode.f || t.f == currentNode.f && t.h < currentNode.h)
                    currentNode = t;
            if (currentNode == endNode) {
                Debug.Log(i);
                return RetracePath(startNode, endNode);
            }

            closedList.Add(currentNode);
            openList.Remove(currentNode);

            foreach (PathNode neighbor in GetNeighbors(currentNode)) {
                if (closedList.Contains(neighbor) || !neighbor.isWalkable) continue;

                int tentativeGCost = currentNode.g + CalculateDistance(currentNode, neighbor);
                if (tentativeGCost < neighbor.g) {

                    // Debug.DrawLine(grid.GetWorldPosition(neighbor.x, neighbor.y) + new Vector3(0.16f / 2, 0.16f / 2, 0), grid.GetWorldPosition(currentNode.x, currentNode.y) + new Vector3(0.16f / 2, 0.16f / 2, 0), Color.blue, 500f);

                    neighbor.parent = currentNode;
                    neighbor.g = tentativeGCost;

                    if (!openList.Contains(neighbor)) {
                        neighbor.h = CalculateDistance(neighbor, endNode);
                        openList.Add(neighbor);
                    }
                    neighbor.CalculateFCost();
                }
            }
        }
        Debug.Log(i);
        return null; // No path found
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode){
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private List<PathNode> GetNeighbors(PathNode currentNode){
        List<PathNode> neighbors = new List<PathNode>();

        int x = currentNode.x;
        int y = currentNode.y;

        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                if (dx == 0 && dy == 0) continue;
                int checkX = x + dx;
                int checkY = y + dy;

                if (checkX >= 0 && checkX < grid.GetWidth() && checkY >= 0 && checkY < grid.GetHeight()) {
                    neighbors.Add(grid.GetGridObjectject(checkX, checkY));
                }
            }
        }

        return neighbors;
    }

    private PathNode FindClosestWalkableNode(PathNode node){
        // Implement a method to find the closest walkable node to the given node
        return null;
    }

    private int CalculateDistance(PathNode a, PathNode b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return DIAG_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }
}
