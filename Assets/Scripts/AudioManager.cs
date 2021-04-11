using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource AudioSourcePrefab;

    public AudioSource workingSource;

    public List<AudioSource> unusedAudioSources;
    public List<AudioSource> activeAudioSources;


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        DontDestroyOnLoad(this);

        unusedAudioSources = new List<AudioSource>();
        activeAudioSources = new List<AudioSource>();
    }

    public void PlayClipAt(AudioClip clip, Vector3 pos)
    {
        // If the source is no longer playing, remove from active, add back to unused
        for (int i = 0; i < activeAudioSources.Count; i++)
        {
            if (activeAudioSources[i] && !activeAudioSources[i].isPlaying)
            {
                unusedAudioSources.Add(activeAudioSources[i]);
                activeAudioSources.Remove(activeAudioSources[i]);
                i--;
            }
        }

        workingSource = null;
        
        // If there are no available sources to use, instantiate one.
        if (unusedAudioSources.Count < 1)
        {
            workingSource = Instantiate(AudioSourcePrefab, pos, Quaternion.identity);
            Debug.Log("Created: " + workingSource);
        }
        else // If there is an unused source, use it
        {
            workingSource = unusedAudioSources[0];
        }
        Debug.Log("Using: " + workingSource);

        // Add this source to active
        activeAudioSources.Add(workingSource);
        // Set source clip and play it
        workingSource.clip = clip;
        workingSource.Play();
    }

}
