using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timeLeftText;
    public Image clockImage;

    private int maxTime = 60;

    public int flashTimeLimit = 10;
    public AudioClip flashBeep;
    public float flashInterval = 1f;
    public Color flashColor = Color.red;

    private IEnumerator flashRoutine;

    public bool paused = false;

    public void InitTimer(int maxTime = 60)
    {
        this.maxTime = maxTime;
        if (timeLeftText != null)
        {
            timeLeftText.text = maxTime.ToString();
        }
    }
    public void UpdateTimer(int currentTime)
    {
        if (paused)
        {
            return;
        }

        if (clockImage != null)
        {
            clockImage.fillAmount = (float)currentTime / (float)maxTime;
            if (currentTime <= flashTimeLimit)
            {
                flashRoutine = FlashRoutine(clockImage,flashColor,flashInterval);
                StartCoroutine(flashRoutine);

                if (SoundManager.Instance != null && flashBeep)
                {
                    SoundManager.Instance.PlayClipAtPoint(flashBeep,Vector3.zero,SoundManager.Instance.fxVolume,false);
                }
            }
        }

        if (timeLeftText != null)
        {
            timeLeftText.text = currentTime.ToString();
        }

    }

    private IEnumerator FlashRoutine(Image image,Color targetColor,float interval)
    {
        if (image != null)
        {
            Color originalColor = image.color;
            image.CrossFadeColor(targetColor,interval * .3f,true,true);
            yield return new WaitForSeconds(interval * .5f);

            image.CrossFadeColor(originalColor,interval * .3f,true,true);
            yield return new WaitForSeconds(interval * .5f);
        }
    }
    public void FadeOff()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        ScreenFader[] screenFaders = GetComponentsInChildren<ScreenFader>();
        foreach (ScreenFader fader in screenFaders)
        {
            fader.FadeOff();
        }
    }
}
