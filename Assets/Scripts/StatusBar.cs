using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    GameObject statusBar;
    Slider slider;
    float value;
    float updateSpeedFactor = 20f;

    public void Init(Color32 color)
    {
        statusBar = Instantiate(Resources.Load<GameObject>("UI/StatusBar"));
        statusBar.name = "Status Bar";
        statusBar.transform.SetParent(gameObject.transform, false);


        SetColor(color);

        slider = statusBar.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
    }

    public void Update() 
    {
        if (value == 1) 
        {
            slider.value = 1;
        } else {
            var t = updateSpeedFactor * Time.deltaTime;
            slider.value = Mathf.Lerp(slider.value, value, t);
        }
    }

    public void UpdateValue(float value)
    {
        this.value = value;
    }

    public void SetWidth(int width) 
    {
        var rectTransform = statusBar.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2 (width, rectTransform.sizeDelta.y);
    }

    public void SetUpdateSpeedFactor(float updateSpeedFactor)
    {
        this.updateSpeedFactor = updateSpeedFactor;
    }

    private void SetColor(Color32 color) {
        // Panel > Slider > Fill Area > Fill
        var fill = statusBar.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        fill.GetComponent<Image>().color = color;
    }

}
