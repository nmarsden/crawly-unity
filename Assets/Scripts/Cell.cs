using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Color activeColor = new Color32(74, 255, 0, 255); // lighter green

    bool isActivated;
    float activatedTime;
    Main main;
    Arena.CellType cellType;

    static IDictionary<Arena.CellType, Material> cellMaterials;

    public void Init(Main main, Arena.CellType cellType) {
        this.main = main;
        this.cellType = cellType;

        var activatableMaterial = Resources.Load<Material>("Material/Cell_Activatable_Material");
        var basicMaterial = Resources.Load<Material>("Material/Cell_Basic_Material");
        var touchMaterial = Resources.Load<Material>("Material/Cell_Touch_Material");
        var wallMaterial = Resources.Load<Material>("Material/Cell_Wall_Material");

        cellMaterials = new Dictionary<Arena.CellType, Material>
        {
            { Arena.CellType.ACTIVATABLE,   activatableMaterial }, 
            { Arena.CellType.BASIC,         basicMaterial }, 
            { Arena.CellType.TOUCH,         touchMaterial }, 
            { Arena.CellType.WALL,          wallMaterial }, 
        };
    }

    void Start()
    {
        isActivated = cellType.Equals(Arena.CellType.BASIC);    
        gameObject.GetComponent<Renderer>().material = cellMaterials[cellType];

        if (cellType.Equals(Arena.CellType.WALL)) {
            // -- Wall Cell --
            // Adjust height & disable trigger
            var wallHeight = main.GetWallHeight();
            gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(1, wallHeight, 1));
            gameObject.transform.position += new Vector3(0, wallHeight/2 + 0.5f, 0);
            gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }

    void Update()
    {
        if (isActivated)
        {
            if (cellType.Equals(Arena.CellType.ACTIVATABLE) || cellType.Equals(Arena.CellType.TOUCH))
            {
                var activatedDuration = cellType.Equals(Arena.CellType.TOUCH) ? 0.1f : 3;
                if (Time.time - activatedTime > activatedDuration) {
                    isActivated = false;
                    // Return cell to original (pre-active) material 
                    gameObject.GetComponent<Renderer>().material = cellMaterials[cellType];
                }
            }
            else if (cellType.Equals(Arena.CellType.WALL)) 
            {
                isActivated = false;
                main.HandleHitWall();
            }
        }
    }

    public void TriggerEnter() {
        main.HandleHeadEnteredCell(gameObject.transform.position);
    }

    public void Activate() {
        activatedTime = Time.time;

        if (!isActivated) {
            isActivated = true;
           
            if (cellType.Equals(Arena.CellType.ACTIVATABLE) || cellType.Equals(Arena.CellType.TOUCH)) {
                // Indicate cell is active: Set material to basic with active color
                gameObject.GetComponent<Renderer>().material = cellMaterials[Arena.CellType.BASIC];
                gameObject.GetComponent<Renderer>().material.color = activeColor;

                main.HandleCellActivated();
            }
        }
    }

    public bool IsActivated() {
        return (IsTouch() || IsActivatable()) && isActivated;
    }

    public bool IsWall() {
        return cellType.Equals(Arena.CellType.WALL);
    }

    private bool IsTouch() {
        return cellType.Equals(Arena.CellType.TOUCH);
    }

    private bool IsActivatable() {
        return cellType.Equals(Arena.CellType.ACTIVATABLE);
    }
}
