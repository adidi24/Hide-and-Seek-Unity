using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehaviour : MonoBehaviour
{
    public int rows = 20;
    public int columns =20;
    public float scale = 1.1f;
    [SerializeField] private GameObject gridPrefab;
    public GameObject[,] gameGrid;
    [SerializeField] private Transform _cam;
    public List<GridCell> startingLine;
    public List<GridCell> finishLine;
    public bool usingPredefinedEnv = true;
    public string predefinedEnvType = "env20x20-1";

    // Start is called before the first frame update
    void Start()
    {
        if (gridPrefab)
            GenerateGrid();
        else print("missing grid prefab, please assign");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateGrid()
    {
        gameGrid = new GameObject[columns, rows];
        for (int y = 0; y < columns; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                gameGrid[x, y] = Instantiate(gridPrefab, new Vector3(x * scale, y * scale), Quaternion.identity);
                gameGrid[x, y].GetComponent<GridCell>().setPosition(x, y);
                gameGrid[x, y].transform.parent = transform;
                gameGrid[x, y].gameObject.name = "Grid Space (x :" + x.ToString() + ", y : " + y.ToString() + ")";

            }

        }
        if (usingPredefinedEnv)
        {
            SetPredefinedGrid(predefinedEnvType);
        }
        _cam.transform.position = new Vector3(rows * scale / 2, -3, -19);
        _cam.transform.Rotate(-34, 0, 0);
    }

    public void SetPredefinedGrid(string type = "env20x20-1")
    {
        PredefinedEnvs predefinedEnv = new();
        switch (type)
        {
            case "env20x20-2":
                if (gameGrid.GetLength(0) != gameGrid.GetLength(1) || gameGrid.GetLength(0) != 20)
                {
                    throw new ArgumentException("grid size mismatch with type 20x20.",nameof(type));
                } else
                {
                    makeWalls(type, predefinedEnv);
                }

                break;
            case "env40x40":
                if (gameGrid.GetLength(0) != gameGrid.GetLength(1) || gameGrid.GetLength(0) != 40)
                {
                    throw new ArgumentException("grid size mismatch with type 40x40.", nameof(type));
                }
                else
                {
                    makeWalls(type, predefinedEnv);
                }
                break;
            default:
                if (gameGrid.GetLength(0) != gameGrid.GetLength(1) || gameGrid.GetLength(0) != 20)
                {
                    throw new ArgumentException("grid size mismatch with type.", nameof(type));
                }
                else
                {
                    makeWalls(type, predefinedEnv);
                };
                break;
        };
    }

    private void makeWalls(string type, PredefinedEnvs predefinedEnv)
    {
        foreach (var penv in predefinedEnv.envs[type])
        {
            gameGrid[penv.Item1, penv.Item2].GetComponent<GridCell>().GetComponent<MeshRenderer>().material.color = Color.grey;
            gameGrid[penv.Item1, penv.Item2].GetComponent<GridCell>().transform.localScale = new Vector3(1, 1, 2f);
            Vector3 pos = gameGrid[penv.Item1, penv.Item2].GetComponent<GridCell>().transform.position;
            gameGrid[penv.Item1, penv.Item2].GetComponent<GridCell>().transform.position = new Vector3(pos.x, pos.y, -0.5f);
            gameGrid[penv.Item1, penv.Item2].GetComponent<GridCell>().isWall = true;
        }
    }

    public Vector2Int GetGridPosFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / scale);
        int y = Mathf.FloorToInt(worldPosition.y / scale);

        x = Mathf.Clamp(x, 0, columns);
        y = Mathf.Clamp(y, 0, rows);

        return new Vector2Int(x, y);
    }


    public Vector3 GetworldPosFromGridPos(Vector2Int gridPos)
    {
        float x = gridPos.x * scale;
        float y = gridPos.y * scale;
        return new Vector3(x, y, -1.3f);
    }


}
