using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentController : MonoBehaviour
{
    private GridBehaviour environment;

    public float moveSpeed = 0.5f;

    [SerializeField]
    private GameObject agentPrefab;

    [SerializeField]
    private LayerMask WhatIsAGridLayer;

    private readonly Dictionary<int[], Vector2Int> Returns = new();

    private readonly List<Vector2Int> possibleActions = new();

    public Vector2Int currentAction = new Vector2Int(0, 0);
    private int[] currentState;
    private Vector2Int nextAction = new Vector2Int(0, 0);
    private int[] nextState;

    private readonly int episodesCount = 1000;
    private int currentEpisode = 1;

    private List<int[]> episodeStatesList = new();
    private readonly List<Vector2Int> episodeActionsList = new();

    private int G = 0; // cummulativeReward
    private int R = 0;
    private int p = 8; // for e-greedy policy

    private int maxSteps = 400;
    private int currentStep = 0;
    private Boolean isTerminal = false;

    private Boolean isMoving = false;
    

    public Button startBtn;
    public Dropdown policyPicker;
    private bool started = false;

    public Text episode,
        timeStep,
        cummulativeReward;

    // Start is called before the first frame update
    void Start()
    {
        environment = FindObjectOfType<GridBehaviour>();

        // Init all possibles actions
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                possibleActions.Add(new(i, j));
            }
        }

        currentState = agentPrefab.GetComponent<AgentMove>().GetState();

        currentStep = 0;

        episodeStatesList.Clear();
        episodeActionsList.Clear();

        episode.text = "Episode: " + currentEpisode + " / " + episodesCount;
        timeStep.text = "Time step: " + episodeStatesList.Count;
        cummulativeReward.text = "Cummulative Reward: " + G;

        Debug.Log("Done Init!");
    }

    private void Update()
    {
        if (started)
        {
            StartRun();
            
        }
            
    }

    public void StartBtnOnClick()
    {
        started = !started;
    }

    public void StartRun()
    {
        if (!isMoving)
        {
            MoveStep();
            Debug.Log($"[{string.Join(",", currentState)}]");
        }
    }

    public void ChooseAction()
    {
        System.Random rnd = new();
        
        currentAction = possibleActions[rnd.Next(0, possibleActions.Count)];
    }

    // Function to move a step
    public void MoveStep()
    {
        Vector2Int prevPos = agentPrefab.GetComponent<AgentMove>().GetPosition();
        ChooseAction();
        ComputeNewPosition(prevPos, out Vector2Int newPos);
        Debug.Log(prevPos + " " + currentAction + " " + newPos);


        // Check the new position from boundaries and walls
        CheckWallsAndTakeAction(prevPos, newPos);
        agentPrefab.GetComponent<AgentMove>().SetState();
        currentState = agentPrefab.GetComponent<AgentMove>().GetState();

    }

    private void detachAgentToNewCell(Vector2Int prevPos)
    {
        environment.gameGrid[(int)prevPos.x, (int)prevPos.y]
                    .GetComponent<GridCell>()
                    .objectIsInGridSpace = null;
        environment.gameGrid[(int)prevPos.x, (int)prevPos.y].GetComponent<GridCell>().isOccupied =
            false;
    }

    // Compute New Position with probability 0.1 of using 0 velocity
    private void ComputeNewPosition(Vector2Int prevPos, out Vector2Int newPos)
    {
        newPos = prevPos + currentAction;
    }

    // Attach new cell to the agent
    private void AttachAgentToNewCell(int X, int Y)
    {
        environment.gameGrid[X, Y].GetComponent<GridCell>().objectIsInGridSpace = agentPrefab;
        environment.gameGrid[X, Y].GetComponent<GridCell>().isOccupied = true;
    }

    // Check Walls And Take Action
    private void CheckWallsAndTakeAction(Vector2Int prevPos, Vector2Int newPos)
    {
        bool stillInGrid = newPos.x >= 0 && newPos.x < environment.columns && newPos.y < environment.rows && newPos.y >= 0;
        if (stillInGrid)
        {
            bool wallExists = environment.gameGrid[newPos.x, newPos.y].GetComponent<GridCell>().isWall;
            // Check the new position from walls
            if (prevPos != newPos)
            {
                if (!wallExists)
                {
                    // Dettach agent from the old cell
                    detachAgentToNewCell(prevPos);
                    agentPrefab.GetComponent<AgentMove>().SetPosition(newPos);
                    // Move the agent prefab
                    Vector3 targetPosition = environment.GetworldPosFromGridPos(newPos);
                    agentPrefab.transform.position = targetPosition;
                    //StartCoroutine(MoveCoroutine(targetPosition, agentPrefab.transform.position));
                    AttachAgentToNewCell(newPos.x, newPos.y);
                    isMoving = false;
                }
            }
        }
    }

    IEnumerator MoveCoroutine(Vector3 targetPosition, Vector3 initialPosition)
    {
        isMoving = true;
        //float elapsedTime = 0f;
        float journeyLength = Vector3.Distance(initialPosition, targetPosition);
        float startTime = Time.time;

        while (agentPrefab.transform.position != targetPosition)
        {
            float distCovered = (Time.time - startTime) * moveSpeed;
            float fracJourney = distCovered / journeyLength;

            agentPrefab.transform.Translate((targetPosition - agentPrefab.transform.position) * fracJourney, Space.World);

            yield return null;
        }

        // Ensure the object reaches the target position
        //agentPrefab.transform.position = targetPosition;
    }


}
