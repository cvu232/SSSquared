using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Demolisher : MonoBehaviour
{
    private Button Button;
    private TextMeshProUGUI buttonText;
    private string defaultTxt = "Eraser";
    private string cancelTxt = "Done";

    public BuilderController builder;

    public bool isDemolishing;
    private BuildableObject workingBlock = null;
    public GameObject clickedObj = null;

    public AudioClip destroyBlockSFX; // Set in Inspector

    private void Start()
    {
        isDemolishing = false;

        Button = GetComponent<Button>();
        Button.onClick.AddListener(demoModeOn);
        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        buttonText.text = defaultTxt;

        builder = transform.root.GetComponent<BuilderController>();
    }

    private void Update()
    {
        if (isDemolishing)
        {
            //Debug.Log("Demo on");
            // await left click for block selection
            if (Input.GetMouseButtonDown(0))
            {
                // get object on click
                clickedObj = Global.getMouseClickObject();

                // if the clicked obj exists
                if (clickedObj)
                {
                    // if clicked object is a block and no block selected yet
                    if (clickedObj.GetComponent<BuildableObject>() && !workingBlock)
                    {
                        // set workingBlock
                        workingBlock = clickedObj.GetComponent<BuildableObject>();
                        // set selection highlight
                        workingBlock.HighlightTarget();
                    }

                    // if clicking on an object and the object is the workingBlock then destroy it (Double-click to confirm boom basically)
                    else if (clickedObj && workingBlock && clickedObj == workingBlock.gameObject)
                    {
                        AudioManager.instance.PlayClipAt(destroyBlockSFX, transform.position); // Play destroy clip
                        workingBlock.delete();
                    }
                    // if the clicked obj != working block
                    else if (clickedObj && workingBlock && clickedObj != workingBlock.gameObject)
                    {
                        // clear workingblock select
                        workingBlock.HighlightOff();
                        // set workingBlock
                        workingBlock = clickedObj.GetComponent<BuildableObject>();
                        // set selection highlight
                        workingBlock.HighlightTarget();
                    }
                    else
                    {
                        // unselect
                        clickedObj = null;
                        // set back og material if changed
                        if (workingBlock)
                            workingBlock.HighlightOff();
                        workingBlock = null;
                    }
                }
            }
            // right click deselect
            else if (Input.GetMouseButtonDown(1))
            {
                demoModeOff();
            }
        }
    }

    public void demoModeOn()
    {
        if (builder.isBuilding) // if user is building and cancels mid-building, turn off BuildingMode
        {
            builder.workingBuilder.BuildingModeOff();
        }

        Button.onClick.RemoveAllListeners(); // remove listeners
        isDemolishing = true;
        buttonText.text = cancelTxt; // Change Button text and Listener to indicate next press is Cancel
        Button.onClick.AddListener(demoModeOff);
    }

    public void demoModeOff()
    {
        isDemolishing = false;
        // deselect all
        clickedObj = null;
        // set back og material if changed
        if (workingBlock)
            workingBlock.HighlightOff();
        workingBlock = null;


        Button.onClick.RemoveAllListeners(); // remove listeners 
        buttonText.text = defaultTxt;
        Button.onClick.AddListener(demoModeOn);
    }
}
