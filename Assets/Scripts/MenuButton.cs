using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum MenuButtonFunctions {
    quitToMenu,
    restart
}

public class MenuButton : EventTrigger {

    public MenuButtonFunctions function;
    public bool unpauseOnClick = true;

    public override void OnPointerClick(PointerEventData eventData) {
        if (unpauseOnClick)
            
        switch (function) {

            case MenuButtonFunctions.quitToMenu:

                SceneManager.LoadScene(0);

                break;

            case MenuButtonFunctions.restart:

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                break;

        }
    }

}