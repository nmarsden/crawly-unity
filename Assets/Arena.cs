using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    Color floorColor = new Color32(255, 255, 255, 255);
    Color wallColor = new Color32(0, 0, 200, 255);
    Color gridColor = new Color32(0, 0, 0, 255);
    Color pathColor = new Color32(200, 0, 0, 255);
    GameObject gridLines;
    private float gridSpacing;
    private float wallWidth = 5f;
    private float arenaWidth;
    private float playerHeight;
    
    private Main main;
    public void Init(Main main) {
        this.main = main;
        arenaWidth = main.GetArenaWidth();
        gridSpacing = main.GetGridSpacing();
        playerHeight = main.GetPlayerHeight();
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
        // TODO remove when finished debugging
        //floor.SetActive(false);

        // Wall 1
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Wall 1";
        wall.transform.parent = transform;    
        wall.transform.localScale = new Vector3(arenaWidth + (wallWidth*2), wallWidth, wallWidth);        
        wall.transform.position = new Vector3(0, (wallWidth/2), (arenaWidth/2) + (wallWidth/2));
        AdjustPositionForPlayerHeight(wall.transform);
        wall.GetComponent<Renderer>().material.color = wallColor;

        // Wall 2
        GameObject wall2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall2.name = "Wall 2";
        wall2.transform.parent = transform;    
        wall2.transform.localScale = new Vector3(arenaWidth + (wallWidth*2), wallWidth, wallWidth);        
        wall2.transform.position = new Vector3(0, (wallWidth/2), -((arenaWidth/2) + (wallWidth/2)));
        AdjustPositionForPlayerHeight(wall2.transform);
        wall2.GetComponent<Renderer>().material.color = wallColor;

        // Wall 3
        GameObject wall3 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall3.name = "Wall 3";
        wall3.transform.parent = transform;    
        wall3.transform.localScale = new Vector3(wallWidth, wallWidth, arenaWidth + (wallWidth*2));         
        wall3.transform.position = new Vector3((arenaWidth/2) + (wallWidth/2), (wallWidth/2), 0);
        AdjustPositionForPlayerHeight(wall3.transform);
        wall3.GetComponent<Renderer>().material.color = wallColor;

        // Wall 4
        GameObject wall4 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall4.name = "Wall 4";
        wall4.transform.parent = transform;    
        wall4.transform.localScale = new Vector3(wallWidth, wallWidth, arenaWidth + (wallWidth*2));         
        wall4.transform.position = new Vector3(-((arenaWidth/2) + (wallWidth/2)), (wallWidth/2), 0);
        AdjustPositionForPlayerHeight(wall4.transform);
        wall4.GetComponent<Renderer>().material.color = wallColor;

        gridLines = new GameObject();
        gridLines.name = "Grid Lines";
        gridLines.transform.parent = transform;    
    }

    void Update()
    {
        // Draw grid lines
        var yPos = 0.4f - playerHeight/2;
        for (float i = -(arenaWidth/2) + gridSpacing; i < (arenaWidth/2); i = i + gridSpacing) {
            DrawLine(gridLines, new Vector3(i, yPos, -(arenaWidth/2)), new Vector3(i, yPos, (arenaWidth/2)), gridColor, 0.1f, 0.2f);
            DrawLine(gridLines, new Vector3(-(arenaWidth/2), yPos, i), new Vector3((arenaWidth/2), yPos, i), gridColor, 0.1f, 0.2f);
        }

        // Draw turning path
        var turningPositions = main.GetTurningPoints().GetPositions();

        for (var i = 0; i<turningPositions.Length-1; i++) {
            var pos1 = new Vector3(turningPositions[i].x, yPos, turningPositions[i].z);
            var pos2 = new Vector3(turningPositions[i+1].x, yPos, turningPositions[i+1].z);
            DrawLine(gridLines, pos1, pos2, pathColor, 1, 0.2f);
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
}
