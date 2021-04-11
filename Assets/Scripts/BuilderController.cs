using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderController : MonoBehaviour
{

    public static BuilderController instance;

    public bool isBuilding;
    public Builder workingBuilder;
    public Demolisher demolisher;
    public BuildableObject workingBlock;
    public BlockGrid grid;

    private void Awake()
    {
        instance = this;
        isBuilding = false;
        workingBuilder = null;
        demolisher = FindObjectOfType<Demolisher>();
        workingBlock = null;
        grid = FindObjectOfType<BlockGrid>();
    }

    public void Activate()
    {
        transform.GetChild(0).gameObject.SetActive(true); // disable itself
    }

    public void Deactivate()
    {
        if (workingBuilder)
            workingBuilder.BuildingModeOff();
        demolisher.demoModeOff();

        transform.GetChild(0).gameObject.SetActive(false); // disable itself
    }

    public void Initialize()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
