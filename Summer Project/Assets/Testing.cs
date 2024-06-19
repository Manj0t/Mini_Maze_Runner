using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    const float CELLSIZE = 0.16f;
    void Start()
    {
        Grid grid = new Grid(10, 10, CELLSIZE);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
