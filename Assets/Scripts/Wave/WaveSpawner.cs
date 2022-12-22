using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

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


    // protected override void OnNetworkSpawn()
    // {
    //     // enabled = IsServer;            
    //     // if (!enabled)
    //     // {
    //     //     return;
    //     // }
    //     // // Instantiate the GameObject Instance
    //     // m_PrefabInstance = Instantiate(PrefabToSpawn);
    //     //     
    //     // // Optional, this example applies the spawner's position and rotation to the new instance
    //     // m_PrefabInstance.transform.position = transform.position;
    //     // m_PrefabInstance.transform.rotation = transform.rotation;
    //         
    //     
    // }


    public List<GameObject> spawnedEnemy = new List<GameObject>();
    public Transform[] spawningPoints = new Transform[0];
    
    [SerializeField]
    private float TimeBetweenWaves = 5f;

    private float countdown = 5f;

    public int waveIndex = 0;

    public int EnemiesAlive = 0;

  
    private void Start()
    {
        /*GameObject[] _tempSpawn = GameObject.FindGameObjectsWithTag("Spawner");
        for(int i = 0; i < _tempSpawn.Length; i++)
        {
            spawningPoints[i] = _tempSpawn[i].transform;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
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
       //Debug.Log("timer = " + countdown/TimeBetweenWaves);
    }

  


    IEnumerator SpawnWave()
    {
        waveIndex++;
       
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
        Debug.Log(EnemiesAlive);
    }
}
