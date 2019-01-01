using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Color activeColor = new Color32(200, 0, 0, 255);
    Color nonActiveColor = new Color32(255, 255, 255, 255);

    bool isActivated;
    float activatedTime;
    float activatedDuration = 3;
    Material cellMaterial;
    Arena.CellType cellType;

    public void Init(Arena.CellType cellType) {
        this.cellType = cellType;
    }

    void Start()
    {
        isActivated = cellType.Equals(Arena.CellType.BASIC);    
        cellMaterial = gameObject.GetComponent<Renderer>().material;
        cellMaterial.color = isActivated ? activeColor : nonActiveColor;
    }

    void Update()
    {
        // -- Update 'Activatable' cell
        if (cellType.Equals(Arena.CellType.ACTIVATABLE)) {
            // Make non-activated after a while
            if (isActivated) {
                if (Time.time - activatedTime > activatedDuration) {
                    isActivated = false;
                }
            }
            // Update color
            var color = nonActiveColor;
            if (isActivated) {
                // Over time, change the active color to be 50% non active color
                color = Color32.Lerp(activeColor, nonActiveColor, 0.5f * (Time.time - activatedTime) / activatedDuration);
            }
            cellMaterial.color = color;
        }

    }

    public void Activate() {
        isActivated = true;
        activatedTime = Time.time;
    }

    public bool IsActivated() {
        return isActivated;
    }

}
