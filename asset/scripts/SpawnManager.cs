using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab; // Drag the cube prefab here
    [SerializeField] private GameObject capsulePrefab; // Drag the capsule prefab here

    [SerializeField] private Vector3 spawnAreaSize = new Vector3(50f, 0, 50f); // Define the size of the spawning area
    [SerializeField] private float spawnIntervalMin = 5f; // Minimum spawn interval
    [SerializeField] private float spawnIntervalMax = 10f; // Maximum spawn interval

    [SerializeField] private int minObjects = 4; // Minimum number of objects in the scene
    [SerializeField] private int maxObjects = 12; // Maximum number of objects in the scene

    private int currentObjectsCount = 0; // Current number of spawned objects

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnRandomObject();
        }
        StartCoroutine(SpawnObjects());
    }

    private IEnumerator SpawnObjects()
    {
        while (true)
        {
            // Wait for random interval before next spawn
            float waitTime = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(waitTime);

            // Check if we're below the minimum object count
            while (currentObjectsCount < minObjects)
            {
                SpawnRandomObject();
                yield return new WaitForSeconds(0.1f); // Small delay to not spawn all at once
            }

            // Regular spawning if below maxObjects
            if (currentObjectsCount < maxObjects)
            {
                SpawnRandomObject();
            }
        }
    }

    private void SpawnRandomObject()
    {
        // Randomly choose between cube and capsule
        GameObject objectToSpawn = Random.Range(0f, 1f) > 0.5f ? cubePrefab : capsulePrefab;

        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            1f, // Assuming all objects spawn at ground level
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        currentObjectsCount++;

        // Optionally, if you want the spawned objects to be destroyed after some time, you can add:
        // Destroy(spawnedObject, someTimeDuration);

        // Registering the spawned object's destruction so we can update our count
        spawnedObject.AddComponent<SpawnedObject>().OnObjectDestroyed += () => { currentObjectsCount--; };
    }
}

// Extra class to track the spawned object's lifecycle
public class SpawnedObject : MonoBehaviour
{
    public delegate void ObjectDestroyedAction();
    public event ObjectDestroyedAction OnObjectDestroyed;

    private void OnDestroy()
    {
        OnObjectDestroyed?.Invoke();
    }
}
