using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    Main main;

    public void Init(Main main) 
    {
        this.main = main;
    }

    void Start()
    {
        
    }

    // void OnCollisionEnter(Collision collision) 
    // {
    //     Debug.Log("collided with:" + collision.gameObject.name);

    //     if (collision.gameObject.name == "Food") 
    //     {
    //         main.HandleHitFood();
    //         return;
    //     }
    //     // if (collision.gameObject.name == "Waypoint") 
    //     // {
    //     //     main.HandleHitWaypoint();
    //     //     return;
    //     // }
    // }

    // Update is called once per frame
    void Update()
    {
        
    }
}
