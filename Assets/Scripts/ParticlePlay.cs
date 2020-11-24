using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    public ParticleSystem[] particleArray;
    public float lifeTime = 1f;
    public bool destroyImmedietly = true;
    // Start is called before the first frame update
    void Start()
    {
        particleArray = GetComponentsInChildren<ParticleSystem>();
        if (destroyImmedietly)
        {
            Destroy(gameObject,lifeTime);
        }
    }

    public void Play()
    {
        foreach (ParticleSystem particleSystem in particleArray)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
        Destroy(gameObject,lifeTime);
    }
}
