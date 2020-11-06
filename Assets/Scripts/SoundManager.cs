using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public AudioClip[] musicClips;
    public AudioClip[] winClips;
    public AudioClip[] loseClips;
    public AudioClip[] bonusClips;

    public float lowPitch = .95f,highPitch = 1.05f;

    [Range(0,1)]
    public float musicVolume = .5f;
    
    [Range(0,1)]
    public float fxVolume = 1f;

    private void Start()
    {
        PlayRandomMusic();
    }
    public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            GameObject go = new GameObject("SoundFX "+ clip.name);
            go.transform.position = position;

            AudioSource source = go.AddComponent<AudioSource>();
            source.clip = clip;

            float randomPitch = Random.Range(lowPitch,highPitch);
            source.pitch = randomPitch;
            source.volume = volume;

            source.Play();
            Destroy(go,clip.length);
            return source;
        }
        return null;
    }
    public AudioSource PlayRandom(AudioClip[] clips, Vector3 position, float volume = 1f)
    {
        if (clips != null)
        {
            if (clips.Length != 0)
            {
                int randomIndex = Random.Range(0,clips.Length);

                if (clips[randomIndex] != null)
                {
                    AudioSource source = PlayClipAtPoint(clips[randomIndex],position,volume);
                    return source;
                }
            }
        }
        return null;
    }

    public void PlayRandomMusic()
    {
        PlayRandom(musicClips,Vector3.zero,musicVolume);
    }
    public void PlayWinSound()
    {
        PlayRandom(winClips,Vector3.zero,fxVolume);
    }
    public void PlayLoseSound()
    {
        PlayRandom(loseClips,Vector3.zero,fxVolume * .5f);
    }
    public void PlayBonusSound()
    {
        PlayRandom(bonusClips,Vector3.zero,fxVolume);
    }
}
