using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderController : MonoBehaviour
{
    public bool isBuilding;
    public Builder workingBuilder;
    public Demolisher demolisher;
    public BlockBase workingBlock;
    public BlockGrid grid;

    private void Start()
    {
        isBuilding = false;
        workingBuilder = null;
        demolisher = FindObjectOfType<Demolisher>();
        workingBlock = null;
        grid = FindObjectOfType<BlockGrid>();
    }

    public void DeactivateBuilderCanvas()
    {
        if (workingBuilder)
            workingBuilder.BuildingModeOff();
        demolisher.demoModeOff();

        transform.GetChild(0).gameObject.SetActive(false); // disable itself
    }
}
