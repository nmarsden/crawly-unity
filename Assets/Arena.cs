using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
        public class WallTrigger : MonoBehaviour
    {
        Main main;

        public void Init(Main main) 
        {
            this.main = main;
        }
        void OnTriggerEnter(Collider collider) 
        {
            main.HandleHitWall();
        }

    }

    Color floorColor = new Color32(255, 255, 255, 255);
    Color wallColor = new Color32(0, 0, 200, 255);
    Color gridColor = new Color32(0, 0, 0, 255);
    Color pathColor = new Color32(200, 0, 0, 255);
    GameObject gridLines;
    private float gridSpacing;
    private float wallWidth = 5f;
    private float wallHeight = 2.5f;
    private float arenaWidth;
    private float playerHeight;
    private bool isShowTurningPoints;
    private bool isShowCellTriggers;
    private int totalCellCount;
    private int totalWallCount;
    private GameObject cellsContainer;
    
    private Main main;
    public void Init(Main main) {
        this.main = main;
        arenaWidth = main.GetArenaWidth();
        gridSpacing = main.GetGridSpacing();
        playerHeight = main.GetPlayerHeight();
        isShowTurningPoints = main.IsShowTurningPoints();
        isShowCellTriggers = main.IsShowCellTriggers();
        totalCellCount = 0;
    }

    void Start()
    {
        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.parent = transform;    
        floor.transform.localScale = new Vector3(arenaWidth + wallWidth, 1, arenaWidth + wallWidth);        
        floor.transform.position = new Vector3(0, -0.6f, 0);
        AdjustPositionForPlayerHeight(floor.transform);
        floor.GetComponent<Renderer>().material.color = floorColor;

        // Walls
        AddWalls();

        // Cells
        AddCells();

        // Grid Lines
        gridLines = new GameObject();
        gridLines.name = "Grid Lines";
        gridLines.transform.parent = transform;    
    }

    void Update()
    {
        // Draw grid lines
        var yPos = 0.1f - playerHeight/2;
        for (float i = -(arenaWidth/2) + gridSpacing; i < (arenaWidth/2); i = i + gridSpacing) {
            DrawLine(gridLines, new Vector3(i, yPos, -(arenaWidth/2)), new Vector3(i, yPos, (arenaWidth/2)), gridColor, 0.1f, 0.2f);
            DrawLine(gridLines, new Vector3(-(arenaWidth/2), yPos, i), new Vector3((arenaWidth/2), yPos, i), gridColor, 0.1f, 0.2f);
        }

        // Draw turning path
        if (isShowTurningPoints) {
            var turningPositions = main.GetTurningPoints().GetPositions();

            for (var i = 0; i<turningPositions.Length-1; i++) {
                var pos1 = new Vector3(turningPositions[i].x, yPos, turningPositions[i].z);
                var pos2 = new Vector3(turningPositions[i+1].x, yPos, turningPositions[i+1].z);
                DrawLine(gridLines, pos1, pos2, pathColor, 1, 0.2f);
            }
        }
    }

    void AdjustPositionForPlayerHeight(Transform transform) {
        transform.position += new Vector3(0, -playerHeight/2, 0);
    }

    void DrawLine(GameObject parent, Vector3 start, Vector3 end, Color color, float width, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.name = "Line";
        myLine.transform.parent = parent.transform;    
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = width;
        lr.endWidth = width;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    void AddWalls() {
        var yPos = (wallHeight/2);
        var wallLength = arenaWidth + (wallWidth*2);
        var halfWallWidth = (wallWidth/2);
        var halfArenaWidth = (arenaWidth/2);
        var offset = halfArenaWidth + halfWallWidth + 0.5f; // <- extra 0.5f gap so that the wall is not hit when turning next to it.

        AddWall(new Vector3(      0, yPos,  offset), new Vector3(wallLength, wallHeight,  wallWidth));
        AddWall(new Vector3(      0, yPos, -offset), new Vector3(wallLength, wallHeight,  wallWidth));        
        AddWall(new Vector3( offset, yPos,       0), new Vector3(wallWidth,  wallHeight, wallLength));         
        AddWall(new Vector3(-offset, yPos,       0), new Vector3(wallWidth,  wallHeight, wallLength));         
    }

    void AddWall(Vector3 position, Vector3 scale) {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall " + totalWallCount++;
        wall.transform.parent = transform;    
        wall.transform.localScale = scale;        
        wall.transform.position = position;
        AdjustPositionForPlayerHeight(wall.transform);
        wall.GetComponent<Renderer>().material.color = wallColor;
        wall.GetComponent<Collider>().isTrigger = true;
        wall.AddComponent<WallTrigger>();
        wall.GetComponent<WallTrigger>().Init(main);
    }

    void AddCells() {
        cellsContainer = new GameObject();
        cellsContainer.name = "Cells";
        cellsContainer.transform.parent = transform;    

        for (float x = -(arenaWidth/2) + (gridSpacing/2); x < (arenaWidth/2); x = x + gridSpacing) {
            for (float z = -(arenaWidth/2) + (gridSpacing/2); z < (arenaWidth/2); z = z + gridSpacing) {
                AddCell(cellsContainer, new Vector3(x, 0, z));
            }
        }
    }

    void AddCell(GameObject cellsContainer, Vector3 position) {

        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trigger.name = "Cell " + totalCellCount++;
        trigger.transform.parent = cellsContainer.transform;    
        trigger.transform.localScale = new Vector3(gridSpacing, playerHeight, gridSpacing);        
        trigger.transform.position = position;

        if (isShowCellTriggers) {
            Material material = new Material(Shader.Find("Transparent/Diffuse"));
            trigger.GetComponent<Renderer>().material = material;
        } else {
            Object.Destroy(trigger.GetComponent<Renderer>());
        }
        
        trigger.GetComponent<Collider>().isTrigger = true;
        trigger.AddComponent<CellTrigger>();
    }

    public List<Vector3> GetEmptyPositions() {
        var cellTriggers = cellsContainer.GetComponentsInChildren<CellTrigger>();
        var emptyPositions = new List<Vector3>();
        foreach (CellTrigger cellTrigger in cellTriggers) {
            if (!cellTrigger.IsTriggered()) {
                emptyPositions.Add(cellTrigger.GetPosition());
            }
        }
        return emptyPositions;
    }
}
