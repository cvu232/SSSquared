using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AddButtonSFX : MonoBehaviour
{
    /*
     * Place in a GameObject to run.
     * Adds ButtonsSFX script to all buttons in scene.
     * This lets button play set sfx on hover/clicked.
     * 
     */

    private void Awake()
    {
        runInEditMode = true;
    }

    private void Start()
    {
        Run();
    }

    public void Run()
    {
        // Get all Buttons in scene
        UnityEngine.UI.Button[] buttons = FindObjectsOfType<UnityEngine.UI.Button>();

        // Add sfx to all Buttons in scene if not already
        foreach (UnityEngine.UI.Button b in buttons)
        {
            if (!b.gameObject.GetComponent<ButtonsSFX>())
            {
                b.gameObject.AddComponent<ButtonsSFX>();
                Debug.Log("Added AddButtonSFX to " + b.name);
            }
        }

        Debug.Log("End AddButtonSFX");

        DestroyImmediate(this); // Delete script on end
    }
}
