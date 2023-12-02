using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AgentMove : MonoBehaviour
{
    [SerializeField]
    private int posX;

    [SerializeField]
    private int posY;
    private bool isHider = false;
    private GridBehaviour environment;
    //public AgentState agentState = new();
    public Button spawnBtn;
    public Vector3 detectionSize = new(8f, 8f, 0.1f);
    private readonly int[,] Observation = new int[9, 9];
    private readonly int ObservationRange = 9;

    //Returns the agent state
    public int[] GetState()
    {
        return To1DArray(Observation);
    }

    public void SetState()
    {
        environment = FindObjectOfType<GridBehaviour>();
        Array.Clear(Observation, 0, ObservationRange * ObservationRange);
        Vector2Int agentGridPos = environment.GetGridPosFromWorld(transform.position);

        Bounds detectionBounds = new Bounds(transform.position, detectionSize);

        // Find all colliders within the specified bounds
        Collider[] colliders = Physics.OverlapBox(detectionBounds.center, detectionBounds.extents);


        foreach (Collider collider in colliders)
        {
            //Debug.Log("Detected: " + collider.gameObject.name + "Pos: " + collider.gameObject.transform.position);
            Vector2Int coliderGridPos = environment.GetGridPosFromWorld(collider.gameObject.transform.position);
            //Debug.Log("coliderGridPos: " + coliderGridPos);
            Vector2Int coliderRangePos = new(0, 0);
            int subgridStartX = agentGridPos.x - ObservationRange / 2;
            int subgridStartY = agentGridPos.y - ObservationRange / 2;


            coliderRangePos.x = coliderGridPos.x - subgridStartX;
            coliderRangePos.y = coliderGridPos.y - subgridStartY;
            //Debug.Log("Agent: "+ agentGridPos);
            //Debug.Log(subgridStartX + " "  + subgridStartY);
            //Debug.Log("coliderRangePos: " + coliderRangePos);


            // if Obstacle
            if (collider.gameObject.GetComponent<GridCell>())
            {
                if (collider.gameObject.GetComponent<GridCell>().isWall)
                {
                    Observation[coliderRangePos.x, coliderRangePos.y] = 3;
                }
            }

            // if Agent
            else if (collider.gameObject.GetComponent<AgentMove>())
            {
                if (collider.gameObject.GetComponent<AgentMove>().GetHider())
                {
                    // if Hider
                    Observation[coliderRangePos.x, coliderRangePos.y] = 1;
                }
                else
                {
                    // if Seeker
                    Observation[coliderRangePos.x, coliderRangePos.y] = 2;
                }
            }

        }
    }

    public void SetPosition(Vector2Int newPos)
    {
        posX = newPos.x;
        posY = newPos.y;
    }

    public Vector2Int GetPosition()
    {
        return new Vector2Int(posX, posY);
    }

    public void SetHider(bool ishider)
    {
        isHider = ishider;
    }

    public bool GetHider()
    {
        return isHider;
    }

    public void Update()
    {
        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 bottomCenter = transform.position;
        Gizmos.DrawWireCube(bottomCenter, detectionSize);
    }

    // Spawn the agent randomly at the starting line
    public void SpawnAgent()
    {
        environment = FindObjectOfType<GridBehaviour>();
        environment.startingLine = new List<GridCell>();
        // loop over the first row in the grid and add cells to starting line if not wall
        for (int i = 0; i < environment.columns; i++)
        {
            GridCell cell = environment.gameGrid[i, 0].GetComponent<GridCell>();
            if (!cell.isWall)
            {
                environment.startingLine.Add(cell);
            }
        }
        // loop over the last column in the grid and add cells to finish line if not wall
        for (int j = 0; j < environment.rows; j++)
        {
            GridCell cell = environment
                .gameGrid[environment.columns - 1, j]
                .GetComponent<GridCell>();
            if (!cell.isWall)
            {
                cell.GetComponent<MeshRenderer>().material.color = Color.green;
                environment.finishLine.Add(cell);
            }
        }

        // take a random position at the starting line to instanciate the agent
        var random = new System.Random();
        int index = random.Next(environment.startingLine.Count);
        int x = environment.startingLine[index].getPosition().x;
        int y = environment.startingLine[index].getPosition().y;

        // Set agent position
        transform.position = new Vector3(x * environment.scale, y * environment.scale, -1.3f);
        gameObject.GetComponent<AgentMove>().SetPosition(new Vector2Int(x, y));

        // make the first cell occupied
        environment.startingLine[index].objectIsInGridSpace = gameObject;
        environment.startingLine[index].isOccupied = true;
        spawnBtn.enabled = false;

        gameObject.SetActive(true);
    }

    // Respawn the agent randomly at the starting line
    public void RespawnAgent()
    {
        environment = FindObjectOfType<GridBehaviour>();

        // take a random position at the starting line to instanciate the agent
        var random = new System.Random();
        int index = random.Next(environment.startingLine.Count);
        int x = environment.startingLine[index].getPosition().x;
        int y = environment.startingLine[index].getPosition().y;

        // Set agent position
        transform.position = new Vector3(x * environment.scale, y * environment.scale, -1.3f);
        gameObject.GetComponent<AgentMove>().SetPosition(new Vector2Int(x, y));

        // make the first cell occupied
        environment.startingLine[index].objectIsInGridSpace = gameObject;
        environment.startingLine[index].isOccupied = true;
        spawnBtn.enabled = false;

        gameObject.SetActive(true);
    }


    static int[] To1DArray(int[,] input)
    {
        int size = input.Length;
        int[] result = new int[size];

        int write = 0;
        for (int j = 0; j <= input.GetUpperBound(0); j++)
        {
            for (int i = 0; i <= input.GetUpperBound(1); i++)
            {
                result[write++] = input[i, j];
            }
        }
        return result;
    }
}
