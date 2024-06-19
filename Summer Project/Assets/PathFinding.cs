using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberExtension{
    public static bool Aprox(this float f1, float f2) => Mathf.Approximately(f1, f2);
}
namespace PathFinding{

    public class Node{
        public Node parent;
        public float G{get; set;}
        public float RHS{get; set;}

    }


}
