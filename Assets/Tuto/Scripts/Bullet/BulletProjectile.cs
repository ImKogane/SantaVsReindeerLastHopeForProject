using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class BulletProjectile : NetworkBehaviour
{

    private GameObject m_PrefabInstance;
    private NetworkObject m_SpawnedNetworkObject;
    [SerializeField] private GameObject VFX;

    [Header("Spell stats")]
    [SerializeField] private int bulletDamage;
    [Range(15f, 30f)]
    [SerializeField] private float bulletSpeed;


    private Rigidbody bulletRigidbody;

    private float maxLifetime = 5;
    private float bTime;


    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        bulletRigidbody.velocity = transform.forward * bulletSpeed;
        bTime = maxLifetime;
    }

    private void Update()
    {
        if (IsOwner)
        {
            bTime -= Time.deltaTime;
            if (bTime <= 0) 
            {
                DestroyServerRpc();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<BulletTarget>() != null)
        {
            
            //Hit Target
            m_PrefabInstance = Instantiate(VFX, transform.position, Quaternion.identity);
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            m_SpawnedNetworkObject.Spawn();
            EnemyStats targetEnemy = other.GetComponent<EnemyStats>();
            targetEnemy.PV -= bulletDamage;
        }
        else
        {
            //HitSomethingElse
            m_PrefabInstance = Instantiate(VFX, transform.position, Quaternion.identity);
            m_SpawnedNetworkObject = m_PrefabInstance.GetComponent<NetworkObject>();
            m_SpawnedNetworkObject.Spawn();
        }

        DestroyServerRpc();

    }

    [ServerRpc]
    public void DestroyServerRpc()
    {

        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }

    
}

