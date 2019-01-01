using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Color basicColor = new Color32(42, 255, 42, 255); // green
    Color activeColor = new Color32(74, 255, 0, 255); // lighter green
    Color nonActiveColor = new Color32(255, 255, 255, 255); // white

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
        cellMaterial.color = GetInitialColor();
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
                // Over time, change the active color to be 40% non active color
                color = Color32.Lerp(activeColor, nonActiveColor, 0.4f * (Time.time - activatedTime) / activatedDuration);
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

    Color32 GetInitialColor() {
        if (cellType.Equals(Arena.CellType.ACTIVATABLE)) {
            return isActivated ? activeColor : nonActiveColor;
        } else {
            return basicColor;
        }
    }
}
