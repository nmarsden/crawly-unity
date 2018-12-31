using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    Main main;
    Font font;

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

        // Title Text
        var myText = new GameObject();
        myText.transform.parent = gameObject.transform;
        myText.name = "Title";

        var text = myText.AddComponent<Text>();
        text.font = font;
        text.text = "crawly";
        text.fontSize = 140;
        text.fontStyle = FontStyle.Bold;
        text.color = new Color32(0, 255, 0, 100);
        text.alignment = TextAnchor.LowerLeft;

        // Title text position
        var rectTransform = text.GetComponent<RectTransform>();
        rectTransform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        var hudRectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(hudRectTransform.rect.width, hudRectTransform.rect.height);
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    void Update()
    {
        
    }
}
