using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderController : MonoBehaviour
{
    public bool isBuilding;
    public Builder workingBuilder;
    public BlockBase workingBlock;
    public BlockGrid grid;

    private void Start()
    {
        isBuilding = false;
        workingBuilder = null;
        workingBlock = null;
        grid = FindObjectOfType<BlockGrid>();
    }

    public void DeactivateBuilderCanvas()
    {
        if (workingBlock)
        {
            workingBlock.delete(); // delete block if instantiated
        }
        workingBlock = null;
        workingBuilder = null;

        gameObject.SetActive(false); // disable itself
    }
}
