using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    GameObject statusBar;
    Slider slider;

    public void Init(Color32 color)
    {
        statusBar = Instantiate(Resources.Load<GameObject>("UI/StatusBar"));
        statusBar.name = "Status Bar";
        statusBar.transform.SetParent(gameObject.transform, false);

        SetColor(color);

        slider = statusBar.transform.GetChild(0).GetChild(0).GetComponent<Slider>();
    }

    public void UpdateValue(float value)
    {
        slider.value = value;
    }

    private void SetColor(Color32 color) {
        // Panel > Slider > Fill Area > Fill
        var fill = statusBar.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        fill.GetComponent<Image>().color = color;
    }

}
