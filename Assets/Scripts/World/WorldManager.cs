using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class WorldManager : MonoBehaviour
{

    [SerializeField] List<Plate> groundPlates;
    [SerializeField] GameObject platePrefab;
    [SerializeField] List<GameObject> plateSetPieces;
    [SerializeField] GameObject plateDropParticles;
    [SerializeField] float dropTimer;
    [SerializeField] float maxDropTimer;
    [SerializeField] List<NavMeshSurface> navMesh;
    [SerializeField] List<GameObject> potentialSpawns;
    [SerializeField] int waveNumber;
    [SerializeField] int enemySpawnsLeft;
    public static Action<int> OnWaveChanged;
    [SerializeField] List<Enemy> spawnedEnemies;
    [SerializeField] GameObject portalObject;
    [SerializeField] AudioSource musicSource;
    [SerializeField] GameObject healthOrb;
    [SerializeField] GameObject bigHealthOrb;
    [SerializeField] GameObject superOrb;
    [SerializeField] GameObject currentSpawn;

    // Start is called before the first frame update
    void Start()
    {
        List<Plate> foundPlates = GameObject.FindObjectsOfType<Plate>().ToList();
        foreach (Plate plate in foundPlates)
        {
            if (!groundPlates.Contains(plate))
            {
                groundPlates.Add(plate);
            }
        }
        ChangeWave(1);
        GenerateWorld(5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (waveNumber == 1)
        {
            dropTimer += Time.deltaTime;
        }
        else
        {
            dropTimer += Time.deltaTime * ((float)waveNumber * 0.5f);
        }

        if (dropTimer >= maxDropTimer)
        {
            dropTimer -= maxDropTimer;

            if (enemySpawnsLeft > 0)
            {
                if (groundPlates.Count > 0)
                {
                    int amountOfEnemiesToSpawn = UnityEngine.Random.Range(1, waveNumber);
                    for (int i = 0; i < amountOfEnemiesToSpawn; i++)
                    {
                        enemySpawnsLeft--;
                        SpawnRandomEnemy();
                    }
                    DropRandomPlate();
                }
            }
            else
            {
                if (spawnedEnemies.Count == 0) //if all enemies are dead
                {
                    ChangeWave(waveNumber + 1);
                }
            }


        }
    }

    void ChangeWave(int number)
    {
        waveNumber = number;
        if (waveNumber > 5)
        {
            portalObject.SetActive(true);
            OnWaveChanged?.Invoke(waveNumber);
            GenerateWorld(5, 5);
            return;
        }
        enemySpawnsLeft = waveNumber * 5; //5
        dropTimer = -8f;
        if (waveNumber == 3)
        {
            SpawnOrb(bigHealthOrb);
        }
        else if (waveNumber != 1)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                SpawnOrb(healthOrb);
            }
            else
            {
                SpawnOrb(superOrb);
            }
        }
        OnWaveChanged?.Invoke(waveNumber);
        GenerateWorld(5, 5);
    }
    void DropRandomPlate()
    {
        Plate dropPlate = null;
        bool validPlate = false;
        while (!validPlate)
        {
            dropPlate = groundPlates[UnityEngine.Random.Range(0, groundPlates.Count)];
            bool valid = true;
            foreach (Enemy enemy in spawnedEnemies)
            {
                if (Vector3.Distance(enemy.transform.position, dropPlate.transform.position) < 4f)
                {
                    valid = false;
                }
            }
            if (valid)
            {
                validPlate = true;
            }
        }
        StartCoroutine(DropPlate(dropPlate, 2f));
    }

    void SpawnRandomEnemy()
    {
        Vector3 spawnPosition = groundPlates[UnityEngine.Random.Range(0, groundPlates.Count)].transform.position + new Vector3(0, 1);
        spawnPosition += new Vector3(UnityEngine.Random.Range(-4, 4), 0, UnityEngine.Random.Range(-4, 4));
        GameObject spawnedEnemyObject = Instantiate(potentialSpawns[UnityEngine.Random.Range(0, potentialSpawns.Count)], spawnPosition, Quaternion.identity);
        Enemy spawnedEnemy = spawnedEnemyObject.GetComponent<Enemy>();
        spawnedEnemies.Add(spawnedEnemy);
        spawnedEnemy.OnEnemyDeath += HandleEnemyDeath;
    }

    void HandleEnemyDeath(Enemy enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }
    }

    IEnumerator DropPlate(Plate plate, float delay)
    {
        GameObject particles = Instantiate(plateDropParticles, plate.transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(particles, 2f);
        plate.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
        plate.TelegraphDrop();
        yield return new WaitForSeconds(delay);
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, plate.transform.position) < 4f)
            {
                enemy.ForceDie();
            }
        }
        NavMeshObstacle navMeshObstacle = plate.transform.GetChild(0).gameObject.AddComponent<NavMeshObstacle>();
        navMeshObstacle.size = new Vector3(4, 1, 4);
        navMeshObstacle.carving = true;
        groundPlates.Remove(plate);
        plate.StartDrop(-4);
        RegenerateNavMesh();
        if (!musicSource.isPlaying) musicSource.Play(); //bad programming - oh well
        yield return null;
    }

    void SpawnPlate(Vector3 location)
    {
        bool found = false;
        foreach (Plate plate in groundPlates)
        {
            if (plate.transform.position.x == location.x && plate.transform.position.z == location.z)
            {
                found = true;
            }
        }
        if (!found)
        {
            GameObject spawnedObject;

            if (UnityEngine.Random.value > 0.6f && (location != new Vector3(0, -15, 0)))
            {
                spawnedObject = Instantiate(plateSetPieces[UnityEngine.Random.Range(0, plateSetPieces.Count)], location, Quaternion.identity);
                spawnedObject.transform.rotation = GetRandomRotation();
            }
            else
            {
                spawnedObject = Instantiate(platePrefab, location, Quaternion.identity);
            }
            groundPlates.Add(spawnedObject.GetComponent<Plate>());
            spawnedObject.GetComponent<Plate>().StartDrop(4f);
        }
    }

    void RegenerateNavMesh()
    {
        foreach (NavMeshSurface surface in navMesh)
        {
            surface.BuildNavMesh();
        }
    }

    IEnumerator DelayedNavMeshGeneration()
    {
        yield return new WaitForSeconds(8f);
        RegenerateNavMesh();
        yield return null;
    }

    void GenerateWorld(int width, int height)
    {
        int xOffset = Mathf.FloorToInt(width / 2f);
        int zOffset = Mathf.FloorToInt(height / 2f);

        Debug.Log(xOffset + ", " + zOffset);

        for (int x = -xOffset; x < width - xOffset; x++)
        {
            for (int z = -zOffset; z < height - zOffset; z++)
            {
                bool found = false;
                foreach (Plate plate in groundPlates)
                {
                    if (plate.transform.position == new Vector3(x * 8, 0, z * 8))
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    SpawnPlate(new Vector3(x * 8, -15, z * 8));

                }
            }
        }

        StartCoroutine(DelayedNavMeshGeneration());
    }

    Quaternion GetRandomRotation()
    {
        int rotation = UnityEngine.Random.Range(0, 4);
        return Quaternion.Euler(0, rotation * 90, 0);
    }

    void SpawnOrb(GameObject gameObject)
    {
        if (currentSpawn == null)
        {
            currentSpawn = Instantiate(gameObject, new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}
