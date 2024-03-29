﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class Builder : MonoBehaviour
{
    // button objects
    private Button BuildButton;
    private TextMeshProUGUI buttonText;

    private BuilderController builder;
    public BuildableObject block;

    private void Start()
    {

        if (!Application.isPlaying)
            return;

        BuildButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        builder = transform.root.GetComponent<BuilderController>();

        buttonText.text = gameObject.name;
        BuildButton.onClick.AddListener(Build);
    }

    private void Update()
    {

        if (!Application.isPlaying) {
            if (!buttonText)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = gameObject.name;
            return;
        }

        // if building and selected block and it also is ready
        if (builder.isBuilding && builder.workingBlock && builder.workingBlock.buildReady)
        {
            // snap block to grid //
            builder.workingBlock.pos = builder.grid.GetNearestPointOnGrid(Global.getScreenToWorldMouse());

            // left-click build in valid space and not on UI //
            if (Input.GetMouseButton(0) &&
                !builder.workingBlock.isEncroaching &&
                !EventSystem.current.IsPointerOverGameObject() &&
                Vector3.Distance(builder.workingBlock.transform.position, builder.grid.GetNearestPointOnGrid(Global.getScreenToWorldMouse())) < 0.5f
                    )
            {
                builder.workingBlock.place(GamePhaseManager.instance.levels[GamePhaseManager.instance.currentLevel]);
                InstantiateObject(builder.workingBuilder.block); // instantiate new block from builder
                builder.workingBlock.transform.position = Vector3.down * 100; // starts the placed block lower so it flies to clicked location
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
        // is demolisher is working then turn off demolisher
        if (builder.demolisher.isDemolishing)
        {
            builder.demolisher.demoModeOff();
        }
        // if not already building
        if (!builder.isBuilding)
        {
            BuildingModeOn(this);
            // create a new block at mouse pos //
            InstantiateObject(builder.workingBuilder.block);
        }
        else
        {
            BuildingModeOff();
            BuildingModeOn(this);
            InstantiateObject(builder.workingBuilder.block);
        }
    }

    private void InstantiateObject(BuildableObject b)
    {
        builder.workingBlock = Instantiate(b, Vector3.zero, Quaternion.identity); // instantiate new block from builder
    }

    public void BuildingModeOn(Builder b)
    {
        if (builder.workingBlock) // delete instantiated block
        {
            builder.workingBlock.delete();
        }
        builder.workingBlock = null; // no working block

        builder.workingBuilder = b; // set working builder to this
        builder.isBuilding = true; // is building
    }

    public void BuildingModeOff()
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
