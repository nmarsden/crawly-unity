using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    Main main;
    Font font;
    GameObject menuButtonPrefab;

    public void Init(Main main) 
    {
        this.main = main;
        menuButtonPrefab = Resources.Load<GameObject>("UI/MenuButton");
    }

    void Start()
    {
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
        font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        // Title text
        var titleText = AddTextMesh(gameObject, "crawly", TextAnchor.MiddleCenter, new Color32(0, 255, 0, 100), 12);
        titleText.name = "Title";
        titleText.transform.Translate(new Vector3(0, 7, 0));

        // Play Button
        var playButton = AddButton("PLAY");
        playButton.name = "Play Button";
        playButton.transform.Translate(new Vector3(0, -5, 0));
        playButton.GetComponent<Button>().onClick.AddListener(PlayButtonOnClick);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            PlayButtonOnClick();
        }
    }

    void PlayButtonOnClick() {
        main.HandlePlayButtonPressed();
    }

    public GameObject AddTextMesh(GameObject parent, string textContent, TextAnchor alignment, Color32 color, float scale) {
        // Note: TextMesh scales better than regular Text
        var txtMesh = new GameObject();
        txtMesh.transform.SetParent(parent.transform, false);
        txtMesh.transform.localScale = new Vector3(scale, scale, scale);
        var tm = txtMesh.AddComponent<TextMesh>();
        tm.text = textContent;
        tm.fontSize = 200;
        tm.fontStyle = FontStyle.Bold;
        tm.color = color;
        tm.offsetZ = -10;
        tm.anchor = alignment;

        return txtMesh;
    }

    public GameObject AddButton(string textContent) {
        // Create button & set parent
        var button = Instantiate(menuButtonPrefab);
        button.transform.SetParent(gameObject.transform, false);

        // Setup button width & height
        var playButtonRectTransform = button.GetComponent<RectTransform>();
        playButtonRectTransform.sizeDelta = new Vector2 (320, 120);

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
