using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadyCheck : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button.GetComponent<Button>();
        button.onClick.AddListener(ready);
    }

    public void ready()
    {
        Loop.readyCheck = true;
        gameObject.SetActive(false);
    }
}
