using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    // DEVNOTE: You can run multiple ObjectSpawners in your scene. They can share the same 
    // spawn-points(? untested) or you can setup different (groups of) spawn-points with different tags... 
    // adjust tag string here:
    [Tooltip("Tag used to denote spawn-points for this ObjectSpawner:")]
    [SerializeField] string tagForSpawnPoints = "SpawnPoint";

    // DEVNOTE: These debugging commands work in the editor or on "debug" builds. 
    // Assign them to keys not in-use [in the Start() method]:
    private bool despawnCommand;
    private bool spawnAllCommand;
    private bool spawnRandomCommand;

    // DEVNOTE: simplified mode, i.e. fill a random empty spawn-point every N seconds:
    [Tooltip("Fills a random empty spawn-point, every N")] [SerializeField] bool simplified = true;
    [Tooltip("time between simple spawn, in s")] [SerializeField] float simpleDelay = 10;

    // DEVNOTE: Not all features are fully implemented, sorry. Feel free to fork and suggest improvements.
    [Tooltip("Drop GameObject prefab-to-be-spawned here:")] [SerializeField] GameObject[] gameObjectsToSpawn;
    [Tooltip("Delay between re-spawn, in s")] [SerializeField] float delayBetweenRespawn = 1;
    [Tooltip("Delay between spawn, in s")] [SerializeField] float delayBetweenSpawn = 1;
    [Tooltip("Delay between waves, in s")] [SerializeField] float delayBetweenWaves = 1;
    [Tooltip("Number of waves, 0 = infinite")] [SerializeField] int numberOfWaves = 0;

    private bool debugMode = false;
    private bool respawn = false;
    private float spawnTime = 0;
    private int thisWave = 1;
    private GameObject[] spawnPoints;

    private void Start()
    {
        debugMode = Debug.isDebugBuild;

        spawnPoints = GameObject.FindGameObjectsWithTag(tagForSpawnPoints);

        var a = spawnPoints.Length;
        var b = tagForSpawnPoints;
        Debug.Log("ObjectSpawner.cs report: " + a + " spawn-points registered with tag '" + b + "'.");

        Invoke("SpawnAllSpawnpointsInstantly", 2);
    }

 #region Updates...
    private void Update()
    {
        if (debugMode) TryDebug();

        if (simplified) SimplifiedUpdate();
        else StandardUpdate();
    }

    private void SimplifiedUpdate()
    {
        var timeNow = Time.time;
        if (timeNow > spawnTime + simpleDelay)
        {
            SpawnRandomSpawnpoint();
            spawnTime = timeNow;
        }
    }

    private void StandardUpdate()
    {
        if (SpawnPointsAreEmpty() && !respawn) TriggerRespawn();
        if (SpawnPointsAreFull()) respawn = false;
    }
#endregion

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

    private void FillPosition(Transform position)
    {
        // Debug.Log(Time.time + " :ObjectSpawner.cs: FillPosition(" + position + ")");
        var n = gameObjectsToSpawn.Length;
        var a = gameObjectsToSpawn[Random.Range(0, n)];
        var b = position.transform.position;
        var c = Quaternion.identity;
        GameObject spawnedObject = Instantiate(a, b, c) as GameObject;
        spawnedObject.transform.parent = position;
        spawnedObject.SetActive(true);
    }

    // DEVNOTE: to enable / disable various debug features, swap the desired lines and keys here:
    private void PollDebugKeys()
    {
        // Set keys here for enabled commands, comment-out and replace with '*Command = false' line to disable.
        despawnCommand = Input.GetKeyDown(KeyCode.P);
        spawnAllCommand = Input.GetKeyDown(KeyCode.I);
        spawnRandomCommand = Input.GetKeyDown(KeyCode.O);

        // Set commands to false to fully disable them
        // despawnCommand = false;
        // spawnAllCommand = false;
        // spawnRandomCommand = false;
    }

    private GameObject RandomFreePosition()
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

    public void SpawnAllSpawnpoints()
    {
        GameObject freePos = RandomFreePosition();
        if (freePos) FillPosition(freePos.transform);

        if (RandomFreePosition()) Invoke("SpawnAllSpawnpoints", delayBetweenSpawn);
        else if (SpawnPointsAreFull() && thisWave <= numberOfWaves)
        {
            thisWave++;
            respawn = false;
        }
    }

    public void SpawnAllSpawnpointsInstantly()
    {
        GameObject freePos = RandomFreePosition();
        if (freePos) FillPosition(freePos.transform);

        if (RandomFreePosition()) Invoke("SpawnAllSpawnpointsInstantly", 0.01f);
        else if (SpawnPointsAreFull() && thisWave <= numberOfWaves)
        {
            thisWave++;
            respawn = false;
        }
    }

    public bool SpawnPointsAreEmpty()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount > 0) return false;
        }
        return true;
    }

    public bool SpawnPointsAreFull()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (spawnPoint.transform.childCount == 0) return false;
        }
        return true;
    }

    public void SpawnRandomSpawnpoint()
    {
        GameObject freePos = RandomFreePosition();
        if (freePos) FillPosition(freePos.transform);
    }

    public void TriggerRespawn()
    {
        if ((thisWave <= numberOfWaves) || numberOfWaves == 0)
        {
            if (!respawn) Invoke("SpawnAllSpawnpoints", delayBetweenRespawn);
            respawn = true;
        }
    }

    private void TryDebug()
    {
        PollDebugKeys();
        if (despawnCommand) DespawnAll();
        if (spawnRandomCommand) SpawnRandomSpawnpoint();
        if (spawnAllCommand) SpawnAllSpawnpointsInstantly();
    }
}
