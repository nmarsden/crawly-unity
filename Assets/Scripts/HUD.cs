using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HUD : MonoBehaviour
{
    Main main;
    GameObject menuButtonPrefab;
    GameObject levelNumText;    
    GameObject overlay;
    GameObject demoText;
    GameObject gameCompletedText;
    GameObject gameOverText;
    GameObject pausedText;
    GameObject okButton;
    int currentLevelNum;

    StatusBar shieldStatusBar;
    StatusBar filledStatusBar;
    Color32 shieldStatusColor = new Color32(0, 35, 102, 200);
    Color32 filledStatusColor = new Color32(255, 255, 0, 200);

    public void Init(Main main) 
    {
        this.main = main;
        currentLevelNum = main.GetCurrentLevelNum();

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

        var levelTextColor = new Color32(255, 255, 255, 250);
        var levelOutlineColor = new Color32(0, 0, 0, 200);

        var textColor = new Color32(250, 189, 135, 255);
        var outlineColor = new Color32(247, 255, 0, 255);

        var textSize = new Vector2 (160, 50);
        var mediumTextSize = new Vector2 (236, 80);
        var largeTextSize = new Vector2 (360, 90);

        // Title text
        var titleText = AddTextMeshPro(gameObject, "CRAWLY", TextAnchor.MiddleCenter, textColor, mediumTextSize, outlineColor);
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(0, 25.5f, 0));

        // Level Number text
        levelNumText = AddTextMeshPro(gameObject, "Level " + currentLevelNum, TextAnchor.MiddleCenter, levelTextColor, textSize, levelOutlineColor);
        levelNumText.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(levelTextColor);
        levelNumText.name = "Level Number";
        levelNumText.transform.Translate(new Vector3(37, 25, 0));

        // Overlay
        overlay = AddOverlay();

        // Demo text
        demoText = AddTextMeshPro(gameObject, "DEMO", TextAnchor.MiddleCenter, levelTextColor, textSize, levelOutlineColor);
        demoText.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(levelTextColor);
        demoText.name = "Demo";
        demoText.transform.Translate(new Vector3(-37.5f, -25, 0));

        // Game Completed text
        gameCompletedText = AddTextMeshPro(gameObject, "COMPLETE", TextAnchor.MiddleCenter, textColor, largeTextSize, outlineColor);
        gameCompletedText.name = "Game Completed";
        gameCompletedText.transform.Translate(new Vector3(0, 6, 0));

        // Game Over text
        gameOverText = AddTextMeshPro(gameObject, "GAME OVER", TextAnchor.MiddleCenter, textColor, largeTextSize, outlineColor);
        gameOverText.name = "Game Over";
        gameOverText.transform.Translate(new Vector3(0, 6, 0));

        // Paused text
        pausedText = AddTextMeshPro(gameObject, "PAUSED", TextAnchor.MiddleCenter, textColor, largeTextSize, outlineColor);
        pausedText.name = "Paused";
        pausedText.transform.Translate(new Vector3(0, 6, 0));

        // OK Button
        okButton = AddButton(canvas, "OK", 200);
        okButton.name = "OK Button";
        okButton.transform.Translate(new Vector3(0, -6, 0));
        okButton.GetComponent<Button>().onClick.AddListener(OkButtonOnClick);

        // Shield Status Bar
        shieldStatusBar = CreateShieldStatusBar(shieldStatusColor);

        // Filled Status Bar
        filledStatusBar = CreateFilledStatusBar(filledStatusColor);

        gameObject.SetActive(false);
    }

    public void Show(int selectedLevelNumber, Main.PlayMode playMode) {
        UpdateSelectedLevel(selectedLevelNumber);
        UpdateShieldStatusValue(0);
        UpdateFilledStatusBarWidth();                   

        demoText.SetActive(playMode.Equals(Main.PlayMode.DEMO));

        overlay.SetActive(false);
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
        levelNumText.GetComponent<TextMeshProUGUI>().text = "Level " + currentLevelNum;
    }

    void OkButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandleHudOkButtonClicked();
    }

    void Update()
    {   
        filledStatusBar.UpdateValue(main.GetFilledPercentage());
    }

    public void ShowGameOverMessage() {
        overlay.SetActive(true);
        gameOverText.SetActive(true);
        okButton.SetActive(true);
    }

    public void ShowGameCompletedMessage() {
        overlay.SetActive(true);
        gameCompletedText.SetActive(true);
        okButton.SetActive(true);
    }

    public void ShowPausedMessage() {
        overlay.SetActive(true);
        pausedText.SetActive(true);
        okButton.SetActive(true);
    }

    public void HidePausedMessage() {
        overlay.SetActive(false);
        pausedText.SetActive(false);
        okButton.SetActive(false);
    }

    public GameObject AddTextMeshPro(GameObject parent, string textContent, TextAnchor alignment, Color32 color, Vector2 size, Color32 outlineColor) {
        var txtMesh = new GameObject();
        txtMesh.transform.SetParent(parent.transform, false);
        txtMesh.layer = 5; // UI

        var tm = txtMesh.AddComponent<TextMeshProUGUI>();
        tm.text = textContent;

        var rectTransform = tm.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;

        tm.enableAutoSizing = true;
        tm.fontSizeMax = 700;
        tm.alignment = TextAlignmentOptions.Center;
        tm.faceColor = color;

        tm.enableVertexGradient = true;
        var topColor = new Color32(255, 133, 0, 255);
        var bottomColor = new Color32(255, 211, 120, 255);
        tm.colorGradient = new VertexGradient(topColor, topColor, bottomColor, bottomColor);

        tm.outlineColor = outlineColor;

        return txtMesh;
    }

    public GameObject AddButton(Canvas canvas, string textContent, int width) {
        // Create button & set parent
        var button = Instantiate(menuButtonPrefab);
        button.transform.SetParent(canvas.transform, false);
        button.transform.localScale = new Vector3(1, 1, 1);

        // Setup button width & height
        var playButtonRectTransform = button.GetComponent<RectTransform>();
        playButtonRectTransform.sizeDelta = new Vector2 (width, 60);

        // Setup button colors
        var btnColorBlock = ColorBlock.defaultColorBlock;
        btnColorBlock.normalColor = ConvertColor(0, 100, 0);
        btnColorBlock.highlightedColor = ConvertColor(0, 150, 0);
        btnColorBlock.pressedColor = ConvertColor(0, 100, 0);
        button.GetComponent<Button>().colors = btnColorBlock;

        // Hide existing Text
        var txt = button.GetComponentInChildren<Text>();
        txt.text = "";

        // Add TextMesh
        var textColor = new Color32(255, 255, 255, 250);
        var outlineColor = new Color32(0, 0, 0, 200);
        var txtMesh = AddTextMeshPro(button, textContent, TextAnchor.MiddleCenter, textColor, new Vector2 (70, 50), outlineColor);
        txtMesh.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(textColor);
        txtMesh.name = "TextMeshPro";

        return button;
    }

    public void UpdateShieldStatusValue(float value) {
        shieldStatusBar.UpdateValue(value);
    }

    private void UpdateFilledStatusBarWidth() {
        filledStatusBar.SetWidth(main.GetNumberOfFillableCells() * 20);
    }

    GameObject AddOverlay() 
    {
        var overlay = new GameObject();        
        overlay.name = "Overlay";
        overlay.transform.SetParent(gameObject.transform, false);
        overlay.transform.localScale = new Vector3(5f, 5f, 1f);
        overlay.transform.Translate(new Vector3(0, 0, 1));
        overlay.AddComponent<SpriteRenderer>();
        overlay.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/Overlay");
        overlay.GetComponent<SpriteRenderer>().color = ConvertColor(0, 0, 0, 80);
        return overlay;
    }

    StatusBar CreateShieldStatusBar(Color32 color) 
    {
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

    StatusBar CreateFilledStatusBar(Color32 color) 
    {
        var filledStatusBar = new GameObject();
        filledStatusBar.name = "Filled Status Bar";
        filledStatusBar.transform.SetParent(gameObject.transform, false);
        filledStatusBar.transform.localScale = new Vector3(2, 2, 2);
        filledStatusBar.transform.Translate(new Vector3(0, -25, 0));

        var statusBar = filledStatusBar.AddComponent<StatusBar>();
        statusBar.Init(color);
        statusBar.UpdateValue(0);
        statusBar.SetUpdateSpeedFactor(10);
        return statusBar;
    }

    Color ConvertColor (int r, int g, int b) {
        return new Color(r/255f, g/255f, b/255f);
    }
    Color ConvertColor (int r, int g, int b, int a) {
        return new Color(r/255f, g/255f, b/255f, a/255f);
    }
}
