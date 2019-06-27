using UnityEngine;
using System.Collections;
using System;

public class Model : MonoBehaviour
{
    public int width = 4;
    public int height = 7;
    public Cube[] cubeList;

    internal Cube GetCube(int x, int y)
    {
        if (x < 1 || x > height)
        {
            return null;
        }

        if (y < 1 || y > width)
        {
            return null;
        }
        int n = Mathf.Clamp((x - 1), 0, 6) * 4 + Mathf.Clamp((y - 1), 0, 3);
        return cubeList[n];
    }
}
