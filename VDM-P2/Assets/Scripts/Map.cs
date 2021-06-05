using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// CONTAINERS ---------------------------------
[System.Serializable]
public struct Point
{
    public int x;
    public int y;
}

[System.Serializable]
public struct PointDouble
{
    public double x;
    public double y;
}

[System.Serializable]
public struct Wall
{
    public Point o;
    public Point d;
}

[System.Serializable]
public struct Trap
{
    string d;

    Point v1, v2;
}

[System.Serializable]
public class JSONmap
{
    public int c, r;
    public Point s;
    public Point f;
    public Point[] h;
    public Wall[] w;
    public PointDouble[] i;
    public Point[] e;
    public object[] t;
}

public class TileInfo
{
    public bool wallTop = false, wallLeft = false;
    public bool iceFloor = false;
    public bool goal = false;
}

[System.Serializable]
public class Map
{
    /// <summary>
    /// Constructs the map using data that can be retrieved from the level jsons
    /// </summary>
    /// <param name="c">Columns</param>
    /// <param name="r">Rows</param>
    /// <param name="s">Start point</param>
    /// <param name="h">Hint array</param>
    /// <param name="w">Wall array</param>
    /// <param name="i">Ice floor array</param>
    /// <param name="e">Enemy array</param>
    public Map(int c, int r, Point s, Point[] h, Wall[] w, PointDouble[] i, Point[] e)
    {
        X = c + 1; Y = r + 1;
        tileInfoMatrix = new TileInfo[X, Y]; //one extra col & row to draw the bottom and right walls of the map
        for (int col = 0; col <= c; ++col)
        {
            for (int row = 0; row <= r; ++row)
            {
                tileInfoMatrix[col, row] = new TileInfo();
            }
        }

        start = s;

        // hint information
        //tileInfoMatrix[s.x, Mathf.Abs(s.y - r + 1)].hintPoint = true;
        hintArray = new Vector2[h.Length + 1];
        int index = 0;
        hintArray[index] = new Vector2(s.x, Mathf.Abs(s.y - r + 1));
        foreach (Point hint in h)
        {
            ++index;
            hintArray[index] = new Vector2(hint.x, Mathf.Abs(hint.y - r + 1));
            //tileInfoMatrix[hint.x, Mathf.Abs(hint.y - r + 1)].hintPoint = true;
        }

        // wall information
        foreach (Wall wall in w)
        {
            bool left = (wall.o.x == wall.d.x);
            bool top = (wall.o.y == wall.d.y);
            int x = Mathf.Min(wall.o.x, wall.o.x);
            int y = Mathf.Min(Mathf.Abs(wall.o.y - r), Mathf.Abs(wall.d.y - r));
            if (top) tileInfoMatrix[x, y].wallTop = true;
            if (left) tileInfoMatrix[x, y].wallLeft = true;
        }

        // ice floor information
        foreach (PointDouble ice in i)
        {
            tileInfoMatrix[Mathf.FloorToInt((float)ice.x), Mathf.FloorToInt(Mathf.Abs((float)(ice.y - r + 1)))].iceFloor = true;
        }
    }


    /// <summary> 
    /// Matrix that stores all the information needed for the creation of tiles.
    /// Access with tileInfoMatrix[x,y]
    /// </summary>
    public TileInfo[,] tileInfoMatrix;
    public Vector2[] hintArray;
    public Point start;
    public int X, Y;

    /// <summary>
    /// Reads a json level file and creates an instance of Map with a filled TileInfo matrix
    /// </summary>
    /// <param name="path">Path to the json file</param>
    /// <returns>An completed instance of the Map class</returns>
    public static Map FromJson(string path)
    {
        string jsonS = File.ReadAllText(path);
        JSONmap jsonmap = JsonUtility.FromJson<JSONmap>(jsonS);
        Map map = new Map(jsonmap.c, jsonmap.r, jsonmap.s, jsonmap.h, jsonmap.w, jsonmap.i, jsonmap.e);

        return map;
    }
}
