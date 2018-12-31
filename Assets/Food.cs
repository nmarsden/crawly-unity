using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public class FoodTrigger : MonoBehaviour
    {
        Main main;

        public void Init(Main main) 
        {
            this.main = main;
        }
        void OnTriggerEnter(Collider collider) 
        {
            main.HandleHitFood();
        }

    }

    Color foodColor = new Color32(200, 0, 0, 255);

    GameObject food;

    Main main;

    float gridSpacing;
    float yPos;

    public void Init(Main main) 
    {
        this.main = main;
        gridSpacing = main.GetGridSpacing();
        yPos = 1 - main.GetPlayerHeight()/2;
    }

    void Start()
    {
        food = GameObject.CreatePrimitive(PrimitiveType.Cube);
        food.name = "Food";
        food.transform.parent = transform;    
        food.transform.localScale = new Vector3(2, 2, 2);        
        food.transform.position = new Vector3(gridSpacing * 3, yPos, gridSpacing * 3);
        food.GetComponent<Renderer>().material.color = foodColor;
        food.GetComponent<Collider>().isTrigger = true;
        food.AddComponent<FoodTrigger>();
        food.GetComponent<FoodTrigger>().Init(main);
    }

    void Update()
    {
        
    }

    public void Reposition(float xPos, float zPos) {
        food.transform.position = new Vector3(xPos, yPos, zPos);
    }

}
