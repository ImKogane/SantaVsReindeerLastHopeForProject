using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Netcode;

public class VFXLifetime : NetworkBehaviour
{
    private float lifetime = 0.3f;

    void Update()
    {
        if (lifetime > 0)
        {
            lifetime -= Time.deltaTime;
        }

        if (lifetime <= 0)
        {
            DestroyVFXServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyVFXServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
