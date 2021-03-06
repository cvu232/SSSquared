using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Demolisher : MonoBehaviour
{
    private Button Button;
    private TextMeshProUGUI buttonText;
    private string defaultTxt = "Destroy Mode";
    private string cancelTxt = "Cancel";

    public Material highlightBoom;

    public BuilderController builder;

    public bool isDemolishing;
    private BlockBase workingBlock = null;
    public GameObject clickedObj = null;

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
                    if (clickedObj.GetComponent<BlockBase>() && !workingBlock)
                    {
                        // set workingBlock
                        workingBlock = clickedObj.GetComponent<BlockBase>();
                        // set selection highlight
                        workingBlock.meshRenderer.material = highlightBoom;
                    }

                    // if clicking on an object and the object is the workingBlock then destroy it (Double-click to confirm boom basically)
                    else if (clickedObj && workingBlock && clickedObj == workingBlock.gameObject)
                        Destroy(workingBlock.gameObject);
                    // if the clicked obj != working block
                    else if (clickedObj && workingBlock && clickedObj != workingBlock.gameObject)
                    {
                        // clear workingblock select
                        workingBlock.meshRenderer.material = workingBlock.material;
                        // set workingBlock
                        workingBlock = clickedObj.GetComponent<BlockBase>();
                        // set selection highlight
                        workingBlock.meshRenderer.material = highlightBoom;
                    }
                    else
                    {
                        // unselect
                        clickedObj = null;
                        // set back og material if changed
                        if (workingBlock)
                            workingBlock.meshRenderer.material = workingBlock.material;
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
            workingBlock.meshRenderer.material = workingBlock.material;
        workingBlock = null;


        Button.onClick.RemoveAllListeners(); // remove listeners 
        buttonText.text = defaultTxt;
        Button.onClick.AddListener(demoModeOn);
    }
}
