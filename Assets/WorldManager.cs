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
    [SerializeField] GameObject plateDropParticles;
    [SerializeField] float dropTimer;
    [SerializeField] float maxDropTimer;
    [SerializeField] NavMeshSurface navMesh;

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

        GenerateWorld(5, 5);
    }

    // Update is called once per frame
    void Update()
    {
        dropTimer += Time.deltaTime;
        if (dropTimer >= maxDropTimer)
        {
            dropTimer -= maxDropTimer;
            if (groundPlates.Count > 0)
            {
                DropRandomPlate();
            }
        }
    }
    void DropRandomPlate()
    {
        Plate dropPlate = groundPlates[UnityEngine.Random.Range(0, groundPlates.Count)];
        StartCoroutine(DropPlate(dropPlate, 2f));
    }

    IEnumerator DropPlate(Plate plate, float delay)
    {
        GameObject particles = Instantiate(plateDropParticles, plate.transform.position, Quaternion.Euler(-90, 0, 0));
        Destroy(particles, 2f);
        yield return new WaitForSeconds(delay);
        NavMeshObstacle navMeshObstacle = plate.transform.GetChild(0).gameObject.AddComponent<NavMeshObstacle>();
        navMeshObstacle.size = new Vector3(4, 1, 4);
        navMeshObstacle.carving = true;
        groundPlates.Remove(plate);
        plate.StartDrop();
        navMesh.BuildNavMesh();
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
                    GameObject spawnedObject = Instantiate(platePrefab, new Vector3(x * 8, 0, z * 8), Quaternion.identity);
                    groundPlates.Add(spawnedObject.GetComponent<Plate>());
                }
            }
        }

        navMesh.BuildNavMesh();
    }
}
