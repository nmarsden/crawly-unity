using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
    int currentLevelWidthInCells;

    Arena.CellType[,] currentLevelCellTypes;
    int numberOfFillableCells;

    string[][] levelMaps = {
        new string[] { // Level 1: Food 0
            "#####",
            "#####",
            "#_#_#",
            "#___#",
            "#####",
        },
        new string[] { // Level 2: Food 2
            "#####",
            "##__#",
            "#___#",
            "#___#",
            "#####",
        },
        new string[] { // Level 3: Food 5
            "#_###",
            "#___#",
            "#___#",
            "#___#",
            "#####",
        },
        new string[] { // Level 4: Food 10
            "#___#",
            "#___#",
            "#___#",
            "#___#",
            "#####",
        },
        new string[] { // Level 5: Food 13
            "_____",
            "_X#X_",
            "_X#X_",
            "_X#X_",
            "_###_",
        },
        new string[] { // Level 6: Food 16
            "_____",
            "_X#X_",
            "_X#X_",
            "_X_X_",
            "##___",
        },
        new string[] { // Level 7: Food 19
            "_____",
            "_X#X_",
            "_#_#_",
            "_X_X_",
            "##___",
        },
        new string[] { // Level 8: Food 23
            "T____",
            "#X#X_",
            "##T#_",
            "#X_X_",
            "##___",
        },
        new string[] { // Level 9: Food 26
            "T###T",
            "#X#X#",
            "T#T#T",
            "#X#X#",
            "##T#T",
        },
        new string[] { // Level 10: Food 35
            "T#X#T",
            "#####",
            "#XTX#",
            "#####",
            "T#X#T",
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

    public int GetNumberOfLevels() {
        return levelMaps.Length;
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
