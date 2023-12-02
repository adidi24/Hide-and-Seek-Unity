using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    //private GridBehaviour environment;
    [SerializeField]
    private LayerMask WhatIsAGridLayer;
    private GridCell previouslyHoveredCell; // Variable to track previously hovered cell
    public Button spawnBtn;
    [SerializeField] private GridBehaviour environment;

    // Start is called before the first frame update
    void Start()
    {
        environment = FindObjectOfType<GridBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        GridCell mouseIsOverCell = IsMouseOverAGridSpace();

        if (!environment.usingPredefinedEnv)
        {
            if (mouseIsOverCell != null)
            {
                if (previouslyHoveredCell != mouseIsOverCell) // Check if the cell is a new hover
                {
                    // Execute the instructions only once when the mouse hovers over the cell
                    previouslyHoveredCell = mouseIsOverCell;

                    if (Input.GetMouseButton(0))
                    {
                        MakeWall(mouseIsOverCell);
                    }
                }
                if (Input.GetMouseButtonDown(0))
                {
                    MakeWall(mouseIsOverCell);
                }
            }
            else
            {
                // Reset the previouslyHoveredCell when the mouse is not over any cell
                previouslyHoveredCell = null;
            }
        }
        
    }

    private static void MakeWall(GridCell mouseIsOverCell)
    {
        //print("wall");
        if (mouseIsOverCell.GetComponent<MeshRenderer>().material.color != Color.grey)
        {
            mouseIsOverCell.GetComponent<MeshRenderer>().material.color = Color.grey;
            mouseIsOverCell.transform.localScale = new Vector3(1, 1, 2f);
            Vector3 pos = mouseIsOverCell.transform.position;
            mouseIsOverCell.transform.position = new Vector3(pos.x, pos.y, -0.5f);
        }
        else
        {
            mouseIsOverCell.GetComponent<MeshRenderer>().material.color = Color.white;
            mouseIsOverCell.transform.localScale = new Vector3(1, 1, 1);
            Vector3 pos = mouseIsOverCell.transform.position;
            mouseIsOverCell.transform.position = new Vector3(pos.x, pos.y, 0);
        }

        mouseIsOverCell.isWall = !mouseIsOverCell.isWall;
    }

    //Returns the grid cell if mouse is over a grid cell
    private GridCell IsMouseOverAGridSpace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 300f, WhatIsAGridLayer))
        {
            return hitInfo.transform.GetComponent<GridCell>();
        }
        else
        {
            return null;
        }
    }
}
