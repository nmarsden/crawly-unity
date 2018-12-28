using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    Color foodColor = new Color32(200, 0, 0, 255);

    GameObject food;

    float gridSpacing;
    float yPos;

    public void Init(Main main) 
    {
        gridSpacing = main.GetGridSpacing();
        yPos = 1 - main.GetPlayerHeight()/2;
    }

    void Start()
    {
        food = GameObject.CreatePrimitive(PrimitiveType.Cube);
        food.name = "Food";
        food.transform.parent = transform;    
        food.transform.localScale = new Vector3(2, 2, 2);        
        food.transform.position = new Vector3(0, yPos, gridSpacing * 3);
        food.GetComponent<Renderer>().material.color = foodColor;
    }

    void Update()
    {
        
    }

    public void Reposition() {
        // TODO ensure food is not moved to where the player is positioned
        var foodX = gridSpacing * Random.Range(-4, 5);
        var foodZ = gridSpacing * Random.Range(-4, 5);
        food.transform.position = new Vector3(foodX, yPos, foodZ);
    }

}
