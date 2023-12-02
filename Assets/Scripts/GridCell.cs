using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private int posX;
    private int posY;

    // saves a reference to the gameobject that gets place on this cell
    public GameObject objectIsInGridSpace = null;

    // Saves if the grid space is occupied or not
    public bool isOccupied = false;
    public bool isWall = false;

    public void setPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public Vector2Int getPosition()
    {
        return new Vector2Int(posX, posY);
    }
}
