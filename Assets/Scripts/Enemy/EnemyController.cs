using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.Netcode.Components;

public class EnemyController : NetworkBehaviour
{
    [SerializeField] private Animator anim;

    [SerializeField] private AnimationClip clip;

    private ThirdPersonController playerController;

    private Transform movePositionTransform;
    
    public GameObject[] players = new GameObject[0];

    private NavMeshAgent agent;

    private EnemyStats enemyStats;

    // private NetworkVariable<float> PV = new NetworkVariable<float>();

    public float cooldown;

    public static bool isMove;

    private bool canMove;
    [SerializeField] private bool canAttack;
    private bool inAttack;

    // public override void OnNetworkSpawn()
    // {
    //     if (IsServer)
    //     {
    //         PV = enemyStats.PV;
    //     }
    // }

    // private void OnPVChanged()
    // {
    //     enemyStats.PV = PV;
    // }

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
        if (canMove == true)
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
            canAttack = true;
             
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
        AttackServerRpc(player.GetComponent<NetworkObject>().OwnerClientId);
        yield return new WaitForSeconds(0.3f);
        anim.ResetTrigger("Attack");
    }

    [ServerRpc]
    private void AttackServerRpc(ulong clientId)
    {
        var client = NetworkManager.Singleton.ConnectedClients[clientId]
            .PlayerObject.GetComponent<ThirdPersonController>();
        client.PV -= enemyStats.damage;
        client.UpdateHealthBar();
        Debug.Log(client.PV);

        NotifyHealthChangedClientRpc(enemyStats.damage, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    private void NotifyHealthChangedClientRpc(float damage, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;

        Debug.Log($"Client got damage: {damage}");
        playerController.PV -= damage;
        Debug.Log($"Client HP: {playerController.PV}");
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
                playerController = i.GetComponent<ThirdPersonController>();
                //isMove = false;
            } else {
                int _tempRand = UnityEngine.Random.Range(0, players.Length);
                movePositionTransform = players[_tempRand].transform;
                playerController = players[_tempRand].GetComponent<ThirdPersonController>();
            }
        }
    }
}
