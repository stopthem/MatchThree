using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    private float solidAlpha = 1f;
    private float clearAlpha = 0f;
    private float delay = 0f;
    private float timeToFade = 1.5f;

    private MaskableGraphic maskableGraphic;
    private void Start()
    {
        maskableGraphic = GetComponent<MaskableGraphic>();
        // FadeOff();
    }
    private IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(delay);

        maskableGraphic.CrossFadeAlpha(alpha, timeToFade, true);
    }

    public void FadeOn()
    {
        StartCoroutine(FadeRoutine(solidAlpha));
    }
    public void FadeOff()
    {
        StartCoroutine(FadeRoutine(clearAlpha));
    }
}
