using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using Photon.Pun;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private AnimationClip clip;

    private Transform movePositionTransform;
    
    public GameObject[] players = new GameObject[0];

    private NavMeshAgent agent;

    private EnemyStats enemyStats;

    public float cooldown;

    public static bool isMove;

    private bool canMove;
    [SerializeField] private bool canAttack;
    private bool inAttack;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();
    
    }

    void Start()
    {
        cooldown = 0;
        canAttack = true;
        inAttack = false;
        canMove = true;
    }


    // Update is called once per frame
    void Update()
    {
        GetNearestPlayer();
        if (canMove /*&& PhotonNetwork.IsMasterClient*/ == true)
        {
            agent.destination = movePositionTransform.position;
        }
        CheckCooldown();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(canAttack)
            {
                StartCoroutine(Attack(collision.gameObject));
            }
        }
        
        return;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {


        }
        return;
    }

    void CheckCooldown()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0)
            {
                canAttack = true;
                inAttack = false;
                canMove = true;
                anim.ResetTrigger("Attack");
            }
        }
    }

    void SetCooldown()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "Attack")
            {
                cooldown = clip.length;
                break;
            }
        }
    }

    IEnumerator Attack(GameObject player)
    {
        anim.SetTrigger("Attack");
        canAttack = false;
        inAttack = true;
        //canMove = false;
        SetCooldown();
        ThirdPersonController thirdPerson = player.GetComponent<ThirdPersonController>();
//        thirdPerson.PV -= enemyStats.damage;
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Attack");
    }

    private void GetNearestPlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject i in players)
        {
            float _tempPos = Vector3.Distance(i.transform.position, transform.position);
            if(_tempPos < 2)
            {
                movePositionTransform = i.transform;
                //isMove = false;
            } else {
                int _tempRand = UnityEngine.Random.Range(0, players.Length);
                movePositionTransform = players[_tempRand].transform;
            }
        }
    }
}
