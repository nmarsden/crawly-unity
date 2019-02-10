using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TitleScreen : MonoBehaviour
{
    enum ScreenMode { MAIN_MENU, HELP };

    delegate void Action();

    IDictionary<ScreenMode, Action> screenModeToAction;

    ScreenMode screenMode;
    Main main;
    int numberOfLevels;
    GameObject menuButtonPrefab;
    TMP_SpriteAsset spriteAsset;

    GameObject levelText;
    int selectedLevelNumber;
    GameObject cameraMount;

    public void Awake() {
        screenModeToAction = new Dictionary<ScreenMode, Action>
        {
            { ScreenMode.MAIN_MENU, InitMainMenuScreenMode },
            { ScreenMode.HELP,      InitHelpScreenMode },
        };

    }

    public void Init(Main main, int numberOfLevels) 
    {
        this.main = main;
        this.numberOfLevels = numberOfLevels;

        // Load menuButton prefab
        menuButtonPrefab = Resources.Load<GameObject>("UI/MenuButton");

        // Load sprite asset
        spriteAsset = Resources.Load<TMP_SpriteAsset>("UI/SpriteAsset");

        InitCameraMount();
        InitMainMenuScreen();
        InitHelpScreen();

        // Initially not active
        gameObject.SetActive(false);
    }

    public void Show(int selectedLevelNumber) {
        // Set intial camera position/rotation
        Camera.main.transform.position = new Vector3(25.6f, 25.5f, -26.5f);
        Camera.main.transform.rotation = Quaternion.Euler(30, -45, 0);

        InitScreenMode(ScreenMode.MAIN_MENU);
        UpdateSelectedLevel(selectedLevelNumber);
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (screenMode.Equals(ScreenMode.MAIN_MENU)) {
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
        } else if (screenMode.Equals(ScreenMode.HELP)) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) 
            {
                BackButtonOnClick();
            } 
        }
    }

    void LateUpdate() {
        // Move the camera to the mount position/rotation
        var t = 8f * Time.deltaTime;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraMount.transform.position, t);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraMount.transform.rotation, t);
    }

    void InitScreenMode(ScreenMode screenMode) {
        this.screenMode = screenMode;
        screenModeToAction[screenMode]();
    }

    void InitMainMenuScreenMode() {
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 55;

        cameraMount.transform.position = new Vector3(0, 1.5f, -15);
        cameraMount.transform.rotation = Quaternion.Euler(0, -18, 0);
    }

    void InitHelpScreenMode() {
        Camera.main.orthographic = false;
        Camera.main.fieldOfView = 55;

        cameraMount.transform.position = new Vector3(56, 1.5f, -15);
        cameraMount.transform.rotation = Quaternion.Euler(0, -18, 0);
    }

    void InitCameraMount() {
        cameraMount = new GameObject();
        cameraMount.name = "Camera Mount";
        cameraMount.transform.parent = gameObject.transform;
    }

    void InitMainMenuScreen() {

        GameObject screen = InitScreen();
        screen.name = "Main Menu Screen";
        var canvas = screen.GetComponent<Canvas>();

        // Title text
        var titleText = AddTextMeshPro(screen, "CRAWLY", new Color32(250, 189, 135, 255), new Vector2 (700, 170), new Color32(247, 255, 0, 255));
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(-8.5f, 4.5f, 0));

        // Level text
        levelText = AddTextMeshPro(screen, "LEVEL 1", new Color32(255, 255, 255, 250), new Vector2 (140, 30), new Color32(0, 0, 0, 200));
        levelText.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(new Color32(255, 255, 255, 250));
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

        // Main Menu Player
        var player = new GameObject();
        player.name = "Main Menu Player";
        player.transform.parent = screen.transform;
        player.AddComponent<Player>();
        player.GetComponent<Player>().InitForTitleScreen(this.main);
    }

    void InitHelpScreen() {
        GameObject screen = InitScreen();
        screen.name = "Help Screen";
        screen.transform.Translate(new Vector3(50, 0, 0));
        var canvas = screen.GetComponent<Canvas>();
        canvas.transform.Translate(new Vector3(0, 1, 0));

        // Title text
        var titleText = AddTitleText(screen, "HELP");
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(0, 7, 0));

        // Info Text
        var turnText = AddInfoText(screen, "<sprite=0> <sprite=1> - player controls");
        turnText.name = "Turn Text";
        turnText.transform.Translate(new Vector3(0, 4, 0));

        var viewText = AddInfoText(screen, "<sprite=2> - toggle view");
        viewText.name = "View Text";
        viewText.transform.Translate(new Vector3(0, 1, 0));

        var pauseText = AddInfoText(screen, "<sprite=3> - pause");
        pauseText.name = "Pause Text";
        pauseText.transform.Translate(new Vector3(0, -2, 0));

        // Back Button
        var helpButton = AddButton(canvas, "BACK", 100);
        helpButton.name = "Back Button";
        helpButton.transform.Translate(new Vector3(0, -5.5f, 0));
        helpButton.GetComponent<Button>().onClick.AddListener(BackButtonOnClick);
    }

    GameObject InitScreen() {
        GameObject screen = new GameObject();
        screen.transform.parent = gameObject.transform;

        // Add components
        screen.AddComponent<Canvas>();
        screen.AddComponent<CanvasScaler>();
        screen.AddComponent<GraphicRaycaster>();
        screen.transform.position = new Vector3 (0, 0, 0);
        screen.layer = 5; // UI

        // Setup canvas
        var canvas = screen.GetComponent<Canvas>();
        canvas.transform.position = new Vector3 (0, 0, 0);

        canvas.renderMode = RenderMode.WorldSpace;

        var canvasRectTransform = canvas.GetComponent<RectTransform>();
        canvasRectTransform.localScale = new Vector3(0.05f, 0.05f, 1);
        canvasRectTransform.sizeDelta = new Vector2(960, 600);
        canvasRectTransform.anchorMin = new Vector2(0, 0);
        canvasRectTransform.anchorMax = new Vector2(0, 0);

        return screen;
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
        InitScreenMode(ScreenMode.HELP);
    }

    void BackButtonOnClick() {
        main.HandleButtonPressedFX();
        InitScreenMode(ScreenMode.MAIN_MENU);
    }

    private GameObject AddTitleText(GameObject parent, string textContent) 
    {
        var titleColor = new Color32(12, 46, 18, 200);
        var outlineColor = new Color32(255, 255, 255, 250);
        var size = new Vector2 (700, 50);

        var titleText = AddTextMeshPro(parent, textContent, titleColor, size, outlineColor);
        titleText.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(titleColor);

        return titleText;
    }

    private GameObject AddInfoText(GameObject parent, string textContent) 
    {
        var infoColor = new Color32(255, 255, 255, 250);
        var outlineColor = new Color32(0, 0, 0, 200);
        var size = new Vector2 (700, 50);

        var infoText = AddTextMeshPro(parent, textContent, infoColor, size, outlineColor);
        infoText.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(infoColor);
        infoText.GetComponent<TextMeshProUGUI>().spriteAsset = spriteAsset;

        return infoText;
    }

    public GameObject AddTextMeshPro(GameObject parent, string textContent, Color32 color, Vector2 size, Color32 outlineColor) {
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
        var txtMesh = AddTextMeshPro(button, textContent, textColor, new Vector2 (70, 30), outlineColor);
        txtMesh.GetComponent<TextMeshProUGUI>().colorGradient = new VertexGradient(textColor);
        txtMesh.name = "TextMeshPro";

        return button;
    }

    Color ConvertColor (int r, int g, int b) {
        return new Color(r/255f, g/255f, b/255f);
    }

}
