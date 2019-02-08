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

    public class ArenaPosition
    {
        public int row { get; private set; }
        public int col { get; private set; }

        public ArenaPosition(int row, int col) {
            this.row = row;
            this.col = col;
        }

        public override bool Equals(object obj)
        {
            if (obj is ArenaPosition)
            {
                return this.Equals((ArenaPosition)obj);
            }
            return false;
        }

        public bool Equals(ArenaPosition p)
        {
            return (row == p.row) && (col == p.col);
        }   

        public override int GetHashCode()
        {
            return row ^ col;
        } 
    }

    public enum CellType { BASIC, WALL, ACTIVATABLE, TOUCH };
    Color floorColor = new Color32(0, 0, 0, 255); // black
    Color wallColor = new Color32(239, 27, 33, 255); // red
    //Color wallColor = new Color32(67, 67, 191, 255); // blue
    Color gridColor = new Color32(0, 0, 0, 255);
    Color pathColor = new Color32(200, 0, 0, 255);
    GameObject gridLines;
    private float gridSpacing;
    private int numberOfFillableCells;
    private float wallWidth = 5f;
    private float wallHeight = 2.5f;
    private float arenaWidth;
    private float playerHeight;
    private bool isShowTurningPoints;
    private bool isShowCellTriggers;
    private bool isAllCellsActivated;
    private int totalCellCount;
    private int totalCellTriggerCount;
    private int totalWallCount;
    private GameObject cellsContainer;
    private int currentLevelNum;
    
    private Main main;
    public void Init(Main main) {
        this.main = main;
        arenaWidth = main.GetArenaWidth();
        gridSpacing = main.GetGridSpacing();
        numberOfFillableCells = main.GetNumberOfFillableCells();
        playerHeight = main.GetPlayerHeight();
        isShowTurningPoints = main.IsShowTurningPoints();
        isShowCellTriggers = main.IsShowCellTriggers();
        currentLevelNum = main.GetCurrentLevelNum();
        totalCellCount = 0;
        totalCellTriggerCount = 0;
        isAllCellsActivated = false;
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
        // var yPos = 0.1f - playerHeight/2;
        
        // Draw grid lines
        // for (float i = -(arenaWidth/2) + gridSpacing; i < (arenaWidth/2); i = i + gridSpacing) {
        //     DrawLine(gridLines, new Vector3(i, yPos, -(arenaWidth/2)), new Vector3(i, yPos, (arenaWidth/2)), gridColor, 0.1f, 0.2f);
        //     DrawLine(gridLines, new Vector3(-(arenaWidth/2), yPos, i), new Vector3((arenaWidth/2), yPos, i), gridColor, 0.1f, 0.2f);
        // }

        // Draw turning path
        // if (isShowTurningPoints) {
        //     var turningPositions = main.GetTurningPoints().GetPositions();

        //     for (var i = 0; i<turningPositions.Length-1; i++) {
        //         var pos1 = new Vector3(turningPositions[i].x, yPos, turningPositions[i].z);
        //         var pos2 = new Vector3(turningPositions[i+1].x, yPos, turningPositions[i+1].z);
        //         DrawLine(gridLines, pos1, pos2, pathColor, 1, 0.2f);
        //     }
        // }
    }

    void LateUpdate() {
        // Check if all cells are activated
        if (GetNumberOfActivatedCells() == numberOfFillableCells) {
            AllCellsActivated();
        }
    }

    void AllCellsActivated() {
        if (!isAllCellsActivated) {
            isAllCellsActivated = true;
            main.HandleAllCellsActivated();
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
        var arenaPadding = 0.5f; // <- padding between arena and wall so that the wall is not hit when turning next to it.
        var yPos = (wallHeight/2);
        var wallLength = arenaWidth + (arenaPadding*2) + (wallWidth*2);
        var halfWallWidth = (wallWidth/2);
        var halfArenaWidth = (arenaWidth/2);
        var offset = halfArenaWidth + halfWallWidth + arenaPadding; 

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
        wall.AddComponent<WallTrigger>();
        wall.GetComponent<WallTrigger>().Init(main);
    }

    void AddCells() {
        cellsContainer = new GameObject();
        cellsContainer.name = "Cells";
        cellsContainer.transform.parent = transform;    

        var levelWidthInCells = (int) Mathf.Floor(arenaWidth / gridSpacing);
        var cellTypes = main.GetLevelCellTypes();

        for (var row = 0; row < levelWidthInCells; row++) {
            for (var col = 0; col < levelWidthInCells; col++) {
                var cellType = cellTypes[row, col];
                var cellPos = ToCellPosition(row, col);

                GameObject cell = AddCell(cellsContainer, cellPos, cellType);
                AddCellTrigger(cell, new Vector3(cellPos.x, 0, cellPos.z));
            }
        }
    }

    public List<Vector3> ToCellPositions(List<ArenaPosition> arenaPositions) 
    {
        return arenaPositions.ConvertAll(ap => ToCellPosition(ap.row, ap.col));
    }

    Vector3 ToCellPosition(int cellRow, int cellCol) {
        float x = -(arenaWidth/2) + (gridSpacing/2) + (cellRow * gridSpacing);
        float y = -(0.4f + (playerHeight/2));
        float z = -(arenaWidth/2) + (gridSpacing/2) + (cellCol * gridSpacing);
        return new Vector3(x, y, z);
    }

    GameObject AddCell(GameObject cellsContainer, Vector3 position, CellType cellType) {
        GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cell.name = "Cell " + totalCellCount++;
        cell.transform.parent = cellsContainer.transform;    
        cell.transform.localScale = new Vector3(gridSpacing, 1, gridSpacing);        
        cell.transform.position = position;
        cell.GetComponent<Collider>().isTrigger = true;
        cell.AddComponent<Cell>();
        cell.GetComponent<Cell>().Init(main, cellType);
        return cell;
    }

    void AddCellTrigger(GameObject cell, Vector3 position) {
        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trigger.name = "Cell Trigger" + totalCellTriggerCount++;
        trigger.transform.parent = cell.transform;    
        trigger.transform.localScale = new Vector3(1, playerHeight, 1);        
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

    public List<ArenaPosition> GetEmptyArenaPositions() {
        var cellTriggers = cellsContainer.GetComponentsInChildren<CellTrigger>();
        var emptyPositions = new List<ArenaPosition>();
        foreach (CellTrigger cellTrigger in cellTriggers) {
            var cell = cellTrigger.transform.parent.GetComponent<Cell>();
            if (!cell.IsWall() && !cellTrigger.IsTriggered()) {
                emptyPositions.Add(ToArenaPosition(cellTrigger.GetPosition()));
            }
        }
        return emptyPositions;
    }

    public List<ArenaPosition> ToArenaPositions(List<Vector3> positions) {
        return positions.ConvertAll(ToArenaPosition);
    }

    ArenaPosition ToArenaPosition(Vector3 position) {
        var row = Mathf.RoundToInt((position.x + (arenaWidth/2) - (gridSpacing/2)) / gridSpacing);
        var col = Mathf.RoundToInt((position.z + (arenaWidth/2) - (gridSpacing/2)) / gridSpacing);
        return new ArenaPosition(row, col);
    }
    
    public int GetNumberOfActivatedCells() {
        if (cellsContainer == null)
        {
            return 0;
        }
        var cells = cellsContainer.GetComponentsInChildren<Cell>();
        var numberOfActiveCells = 0;
        foreach (Cell cell in cells) {
            if (cell.IsActivated()) {
                numberOfActiveCells++;
            }
        }
        return numberOfActiveCells;
    }

}
