using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource AudioSourcePrefab;

    public AudioSource audioSource;


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        AddButtonSFX(); // Add sfx to all Buttons if not already

        if (!audioSource)
            audioSource = Instantiate(AudioSourcePrefab, Vector3.zero, Quaternion.identity);
    }

    public void AddButtonSFX()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button b in buttons)
        {
            if (!b.gameObject.GetComponent<ButtonsSFX>())
                b.gameObject.AddComponent<ButtonsSFX>();
        }
    }

}
