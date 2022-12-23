using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]

    public float PV = 100f;

    public float damage = 0f;

    [SerializeField]
    int pointsOnDeath;

    [SerializeField]
    private WaveSpawner waveSpawner;


    void Start()
    {
        waveSpawner = GameObject.FindWithTag("WaveManager").GetComponent<WaveSpawner>();
    }


    public void Update()
    {
        Death();
    }
    public void Death()
    {
        if (PV <= 0)
        {
            if (waveSpawner != null)
            {
                if (waveSpawner.EnemiesAlive != 0)
                {
                    waveSpawner.EnemiesAlive -= 1;
                    Destroy(gameObject);
                }
            }
        }
    }
}
