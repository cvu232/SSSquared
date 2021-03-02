using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    private Button BuildButton;
    private TextMeshProUGUI buttonText;
    public BlockBase block;
    private BuilderController builder;

    private void Start()
    {
        BuildButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = gameObject.name;
        builder = gameObject.GetComponentInParent<BuilderController>();
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
                BuildingModeOff();
            }
            // right-click cancel
            else if (Input.GetMouseButtonDown(1))
            {
                builder.workingBlock.delete();
                BuildingModeOff();
            }
        }
        else if (builder.isBuilding && block != builder.workingBlock)// if is still building but workingBlock is not this builder's block
        {
            buttonText.text = gameObject.name;
        }
    }

    private void Build()
    {
        // if not already building
        if (!builder.isBuilding)
        {
            BuildingModeOn();
            // create a new block at mouse pos //
            InstantiateBlock();
        }
        else if (builder.isBuilding)
        {
            BuildingModeOff();
            BuildingModeOn();
            InstantiateBlock();
        }
    }

    private void InstantiateBlock()
    {
        builder.workingBlock = Instantiate(block, Vector3.zero, Quaternion.identity);
        builder.workingBlock.buildReady = true; // set block to holo
    }

    private void BuildingModeOn()
    {
        builder.isBuilding = true;
        if (builder.workingBlock)
        {
            Destroy(builder.workingBlock);
        }
        builder.workingBlock = null;
    }

    private void BuildingModeOff()
    {
        if (builder.workingBlock) // detach if not cancelled //
            builder.workingBlock = null;
        builder.isBuilding = false;
    }

}
