﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Menus {
    main,
    options,
    rules,
    charSelect,
    credits
}

public class MainMenu : MonoBehaviour {

    public Menus currentMenu;

    public UIPanel mainPanel;
    public UIPanel optionsPanel;
    public UIPanel rulesPanel;
    public UIPanel charSelectPanel;
    public UIPanel creditsPanel;

    private void Start() {

    }

    public void GameOptions() {
        currentMenu = Menus.options;
    }

    public void Rules() {
        currentMenu = Menus.rules;
    }

    public void CharSelect() {
        currentMenu = Menus.charSelect;
    }

    public void Credits() {
        currentMenu = Menus.credits;
    }

    public void ReturnToMenu() {
        currentMenu = Menus.main;
    }

    private void Update() {

        if (mainPanel)
            mainPanel.show = currentMenu == Menus.main;
        if (optionsPanel)
            optionsPanel.show = currentMenu == Menus.options;
        if (rulesPanel)
            rulesPanel.show = currentMenu == Menus.rules;
        if (charSelectPanel)
            charSelectPanel.show = currentMenu == Menus.charSelect;
        if (creditsPanel)
            creditsPanel.show = currentMenu == Menus.credits;

        SwayingSpacemen.showCredits = currentMenu == Menus.credits;

    }

    public void PlayGame() {
        SceneManager.LoadScene("GameScene");
    }

    public void Menu() {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        Application.Quit();

    }

}