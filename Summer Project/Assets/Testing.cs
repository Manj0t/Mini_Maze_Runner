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
        enemies = GameObject.FindGameObjectsWithTag("Slime");
        foreach(GameObject enemy in enemies){
            Vector3 pos = enemy.transform.position;
            grid = new Grid<int>(10, 10, CELLSIZE, pos - new Vector3(5 * CELLSIZE, 5 * CELLSIZE));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
        // enemy = GameObject.FindGameObjectWithTag("Player").transform.position;
        // grid = new Grid<int>(10, 10, CELLSIZE, enemy);
    }
}
