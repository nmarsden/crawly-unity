using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    int currentLevelNum;

    string[][] levelMaps = {
        new string[] {
            "#######",
            "#___###",
            "#___###",
            "#___###",
            "#######",
            "#######",
            "#######",
        },
        new string[] {
            "#####",
            "#___#",
            "#___#",
            "#___#",
            "#####",
        },
        new string[] {
            "#___#",
            "#___#",
            "#####",
            "#___#",
            "#___#",
        }
    };

    public void Init(int currentLevelNum) {
        this.currentLevelNum = currentLevelNum;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public float GetLevelWidthInCells() {
        return GetCurrentLevelMap()[0].Length;
    }

    public Arena.CellType[,] GetLevelCellTypes() {
        var levelMap = GetCurrentLevelMap();
        Arena.CellType[,] cellTypes = new Arena.CellType[levelMap.Length, levelMap.Length];
        for (var row = 0; row < levelMap.Length; row++) {
            for (var col = 0; col < levelMap.Length; col++) {
                cellTypes[row, col] = (levelMap[row][col] == '#') ? Arena.CellType.BASIC : Arena.CellType.ACTIVATABLE;
            }
        }
        return cellTypes;
    }

    public float GetNumberOfCells() {
        return GetLevelWidthInCells() * GetLevelWidthInCells();
    }
    
    string[] GetCurrentLevelMap() {
        return levelMaps[currentLevelNum - 1];
    }

}
