using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    Main main;
    int numberOfLevels;
    GameObject menuButtonPrefab;
    GameObject levelText;
    int selectedLevelNumber;

    GameObject player;

    void Awake()
    {
        // Load menuButton prefab
        menuButtonPrefab = Resources.Load<GameObject>("UI/MenuButton");

        // Add components
        gameObject.AddComponent<Canvas>();
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
        gameObject.transform.position = new Vector3 (0, 0, 0);
        gameObject.layer = 5; // UI

        // Setup canvas
        var canvas = gameObject.GetComponent<Canvas>();
        canvas.transform.position = new Vector3 (0, 0, 0);

        canvas.renderMode = RenderMode.WorldSpace;

        var canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasRectTransform.localScale = new Vector3(0.05f, 0.05f, 1);
        canvasRectTransform.sizeDelta = new Vector2(960, 600);
        canvasRectTransform.anchorMin = new Vector2(0, 0);
        canvasRectTransform.anchorMax = new Vector2(0, 0);

        // Title text
        var titleText = AddTextMeshPro(gameObject, "CRAWLY", TextAnchor.MiddleCenter, new Color32(250, 189, 135, 255), new Vector2 (700, 170), new Color32(247, 255, 0, 255));
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(-8.5f, 4.5f, 0));

        // Level text
        levelText = AddTextMeshPro(gameObject, "LEVEL 1", TextAnchor.MiddleCenter, new Color32(32, 255, 0, 100), new Vector2 (140, 30), new Color32(255, 255, 255, 255));
        levelText.transform.Translate(new Vector3(0, 0.2f, 0));
        levelText.name = "Level";

        // Previous Level Button
        var prevLevelButton = AddButton(canvas, '<'.ToString(), 50);
        prevLevelButton.name = "Previous Level Button";
        prevLevelButton.transform.Translate(new Vector3(-4, 0, 0));
        prevLevelButton.GetComponent<Button>().onClick.AddListener(SelectPreviousLevel);

        // Next Level Button
        var nextLevelButton = AddButton(canvas, '>'.ToString(), 50);
        nextLevelButton.name = "Next Level Button";
        nextLevelButton.transform.Translate(new Vector3(4, 0, 0));
        nextLevelButton.GetComponent<Button>().onClick.AddListener(SelectNextLevel);

        // Play Button
        var playButton = AddButton(canvas, "PLAY", 130);
        playButton.name = "Play Button";
        playButton.transform.Translate(new Vector3(0, -2, 0));
        playButton.GetComponent<Button>().onClick.AddListener(PlayButtonOnClick);

        // Help Button
        var helpButton = AddButton(canvas, "HELP", 130);
        helpButton.name = "Help Button";
        helpButton.transform.Translate(new Vector3(0, -4, 0));
        helpButton.GetComponent<Button>().onClick.AddListener(HelpButtonOnClick);

        // Initially not active
        gameObject.SetActive(false);
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

        // Title Screen Player
        player = new GameObject();
        player.name = "Title Screen Player";
        player.AddComponent<Player>();
        player.GetComponent<Player>().InitForTitleScreen(this.main);
    }

    public void Hide() {
        gameObject.SetActive(false);

        Object.Destroy(player);
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
        Camera.main.orthographic = false;
        Camera.main.transform.position = new Vector3(0, 1.5f, -15);
        Camera.main.transform.rotation = Quaternion.Euler(0, -18, 0);
        Camera.main.fieldOfView = 55;
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
        levelText.GetComponent<TextMeshProUGUI>().text = "Level " + selectedLevelNumber;
    }

    void PlayButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandlePlayButtonPressed(selectedLevelNumber);
    }

    void HelpButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandleHelpButtonPressed();
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
        playButtonRectTransform.sizeDelta = new Vector2 (width, 30);

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
        var txtMesh = AddTextMeshPro(button, textContent, TextAnchor.MiddleCenter, new Color32(32, 255, 0, 100), new Vector2 (70, 30), new Color32(255, 255, 255, 255));
        txtMesh.name = "TextMeshPro";

        return button;
    }

    Color ConvertColor (int r, int g, int b) {
        return new Color(r/255f, g/255f, b/255f);
    }

}
