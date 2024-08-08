using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepSpawner : MonoBehaviour
{
    [SerializeField] private float spawnTime;
    private float timer;
    [SerializeField] private Path path;
    [SerializeField] private CreepMove creep;
    // Start is called before the first frame update
    void Start()
    {
        timer = spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        CreepMove newCreep = Instantiate(creep);
        newCreep.transform.SetParent(transform);
        //newCreep.Path = path;
        timer = spawnTime;
    }
}
