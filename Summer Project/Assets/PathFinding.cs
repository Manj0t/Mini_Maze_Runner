using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

class PathFinding {
    private const int STRAIGHT_COST = 10;
    private const int DIAG_COST = 14;
    public Grid<PathNode> grid;
    private PriorityQueue openList;
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

                pathNode.setG(int.MaxValue);
                pathNode.CalculateFCost();
                pathNode.setParent(null);
            }
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY){
        PathNode startNode = grid.GetGridObjectject(startX, startY);
        PathNode endNode = grid.GetGridObjectject(endX, endY - 1);

        if (startNode == null || endNode == null) {
            Debug.LogError($"Start or End node is null (start: {startX},{startY}, end: {endX},{endY})");
            return null;
        }

        if (!endNode.isWalkable) {
            endNode = grid.GetGridObjectject(endX - 1, endY - 1);
            if (!endNode.isWalkable) {
                endNode = grid.GetGridObjectject(endX - 1, endY - 2);
            }
            if (endNode == null) {
                Debug.LogError("No walkable end node found.");
                return null;
            }
        }

        openList = new PriorityQueue();
        closedList = new List<PathNode>();
        openList.Enqueue(startNode);

        startNode.setG(0);
        startNode.setH(CalculateDistance(startNode, endNode));
        startNode.CalculateFCost();

        while (!openList.isEmpty() && i != 100000) {
            i++;

            PathNode currentNode = openList.Dequeue();

            if (currentNode == endNode) {
                Debug.Log(i);
                return RetracePath(startNode, endNode);
            }

            closedList.Add(currentNode);
            foreach (PathNode neighbor in GetNeighbors(currentNode)) {
                if (closedList.Contains(neighbor) || !neighbor.isWalkable) continue;

                int newNeighborGCost = currentNode.G + CalculateDistance(currentNode, neighbor);
                if (newNeighborGCost < neighbor.G) {

                    // Debug.DrawLine(grid.GetWorldPosition(neighbor.x, neighbor.y) + new Vector3(0.16f / 2, 0.16f / 2, 0), grid.GetWorldPosition(currentNode.x, currentNode.y) + new Vector3(0.16f / 2, 0.16f / 2, 0), Color.blue, 500f);

                    neighbor.setParent(currentNode);
                    neighbor.setG(newNeighborGCost);

                    if (!openList.Contains(neighbor)) {
                        neighbor.setH(CalculateDistance(neighbor, endNode));
                        openList.Enqueue(neighbor);
                    }
                    neighbor.CalculateFCost();
                }
            }
        }
        return null; // No path found
    }

    private List<PathNode> RetracePath(PathNode startNode, PathNode endNode){
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
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

    private int CalculateDistance(PathNode a, PathNode b){
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return DIAG_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }
}