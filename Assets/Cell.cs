using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Color basicColor = new Color32(42, 255, 42, 255); // green
    Color activeColor = new Color32(74, 255, 0, 255); // lighter green
    Color nonActiveColor = new Color32(255, 255, 255, 255); // white
    Color wallColor = new Color32(239, 27, 33, 255); // red

    bool isActivated;
    float activatedTime;
    float activatedDuration = 3;
    Material cellMaterial;
    Main main;
    Arena.CellType cellType;

    public void Init(Main main, Arena.CellType cellType) {
        this.main = main;
        this.cellType = cellType;
    }

    void Start()
    {
        isActivated = cellType.Equals(Arena.CellType.BASIC);    
        cellMaterial = gameObject.GetComponent<Renderer>().material;
        cellMaterial.color = GetInitialColor();

        if (cellType.Equals(Arena.CellType.WALL)) {
            var wallHeight = main.GetWallHeight();
            gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(1, wallHeight, 1));
            gameObject.transform.position += new Vector3(0, wallHeight/2 + 0.5f, 0);
        }
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
        } else if (cellType.Equals(Arena.CellType.WALL)) {
            if (isActivated) {
                isActivated = false;
                main.HandleHitWall();
            }
        }
    }

    public void Activate() {
        isActivated = true;
        activatedTime = Time.time;
    }

    public bool IsActivated() {
        return isActivated;
    }

    public bool IsWall() {
        return cellType.Equals(Arena.CellType.WALL);
    }

    Color32 GetInitialColor() {
        if (cellType.Equals(Arena.CellType.ACTIVATABLE)) {
            return isActivated ? activeColor : nonActiveColor;
        } else if (cellType.Equals(Arena.CellType.WALL)) {
            return wallColor;
        } else {
            return basicColor;
        }
    }
}
