using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Main main;
    Font font;
    GameObject gameOverText;

    public void Init(Main main) 
    {
        this.main = main;
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

        // Game Over text
        gameOverText = AddText("GAME OVER", TextAnchor.MiddleCenter, new Color32(200, 0, 0, 255));
        gameOverText.name = "Game Over";
        gameOverText.SetActive(false);
    }

    void Update()
    {
        
    }

    public void ShowGameOverMessage() {
        gameOverText.SetActive(true);
    }

    public GameObject AddText(string textContent, TextAnchor allignment, Color32 color) {
        var myText = new GameObject();
        myText.transform.parent = gameObject.transform;

        var text = myText.AddComponent<Text>();
        text.font = font;
        text.text = textContent;
        text.fontSize = 140;
        text.fontStyle = FontStyle.Bold;
        text.color = color;
        text.alignment = allignment;

        // Title text position
        var rectTransform = text.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        var hudRectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(hudRectTransform.rect.width, hudRectTransform.rect.height);
        rectTransform.localScale = new Vector3(1, 1, 1);

        return myText;
    }
}
