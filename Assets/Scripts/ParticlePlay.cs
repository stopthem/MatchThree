using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlay : MonoBehaviour
{
    public ParticleSystem[] particleArray;
    public float lifeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        particleArray = GetComponentsInChildren<ParticleSystem>();
        Destroy(gameObject,lifeTime);
    }

    public void Play()
    {
        foreach (ParticleSystem particleSystem in particleArray)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }
}
