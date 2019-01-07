﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    Main main;
    int numberOfLevels;
    Font font;
    GameObject menuButtonPrefab;
    GameObject levelText;
    int selectedLevelNumber;

    void Awake()
    {
        // Initially not active
        gameObject.SetActive(false);

        // Load menuButton prefab
        menuButtonPrefab = Resources.Load<GameObject>("UI/MenuButton");

        // Add components
        gameObject.AddComponent<Canvas>();
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        // Setup canvas
        var canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.planeDistance = 20;

        // Setup font
        font = Resources.Load<Font>("Font/FiraMono-Bold");

        // Title text
        var titleText = AddTextMesh(gameObject, "crawly", TextAnchor.MiddleCenter, new Color32(0, 255, 0, 100), 10);
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(0, 20, 0));

        // Level text
        levelText = AddTextMesh(gameObject, "Level 1", TextAnchor.MiddleCenter, new Color32(12, 46, 18, 100), 5);
        levelText.name = "Level";

        // Previous Level Button
        var prevLevelButton = AddButton('\u2B05'.ToString(), 160);
        prevLevelButton.name = "Previous Level Button";
        prevLevelButton.transform.Translate(new Vector3(-35, 0, 0));
        prevLevelButton.GetComponent<Button>().onClick.AddListener(SelectPreviousLevel);

        // Next Level Button
        var nextLevelButton = AddButton('\u27A1'.ToString(), 160);
        nextLevelButton.name = "Next Level Button";
        nextLevelButton.transform.Translate(new Vector3(35, 0, 0));
        nextLevelButton.GetComponent<Button>().onClick.AddListener(SelectNextLevel);

        // Play Button
        var playButton = AddButton("PLAY", 320);
        playButton.name = "Play Button";
        playButton.transform.Translate(new Vector3(-20, -18, 0));
        playButton.GetComponent<Button>().onClick.AddListener(PlayButtonOnClick);

        // Help Button
        var helpButton = AddButton("HELP", 320);
        helpButton.name = "Help Button";
        helpButton.transform.Translate(new Vector3(20, -18, 0));
        helpButton.GetComponent<Button>().onClick.AddListener(HelpButtonOnClick);

    }

    public void Init(Main main, int numberOfLevels) 
    {
        this.main = main;
        this.numberOfLevels = numberOfLevels;
    }

    public void Show(int selectedLevelNumber) {
        InitCamera();
        UpdateSelectedLevel(selectedLevelNumber);
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) 
        {
            PlayButtonOnClick();
        } 
        if (Input.GetKeyDown(KeyCode.H)) 
        {
            HelpButtonOnClick();
        } 
        else if (Input.GetKeyDown(KeyCode.RightArrow)) 
        {
            SelectNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            SelectPreviousLevel();
        }
    }

    void InitCamera() {
        Camera.main.orthographic = true;
        Camera.main.transform.position = new Vector3(55.6f, 45.5f, -56.5f);
        Camera.main.transform.rotation = Quaternion.Euler(30, -45, 0);
    }

    void SelectNextLevel() {
        main.HandleButtonPressedFX();
        var nextLevelNumber = selectedLevelNumber + 1;
        if (nextLevelNumber > numberOfLevels) {
            nextLevelNumber = 1;
        }
        UpdateSelectedLevel(nextLevelNumber);
    }

    void SelectPreviousLevel() {
        main.HandleButtonPressedFX();
        var prevLevelNumber = selectedLevelNumber - 1;
        if (prevLevelNumber < 1) {
            prevLevelNumber = numberOfLevels;
        }
        UpdateSelectedLevel(prevLevelNumber);
    }

    void UpdateSelectedLevel(int selectedLevelNumber) {
        this.selectedLevelNumber = selectedLevelNumber;
        levelText.GetComponent<TextMesh>().text = "Level " + selectedLevelNumber;
    }

    void PlayButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandlePlayButtonPressed(selectedLevelNumber);
    }

    void HelpButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandleHelpButtonPressed();
    }

    public GameObject AddTextMesh(GameObject parent, string textContent, TextAnchor alignment, Color32 color, float scale) {
        // Note: TextMesh scales better than regular Text
        var txtMesh = new GameObject();
        txtMesh.transform.SetParent(parent.transform, false);
        txtMesh.transform.localScale = new Vector3(scale, scale, scale);
        var tm = txtMesh.AddComponent<TextMesh>();
        tm.text = textContent;
        tm.font = font;
        tm.fontSize = 200;
        tm.fontStyle = FontStyle.Bold;
        tm.color = color;
        tm.offsetZ = -1;
        tm.anchor = alignment;

        return txtMesh;
    }

    public GameObject AddButton(string textContent, int width) {
        // Create button & set parent
        var button = Instantiate(menuButtonPrefab);
        button.transform.SetParent(gameObject.transform, false);

        // Setup button width & height
        var playButtonRectTransform = button.GetComponent<RectTransform>();
        playButtonRectTransform.sizeDelta = new Vector2 (width, 120);

        // Setup button colors
        var btnColorBlock = ColorBlock.defaultColorBlock;
        btnColorBlock.normalColor = ConvertColor(49, 77, 121);
        btnColorBlock.highlightedColor = ConvertColor(0, 150, 0);
        btnColorBlock.pressedColor = ConvertColor(0, 100, 0);
        button.GetComponent<Button>().colors = btnColorBlock;

        // Hide existing Text
        var txt = button.GetComponentInChildren<Text>();
        txt.text = "";

        // Add TextMesh
        var txtMesh = AddTextMesh(button, textContent, TextAnchor.MiddleCenter, new Color32(255, 255, 255, 255), 5);
        txtMesh.name = "TextMesh";

        return button;
    }

    Color ConvertColor (int r, int g, int b) {
        return new Color(r/255f, g/255f, b/255f);
    }

}