using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VFXLifetime : MonoBehaviour
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
            Destroy(gameObject);
        }
    }
}
