using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuilderController : MonoBehaviour
{
    public bool isBuilding;
    //public Builder workingBuilder;
    public BlockBase workingBlock;
    public BlockGrid grid;

    private void Start()
    {
        isBuilding = false;
        workingBlock = null;
        grid = FindObjectOfType<BlockGrid>();
    }

    public void DeactivateBuilderCanvas()
    {
        if (workingBlock)
            workingBlock = null;
        GetComponentInParent<Canvas>().gameObject.SetActive(false);
    }
}
