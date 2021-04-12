using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance {
        get {
            if (!_instance) {
                try {
                    _instance = FindObjectOfType<AudioManager>();
                } catch {
                    _instance = Instantiate((GameObject)Resources.Load("Managers/AudioManager")).GetComponent<AudioManager>();
                }
            }
            return (_instance);
        }
    }
    private static AudioManager _instance;

    public AudioSource AudioSourcePrefab;

    public AudioSource workingSource;

    public List<AudioSource> unusedAudioSources;
    public List<AudioSource> activeAudioSources;


    private void Awake()
    {
        /*
        if (instance != null && instance != this)
            Destroy(this);
        else
            _instance = this
        */

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
                Debug.Log("Reusing: " + activeAudioSources[i].name);
                unusedAudioSources.Add(activeAudioSources[i]);
                activeAudioSources.Remove(activeAudioSources[i]);
                i--;
            }
        }
        
        // If there are unused sources and it's not playing, use it
        if (unusedAudioSources.Count > 0)
        {
            workingSource = unusedAudioSources[0]; // Set as working Source
            unusedAudioSources.Remove(workingSource); // Remove it from unused
            Debug.Log("Using: " + workingSource.name + "of " + unusedAudioSources.Count);
        }
        else // Instantiate a new source if nothing available
        {
            workingSource = Instantiate(AudioSourcePrefab, pos, Quaternion.identity);
            Debug.Log("Created: " + workingSource.name);
        }

        // Add this source to active
        activeAudioSources.Add(workingSource);
        // Set source clip and play it
        workingSource.clip = clip;
        workingSource.Play();
    }

}
