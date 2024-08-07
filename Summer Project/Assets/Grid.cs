using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cainos.PixelArtTopDown_Basic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem.iOS.LowLevel;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Grid<TGridObject>{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArry;
    private Vector3 originPosition;
    

    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new TGridObject[width, height];

        for(int x = 0; x < gridArray.GetLength(0); x++){
            for (int y = 0; y < gridArray.GetLength(1); y++){
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        debugTextArry = new TextMesh[width, height];
        //Debug.DrawLine draws the grid lines\
        bool showDebug = false;

        if(showDebug){        
            for(int x = 0; x < gridArray.GetLength(0); x++){
                for(int y = 0; y < gridArray.GetLength(1); y++){
                    // debugTextArry[x, y] = CreateWorldText(gridArray[x, y].ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f, 2, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }
    public int GetWidth(){
        return width;
    }
    public int GetHeight(){
        return height;
    }
    public List<TGridObject> GetNeighbors(Vector3 position){
        List<TGridObject> neighbors = new List<TGridObject>();

        int x = Mathf.FloorToInt((position.x - originPosition.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - originPosition.y) / cellSize);

        int[] dx = {1, 0, -1, 0};
        int[] dy = {0, 1, 0, -1};
        
        for(int i = 0; i < dx.Length; i++){
            int nx = x + dx[i];
            int ny = y + dy[i];

            if(nx >= 0 && nx < width && ny >= 0 && ny < height){
                neighbors.Add(gridArray[nx, ny]);
            }
        }
        return neighbors;
    }
    public Vector3 GetWorldPosition(int x, int y){
        return new Vector3(x, y) * cellSize + originPosition;
    }
    private void GetXY(Vector3 worldPosition, out int x, out int y){
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    public void SetValue(int x, int y, TGridObject value){
        if(x >= 0 && y >= 0 && x < width && y < height){
            gridArray[x, y] = value;
            debugTextArry[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value){
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }
    public TGridObject GetGridObjectject(int x, int y) {
        if(x >= 0 && y >= 0 && x < width && y < height){
            return gridArray[x, y];
        } else{
            return default(TGridObject);
        }
    }
    public TGridObject GetGridObjectject(Vector3 wordldPosition){
        int x, y;
        GetXY(wordldPosition, out x, out y);
        return GetGridObjectject(x, y);
    }
    //Everything Under this text is for visualising the grid on screen
    public static TextMesh CreateWorldText(string text, Transform parent, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 10){
        if(color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder){
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}