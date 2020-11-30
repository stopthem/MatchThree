using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreStar : MonoBehaviour
{
    public Image starImage;
    public ParticlePlay starFX;
    public float delay = .5f;
    public AudioClip starSound;
    public bool activated = false;
    private void Start()
    {
        SetActive(false);
        // StartCoroutine(TestRoutine());
    }

    private void SetActive(bool state)
    {
        if (starImage != null)
        {
            starImage.gameObject.SetActive(state);
        }
    }

    public void Activate()
    {
        if (activated)
        {
            return;
        }
        StartCoroutine(ActivateRoutine());
    }
    private IEnumerator ActivateRoutine()
    {
        activated = true;

        if (starFX != null)
        {
            starFX.Play();
        }

        if (SoundManager.Instance != null && starSound != null)
        {
            SoundManager.Instance.PlayClipAtPoint(starSound, Vector3.zero, SoundManager.Instance.fxVolume);
        }
        yield return new WaitForSeconds(delay);

        SetActive(true);

    }
    // IEnumerator TestRoutine()
    // {
    //     yield return new WaitForSeconds(3f);
    //     Activate();
    // }
}
