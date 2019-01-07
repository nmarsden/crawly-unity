using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpScreen : MonoBehaviour
{
    Main main;
    Font font;
    GameObject menuButtonPrefab;

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
        var titleText = AddTextMesh(gameObject, "help", TextAnchor.MiddleCenter, new Color32(0, 255, 0, 100), 10);
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(0, 20, 0));

        // Info Text
        var infoColor = new Color32(12, 46, 18, 100);

        var turnText = AddTextMesh(gameObject, "\u2B05 \u27A1 - player controls", TextAnchor.MiddleCenter, infoColor, 3);
        turnText.name = "Turn Text";
        turnText.transform.Translate(new Vector3(0, 2, 0));

        var pauseText = AddTextMesh(gameObject, "p - toggle pause", TextAnchor.MiddleCenter, infoColor, 3);
        pauseText.name = "Pause Text";
        pauseText.transform.Translate(new Vector3(0, -5, 0));

        var viewText = AddTextMesh(gameObject, "v - toggle view", TextAnchor.MiddleCenter, infoColor, 3);
        viewText.name = "View Text";
        viewText.transform.Translate(new Vector3(0, -12, 0));

        var quitText = AddTextMesh(gameObject, "esc - quit", TextAnchor.MiddleCenter, infoColor, 3);
        quitText.name = "Quit Text";
        quitText.transform.Translate(new Vector3(0, -19, 0));

        // Close Button
        var closeButton = AddButton("\u2716", 120);
        closeButton.name = "Close Button";
        closeButton.transform.Translate(new Vector3(40, 21, 0));
        closeButton.GetComponent<Button>().onClick.AddListener(CloseButtonOnClick);
    }

    public void Init(Main main) 
    {
        this.main = main;
    }

    public void Show() {
        InitCamera();
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape)) 
        {
            CloseButtonOnClick();
        } 
    }

    void InitCamera() {
        Camera.main.orthographic = true;
        Camera.main.transform.position = new Vector3(55.6f, 45.5f, -56.5f);
        Camera.main.transform.rotation = Quaternion.Euler(30, -45, 0);
    }

    void CloseButtonOnClick() {
        main.HandleButtonPressedFX();
        main.HandleHelpCloseButtonPressed();
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
