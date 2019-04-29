using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns;
    public int rows;

    public Count wallCount = new Count(0, 0);
    public Count foodCount = new Count(0, 0);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        gridPositions.Clear();
        for(int x = 1; x < columns - 1; x++)
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for(int x = -1; x < columns + 1; x++)
            for(int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[GameManager.pseudoRandom.Next(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[GameManager.pseudoRandom.Next(0, outerWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
    }

    Vector3 RandomPosition()
    {
        int RandomIndex = GameManager.pseudoRandom.Next(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[RandomIndex];
        gridPositions.RemoveAt(RandomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = GameManager.pseudoRandom.Next(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[GameManager.pseudoRandom.Next(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level * 2, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}