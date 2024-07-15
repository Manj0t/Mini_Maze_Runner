using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    const float CELLSIZE = 0.16f;
    Grid<int> grid;
    GameObject[] enemies;
    void Start()
    {

        PathFinding pathFinding = new PathFinding(10, 10);
    }
}
