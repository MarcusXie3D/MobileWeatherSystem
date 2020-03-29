using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowParticles : MonoBehaviour
{
    private Quaternion rotation;

    void Awake()
    {
        rotation = Quaternion.identity;
        transform.rotation = rotation;
    }

    void Update()
    {
        transform.rotation = rotation;//keep the snow always falling vertically instead of rotating with camera(parent)
    }
}
