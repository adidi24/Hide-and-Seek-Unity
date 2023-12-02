using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AgentState : MonoBehaviour
{
    private readonly Int16[,] Observation = new Int16[9, 9];
    List<Int16> ret = new List<Int16>();
    private readonly int ObservationRange = 9;
    [SerializeField] private GridBehaviour environment;

    //Returns the agent state
    public Array GetState()
    {
        for (int i = 0; i < Observation.GetLength(0); i++)
        {
            for (int j = 0; j < Observation.GetLength(1); j++)
            {
                ret.Add(Observation[i, j]);
            }
        }

        return ret.ToArray();
    }

    public void SetState(GameObject agent)
    {
        environment = FindObjectOfType<GridBehaviour>();
        Array.Clear(Observation, 0, ObservationRange*ObservationRange);
        Vector2Int agentGridPos = agent.GetComponent<AgentMove>().GetPosition();

        Bounds detectionBounds = new Bounds(agent.transform.position, agent.GetComponent<AgentMove>().detectionSize);

        // Find all colliders within the specified bounds
        Collider[] colliders = Physics.OverlapBox(detectionBounds.center, detectionBounds.extents);

        foreach (Collider collider in colliders)
        {
            //Debug.Log("Detected: " + collider.gameObject.name);
            Vector2Int coliderGridPos = environment.GetGridPosFromWorld(collider.transform.position);
            Vector2Int coliderRangePos = coliderGridPos - agentGridPos;

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
                } else
                {
                    // if Seeker
                    Observation[coliderRangePos.x, coliderRangePos.y] = 2;
                }
            }
            
        }
    }

    private float GetDistance(GameObject agent, Vector3 direction, LayerMask WhatIsAGridLayer)
    {
        float distance;
        if (
            Physics.Raycast(
                agent.transform.position,
                direction,
                out RaycastHit HitInfo,
                5f,
                WhatIsAGridLayer
            )
        )
        {
            distance = Mathf.Ceil(HitInfo.distance);
        }
        else
            distance = -1;

        return distance;
    }
}
