using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    // DEVNOTE: You can run multiple ObjectSpawners in your scene. They can share the same spawn-points(??)
    // Or you can setup different (groups of) spawn-points with different tags... adjust tag string below:
    string tagForSpawnPoints = "SpawnPoint";

    [Tooltip("Drop GameObject prefab to be spawned here:")] [SerializeField] GameObject gameObjectToSpawn;
    [Tooltip("Delay between spawn, in s")]                  [SerializeField] float delayBetweenSpawn = 1;
    [Tooltip("Delay between re-spawn, in s")]               [SerializeField] float delayBetweenRespawn = 1;
    [Tooltip("Delay between waves, in s")]                  [SerializeField] float delayBetweenWaves = 1;
    [Tooltip("Number of waves, 0 = infinite")]              [SerializeField] int numberOfWaves = 0;

    private GameObject[] spawnPoints;
    private int thisWave = 1;
    private int numberOfSpawnedObjects = 0;
    private bool respawn = false;
    private bool debugMode = false;

    private void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag(tagForSpawnPoints);

        Debug.Log("ObjectSpawner.cs report: " +  spawnPoints.Length + 
            " spawn-points registered with tag '" + tagForSpawnPoints + "'.");

        debugMode = Debug.isDebugBuild;
    }

    private void Update()
    {
        if (SpawnPointsAreEmpty() && !respawn) TriggerRespawn();
        if (SpawnPointsAreFull()) respawn = false;

        if (debugMode && Input.GetKeyDown(KeyCode.P)) DespawnAll();
    }

    void FillPosition(Transform pos)
    {
        Debug.Log("Fill " + pos);
        GameObject spawnedObject = 
            Instantiate(gameObjectToSpawn, pos.transform.position, Quaternion.identity) as GameObject;
        spawnedObject.transform.parent = pos;
        numberOfSpawnedObjects++;
    }

    bool SpawnPointsAreEmpty()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount > 0) return false;
        }
        return true;
    }

    bool SpawnPointsAreFull()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount == 0) return false;
        }
        return true;
    }
    
    GameObject RandomFreePosition()
    {
        GameObject[] emptySpawnPoints = new GameObject[spawnPoints.Length];
        int inCount = 0;
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount == 0)
            {
                emptySpawnPoints[inCount] = spawnPoint;
                inCount++;
            }
        }
        if (inCount > 0) return emptySpawnPoints[Random.Range(0, inCount)];
        else return null;
    }

    void SpawnAllSpawnpoints()
    {
        Debug.Log("Begin to fill all spawn points @ " + Time.time);
        GameObject freePos = RandomFreePosition();
        if (freePos) FillPosition(freePos.transform);

        if (RandomFreePosition()) Invoke("SpawnAllSpawnpoints", delayBetweenSpawn);
        else if (SpawnPointsAreFull() && thisWave <= numberOfWaves)
        {
            thisWave++;
            respawn = false;
        }
    }

    public void DespawnAll()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount != 0)
            {
                Destroy(spawnPoint.transform.GetChild(0).gameObject);
            }
        }
    }

    public void TriggerRespawn()
    {
        if ((thisWave <= numberOfWaves) || numberOfWaves == 0)
        {
            if (!respawn) Invoke("SpawnAllSpawnpoints", delayBetweenRespawn);
            respawn = true;
        }
    }
}
