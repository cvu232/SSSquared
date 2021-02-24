using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderController : MonoBehaviour
{
    public bool isBuilding = false;
    public BlockBase workingBlock = null;
    public BlockGrid grid = null;

    private void Start()
    {
        isBuilding = false;
        workingBlock = null;
        grid = FindObjectOfType<BlockGrid>();
    }
}
