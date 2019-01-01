using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Main main;
    Font font;
    GameObject fillText;
    GameObject gameCompletedText;
    GameObject gameOverText;
    int currentLevelNum;

    public void Init(Main main) 
    {
        this.main = main;
        currentLevelNum = main.GetCurrentLevelNum();
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
        var titleText = AddText("crawly", TextAnchor.LowerLeft, new Color32(0, 255, 0, 100));
        titleText.name = "Title";

        // Level Number text
        var levelNumText = AddText("Level " + currentLevelNum, TextAnchor.UpperRight, new Color32(255, 255, 255, 100));
        levelNumText.name = "Level Number";

        // Fill text
        fillText = AddText("Fill", TextAnchor.LowerRight, new Color32(0, 255, 0, 100));
        titleText.name = "Fill";

        // Game Completed text
        gameCompletedText = AddText("COMPLETE", TextAnchor.MiddleCenter, new Color32(0, 0, 100, 200));
        gameCompletedText.name = "Game Completed";
        gameCompletedText.SetActive(false);

        // Game Over text
        gameOverText = AddText("GAME OVER", TextAnchor.MiddleCenter, new Color32(0, 0, 100, 200));
        gameOverText.name = "Game Over";
        gameOverText.SetActive(false);
    }

    void Update()
    {
        fillText.GetComponent<Text>().text = "filled: " + main.GetFilledPercentage() + "%";
    }

    public void ShowGameOverMessage() {
        gameOverText.SetActive(true);
    }

    public void ShowGameCompletedMessage() {
        gameCompletedText.SetActive(true);
    }

    public GameObject AddText(string textContent, TextAnchor allignment, Color32 color) {
        var textObject = new GameObject();
        textObject.transform.parent = gameObject.transform;

        // Setup Text component
        var text = textObject.AddComponent<Text>();
        text.font = font;
        text.text = textContent;
        text.fontSize = 140;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = allignment;

        // Setup Text's RectTransform component
        var rectTransform = text.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        var hudRectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(hudRectTransform.rect.width, hudRectTransform.rect.height);
        rectTransform.localScale = new Vector3(1, 1, 1);

        return textObject;
    }
}
