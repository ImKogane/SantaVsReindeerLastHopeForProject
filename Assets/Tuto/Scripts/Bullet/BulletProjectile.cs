using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{



    [SerializeField] private GameObject VFX;

    [Header("Spell stats")]
    [SerializeField] private int bulletDamage;
    [Range(15f, 30f)]
    [SerializeField] private float bulletSpeed;


    private Rigidbody bulletRigidbody;


    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
        bulletRigidbody.velocity = transform.forward * bulletSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<BulletTarget>() != null)
        {
            
            //Hit Target
            Instantiate(VFX, transform.position, Quaternion.identity);
            EnemyStats targetEnemy = other.GetComponent<EnemyStats>();
            targetEnemy.PV -= bulletDamage;
        }
        else
        {
            //HitSomethingElse
            Instantiate(VFX, transform.position, Quaternion.identity);
            
        }
        Destroy(gameObject);
    }

    
}

