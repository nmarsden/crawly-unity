﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HUD : MonoBehaviour
{
    Main main;
    Font font;
    GameObject menuButtonPrefab;
    GameObject levelNumText;    
    GameObject fillText;
    GameObject gameCompletedText;
    GameObject gameOverText;
    GameObject pausedText;
    GameObject okButton;
    int currentLevelNum;

    StatusBar shieldStatusBar;
    Color32 shieldStatusColor = new Color32(0, 35, 102, 200);

    public void Init(Main main) 
    {
        this.main = main;
        currentLevelNum = main.GetCurrentLevelNum();
    }

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
        canvas.planeDistance = 5;

        // Setup font
        font = Resources.Load<Font>("Font/FiraMono-Bold");

        // Title text
        var titleText = AddTextMesh(gameObject, "crawly", TextAnchor.MiddleCenter, new Color32(0, 255, 0, 100), 3);
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(-35, -25, 0));

        // Level Number text
        levelNumText = AddTextMesh(gameObject, "Level " + currentLevelNum, TextAnchor.MiddleCenter, new Color32(255, 255, 255, 100), 3);
        levelNumText.name = "Level Number";
        levelNumText.transform.Translate(new Vector3(31, 25, 0));

        // Fill text
        fillText = AddTextMesh(gameObject, "Fill", TextAnchor.MiddleCenter, new Color32(0, 255, 0, 100), 3);
        fillText.name = "Fill";
        fillText.transform.Translate(new Vector3(25, -25, 0));

        // Game Completed text
        gameCompletedText = AddTextMesh(gameObject, "COMPLETE", TextAnchor.MiddleCenter, new Color32(0, 0, 100, 200), 5);
        gameCompletedText.name = "Game Completed";
        gameCompletedText.transform.Translate(new Vector3(0, 10, 0));
        gameCompletedText.SetActive(false);

        // Game Over text
        gameOverText = AddTextMesh(gameObject, "GAME OVER", TextAnchor.MiddleCenter, new Color32(0, 0, 100, 200), 5);
        gameOverText.name = "Game Over";
        gameOverText.transform.Translate(new Vector3(0, 10, 0));
        gameOverText.SetActive(false);

        // Paused text
        pausedText = AddTextMesh(gameObject, "PAUSED", TextAnchor.MiddleCenter, new Color32(0, 0, 100, 200), 5);
        pausedText.name = "Paused";
        pausedText.transform.Translate(new Vector3(0, 10, 0));
        pausedText.SetActive(false);

        // OK Button
        okButton = AddButton("OK", 200);
        okButton.name = "OK Button";
        okButton.transform.Translate(new Vector3(0, -10, 0));
        okButton.GetComponent<Button>().onClick.AddListener(OkButtonOnClick);
        okButton.SetActive(false);

        // Shield Status Bar
        shieldStatusBar = CreateShieldStatusBar(shieldStatusColor);
    }

    public void Show(int selectedLevelNumber) {
        UpdateSelectedLevel(selectedLevelNumber);
        UpdateShieldStatusValue(0);

        gameOverText.SetActive(false);
        gameCompletedText.SetActive(false);
        pausedText.SetActive(false);
        okButton.SetActive(false);

        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    void UpdateSelectedLevel(int selectedLevelNumber) {
        this.currentLevelNum = selectedLevelNumber;
        levelNumText.GetComponent<TextMesh>().text = "Level " + currentLevelNum;
    }

    void OkButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandleHudOkButtonClicked();
    }

    void Update()
    {   
        fillText.GetComponent<TextMesh>().text = "filled " + main.GetFilledPercentage() + "%";
    }

    public void ShowGameOverMessage() {
        gameOverText.SetActive(true);
        okButton.SetActive(true);
    }

    public void ShowGameCompletedMessage() {
        gameCompletedText.SetActive(true);
        okButton.SetActive(true);
    }

    public void ShowPausedMessage() {
        pausedText.SetActive(true);
        okButton.SetActive(true);
    }

    public void HidePausedMessage() {
        pausedText.SetActive(false);
        okButton.SetActive(false);
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

    public void UpdateShieldStatusValue(float value) {
        shieldStatusBar.UpdateValue(value);
    }

    StatusBar CreateShieldStatusBar(Color32 color) {
        // Add Shield Icon
        var shieldIcon = new GameObject();        
        shieldIcon.name = "Shield Icon";
        shieldIcon.transform.SetParent(gameObject.transform, false);
        shieldIcon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        shieldIcon.transform.Translate(new Vector3(-43, 25, 0));
        shieldIcon.AddComponent<SpriteRenderer>();
        shieldIcon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/Shield");
        shieldIcon.GetComponent<SpriteRenderer>().color = ConvertColor(color.r, color.g, color.b, color.a);

        // Add Shield Status Bar
        var shieldStatusBar = new GameObject();
        shieldStatusBar.name = "Shield Status Bar";
        shieldStatusBar.transform.SetParent(gameObject.transform, false);
        shieldStatusBar.transform.localScale = new Vector3(2, 2, 2);
        shieldStatusBar.transform.Translate(new Vector3(-30, 25, 0));

        var statusBar = shieldStatusBar.AddComponent<StatusBar>();
        statusBar.Init(color);
        statusBar.UpdateValue(0);
        return statusBar;
    }

    Color ConvertColor (int r, int g, int b) {
        return new Color(r/255f, g/255f, b/255f);
    }
    Color ConvertColor (int r, int g, int b, int a) {
        return new Color(r/255f, g/255f, b/255f, a/255f);
    }
}
