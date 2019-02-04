using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Color basicColor = new Color32(42, 255, 42, 255); // green
    Color activatableColor = new Color32(255, 255, 255, 255); // white
    Color touchColor = new Color32(255, 255, 255, 255); // white
    Color activeColor = new Color32(74, 255, 0, 255); // lighter green
    Color wallColor = new Color32(239, 27, 33, 255); // red

    bool isActivated;
    float activatedTime;
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
            // -- Wall Cell --
            // Adjust height & disable trigger
            var wallHeight = main.GetWallHeight();
            gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(1, wallHeight, 1));
            gameObject.transform.position += new Vector3(0, wallHeight/2 + 0.5f, 0);
            gameObject.GetComponent<Collider>().isTrigger = false;
        } else {
            // -- Non-Wall Cell --
            // Give cell material an edge emission texture
            var emissionTexture =  Resources.Load<Texture>("Image/Edge_Emission");
            cellMaterial.EnableKeyword("_EMISSION");
            cellMaterial.SetColor("_EmissionColor", new Color32(255, 255, 255, 255)); // White
            cellMaterial.SetTexture("_EmissionMap", emissionTexture);

            // Set main texture for Touch Cell
            if (cellType.Equals(Arena.CellType.TOUCH)) {
                var cellTexture =  Resources.Load<Texture>("Image/Touch_Cell");
                cellMaterial.SetTexture("_MainTex", cellTexture);
            }
        }
    }

    void Update()
    {
        if (cellType.Equals(Arena.CellType.ACTIVATABLE)) 
        {
            UpdateColor(activatableColor, 3);
        } 
        else if (cellType.Equals(Arena.CellType.TOUCH)) 
        {
            UpdateColor(touchColor, 0.1f);
        } 
        else if (cellType.Equals(Arena.CellType.WALL)) 
        {
            if (isActivated) 
            {
                isActivated = false;
                main.HandleHitWall();
            }
        }
    }

    public void Activate() {
        activatedTime = Time.time;

        if (!isActivated) {
            isActivated = true;
            if (cellType.Equals(Arena.CellType.ACTIVATABLE) || cellType.Equals(Arena.CellType.TOUCH)) {
                main.HandleCellActivated();
            }
        }
    }

    public bool IsActivated() {
        return isActivated;
    }

    public bool IsWall() {
        return cellType.Equals(Arena.CellType.WALL);
    }

    void UpdateColor(Color32 nonActiveColor, float activatedDuration) {
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

    Color32 GetInitialColor() {
        if (cellType.Equals(Arena.CellType.ACTIVATABLE)) {
            return activatableColor;
        } else if (cellType.Equals(Arena.CellType.WALL)) {
            return wallColor;
        } else if (cellType.Equals(Arena.CellType.TOUCH)) {
            return touchColor;
        } else {
            return basicColor;
        }
    }
}
