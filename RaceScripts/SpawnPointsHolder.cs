using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsHolder : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;

    private List<bool> spawnPointOccupied = new List<bool>() { false, false, false, false, false, false , false, false};




    public Vector3 GetRandomUnoccupiedSpawnPositionAndMarkItAsOccupied(int numberOfPlayers)
    {
        bool occupied = true;
        Vector3 result = Vector3.zero;

        while (occupied)
        {
            int index = Random.Range(0, numberOfPlayers);
            if (!spawnPointOccupied[index])
            {
                occupied = false;
                result = spawnPoints[index].position;
                spawnPointOccupied[index] = true;
            }
        }
        return result;
    }
}
