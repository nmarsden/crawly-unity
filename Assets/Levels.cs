using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    int currentLevelWidthInCells;

    Arena.CellType[,] currentLevelCellTypes;
    int numberOfFillableCells;

    string[][] levelMaps = {
        new string[] {
            "X####",
            "#___#",
            "#___#",
            "#T__#",
            "####X",
        },
        new string[] {
            "#___#",
            "#___#",
            "#####",
            "#___#",
            "#___#",
        },
        new string[] {
            "#######",
            "#___###",
            "#___###",
            "#___###",
            "XXXX###",
            "#######",
            "#######",
        },
    };

    public void Init(int currentLevelNum) {
        var currentLevelMap = GetLevelMap(currentLevelNum);
        currentLevelWidthInCells = currentLevelMap[0].Length;
        currentLevelCellTypes = GetLevelCellTypes(currentLevelMap);
        numberOfFillableCells = CountNumberOfFillableCells(currentLevelMap);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public float GetLevelWidthInCells() {
        return currentLevelWidthInCells;
    }

    public Arena.CellType[,] GetLevelCellTypes() {
        return currentLevelCellTypes;
    }

    public int GetNumberOfFillableCells() {
        return numberOfFillableCells;
    }
    
    string[] GetLevelMap(int currentLevelNum) {
        return levelMaps[currentLevelNum - 1];
    }

    Arena.CellType[,] GetLevelCellTypes(string[] levelMap) {
        Arena.CellType[,] cellTypes = new Arena.CellType[levelMap.Length, levelMap.Length];
        for (var row = 0; row < levelMap.Length; row++) {
            for (var col = 0; col < levelMap.Length; col++) {
                cellTypes[row, col] = GetCellType(levelMap[row][col]);
            }
        }
        return cellTypes;
    }

    Arena.CellType GetCellType(char cellChar) {
        if (cellChar == 'X') {
            return Arena.CellType.WALL;
        } else if (cellChar == '_') {
            return Arena.CellType.ACTIVATABLE;
        } else if (cellChar == 'T') {
            return Arena.CellType.TOUCH;
        } else { // '#'
            return Arena.CellType.BASIC;
        }
    }

    int CountNumberOfFillableCells(string[] levelMap) {
        var count=0;
        var cellTypes = GetLevelCellTypes(levelMap);
        for (var row = 0; row < levelMap.Length; row++) {
            for (var col = 0; col < levelMap.Length; col++) {
                if (!GetCellType(levelMap[row][col]).Equals(Arena.CellType.WALL)) {
                    count++;
                }
            }
        }
        return count;
    }

}
