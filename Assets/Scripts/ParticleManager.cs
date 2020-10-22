using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject clearFXPrefab;
    public GameObject breakFXPrefab;
    public GameObject doublebreakFXPrefab;

    public void ClearPieceFXAt(int x, int y, int z = 0)
    {
        if (clearFXPrefab != null)
        {
            GameObject clearFX = Instantiate(clearFXPrefab, new Vector3(x,y,z), Quaternion.identity);

            ParticlePlay particlePlay = clearFX.GetComponent<ParticlePlay>();

            if (particlePlay != null)
            {
                particlePlay.Play();
            }
        }
    }

    public void BreakTileFXAt(int breakableValue, int x, int y, int z = 0)
    {
        GameObject breakFX = null;
        ParticlePlay particlePlay = null;

        if (breakableValue > 1)
        {
            if (doublebreakFXPrefab != null)
            {
                breakFX = Instantiate(doublebreakFXPrefab, new Vector3(x,y,z), Quaternion.identity);
            }
        }
        else
        {
            if (breakFXPrefab != null)
            {
                breakFX = Instantiate(breakFXPrefab, new Vector3(x,y,z), Quaternion.identity);
            }
        }
        if (breakFX != null)
        {
            particlePlay = breakFX.GetComponent<ParticlePlay>();

            if (particlePlay != null)
            {
                particlePlay.Play();
            }
        }
    }
}

