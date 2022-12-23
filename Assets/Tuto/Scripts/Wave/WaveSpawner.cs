using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class WaveSpawner : NetworkBehaviour
{
    public bool DestroyWithSpawner;        
    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;
    
    
    private static WaveSpawner instance;
    public static WaveSpawner Instance    
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WaveSpawner>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    public List<GameObject> spawnedEnemy = new List<GameObject>();
    public Transform[] spawningPoints = new Transform[0];
    
    [SerializeField]
    private float TimeBetweenWaves = 5f;

    [SerializeField]
    private GameObject waveCountText;

    private float countdown;

    private bool countdownStarted = false;

    public int waveIndex = 0;

    public int EnemiesAlive = 0;

    private void Start()
    {
        countdown = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer){
            if (EnemiesAlive > 0)
            {
                return;
            }
            else 
            {
                EnemyController.isMove = false;
            }

            if (countdown <= 0f)
            {
            
                StartCoroutine(SpawnWave());
                countdown = TimeBetweenWaves;
            }
            countdown -= Time.deltaTime;
        Debug.Log("timer = " + countdown);
        } else {
            // Debug.Log(IsHost);
        }
    }

  


    IEnumerator SpawnWave()
    {
        waveIndex++;
        waveCountText.GetComponent<TextMeshPro>().text = waveIndex.ToString();
       
        for ( int i = 0; i < waveIndex; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
            EnemyController.isMove = true;
            
        }

    }

    void SpawnEnemy()
    {
        int random = Random.Range(0, spawnedEnemy.Count);
        int random1 = Random.Range(0, spawningPoints.Length);
        m_PrefabInstance = Instantiate(spawnedEnemy[random], spawningPoints[random1].transform.position, spawningPoints[random1].rotation);
        // Get the instance's NetworkObject and Spawn
        m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
        m_SpawnedNetworkObject.Spawn();
        EnemiesAlive++;
    }
}
