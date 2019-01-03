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
        var titleText = AddText(gameObject, "crawly", TextAnchor.MiddleCenter, new Color32(0, 255, 0, 100));
        titleText.name = "Title";
        titleText.GetComponent<RectTransform>().Translate(new Vector3(0, 7, 0));

        // Play Button
        var playButton = Instantiate(menuButtonPrefab);
        playButton.name = "Play Button";
        playButton.transform.SetParent(gameObject.transform, false);

        var playButtonRectTransform = playButton.GetComponent<RectTransform>();
        playButtonRectTransform.sizeDelta = new Vector2 (320, 120);

        var txt = playButton.GetComponentInChildren<Text>();
        txt.text = "PLAY";
        txt.font = font;
        txt.fontSize = 300;
        txt.fontStyle = FontStyle.Bold;
        txt.color = new Color32(0, 0, 0, 255);

        Button btn = playButton.GetComponent<Button>();
        btn.onClick.AddListener(PlayButtonOnClick);

        var btnColorBlock = ColorBlock.defaultColorBlock;
        btnColorBlock.normalColor = ConvertColor(49, 77, 121);
        btnColorBlock.highlightedColor = ConvertColor(0, 150, 0);
        btnColorBlock.pressedColor = ConvertColor(0, 100, 0);
        btn.colors = btnColorBlock;

        // Setup Text's RectTransform component
        var rectTransform = txt.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        var hudRectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(hudRectTransform.rect.width, hudRectTransform.rect.height);
        rectTransform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
    }

    Color ConvertColor (int r, int g, int b) {
        return new Color(r/255f, g/255f, b/255f);
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

    public GameObject AddText(GameObject parent, string textContent, TextAnchor allignment, Color32 color) {
        var textObject = new GameObject();
        textObject.transform.parent = parent.transform;

        // Setup Shadow
        var shadow = textObject.AddComponent<Shadow>();
        shadow.effectDistance = new Vector2(-2, -2);

        // Setup Text component
        var text = textObject.AddComponent<Text>();
        text.font = font;
        text.text = textContent;
        text.fontSize = 240;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = allignment;

        // Setup Text's RectTransform component
        var rectTransform = text.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(parent.transform.position, parent.transform.rotation);
        var hudRectTransform = parent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(hudRectTransform.rect.width, hudRectTransform.rect.height);
        rectTransform.localScale = new Vector3(1, 1, 1);

        return textObject;
    }
}
