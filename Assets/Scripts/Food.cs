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

    Color foodColor = new Color32(18, 140, 30, 255); // dark green
    Color decayColor = new Color32(0, 0, 0, 255); // black

    GameObject food;

    Main main;

    float gridSpacing;
    float yPos;
    float activeDuration = 15;
    float activeStartTime;
    bool isActive;

    float inactiveDuration = 5;
    float inactiveStartTime;

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
        food.GetComponent<Renderer>().material.color = foodColor;
        food.GetComponent<Collider>().isTrigger = true;
        food.AddComponent<FoodTrigger>();
        food.GetComponent<FoodTrigger>().Init(main);

        Hide();
    }

    public void Eat() {
        Hide();
    }

    void FixedUpdate()
    {
        if (isActive) {
            if (Time.time - activeStartTime > activeDuration) {
                Hide();
            } else {
                Decay();
            }
        } else {
            if (Time.time - inactiveStartTime > inactiveDuration) {
                Respawn();
            }
        }
    }

    void Respawn() 
    {
        Reposition();
        Show();
    }

    void Decay() {
        // Over time change the food color to the decay color
        food.GetComponent<Renderer>().material.color = Color32.Lerp(foodColor, decayColor, (Time.time - activeStartTime) / activeDuration);
    }

    void Reposition() {
        List<Vector3> emptyPositions = main.GetEmptyPositions();
        var position = emptyPositions[Random.Range(0, emptyPositions.Count)];

        food.transform.position = new Vector3(position.x, yPos, position.z);
    }

    void Hide() 
    {
        inactiveStartTime = Time.time;
        isActive = false;
        food.SetActive(false);
    }

    void Show() 
    {
        activeStartTime = Time.time;
        isActive = true;
        food.SetActive(true);
    }
}
