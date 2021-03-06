using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    // button objects
    private Button BuildButton;
    private TextMeshProUGUI buttonText;

    private BuilderController builder;
    public BlockBase block;

    private void Start()
    {
        builder = transform.root.GetComponent<BuilderController>();

        BuildButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = gameObject.name;
        BuildButton.onClick.AddListener(Build);
    }

    private void Update()
    {
        // if building and selected block and it also is ready
        if (builder.isBuilding && builder.workingBlock && builder.workingBlock.buildReady)
        {
            // snap block to grid //
            builder.workingBlock.transform.position = builder.grid.GetNearestPointOnGrid(Global.getScreenToWorldMouse());

            // left-click build in valid space and not on UI //
            if (Input.GetMouseButtonDown(0) && !builder.workingBlock.inBadSpace && !EventSystem.current.IsPointerOverGameObject())
            {
                builder.workingBlock.transform.position = builder.grid.GetNearestPointOnGrid(Global.getScreenToWorldMouse());
                builder.workingBlock.place();
                //builder.workingBlock = null; // insurance
                builder.workingBlock = Instantiate(builder.workingBuilder.block, Vector3.zero, Quaternion.identity); // instantiate new block from builder
            }
            // right-click cancel
            else if (Input.GetMouseButtonDown(1))
            {
                BuildingModeOff();
            }
        }
    }

    private void Build()
    {
        // if not already building
        if (!builder.isBuilding)
        {
            BuildingModeOn(this);
            // create a new block at mouse pos //
            InstantiateBlock(builder.workingBuilder.block);
        }
        else
        {
            BuildingModeOff();
            BuildingModeOn(this);
            InstantiateBlock(builder.workingBuilder.block);
        }
    }

    private void InstantiateBlock(BlockBase b)
    {
        builder.workingBlock = Instantiate(b, Vector3.zero, Quaternion.identity); // instantiate new block from builder
        builder.workingBlock.buildReady = true; // block is build ready
    }

    private void BuildingModeOn(Builder b)
    {
        if (builder.workingBlock) // delete instantiated block
        {
            builder.workingBlock.delete();
        }
        builder.workingBlock = null; // no working block

        builder.workingBuilder = b; // set working builder to this
        builder.isBuilding = true; // is building
    }

    private void BuildingModeOff()
    {
        if (builder.workingBlock) // delete instantiated block
        {
            builder.workingBlock.delete();
        }
        builder.workingBlock = null; // no working block

        builder.workingBuilder = null; // no working builder
        builder.isBuilding = false; // is not building
    }
}
