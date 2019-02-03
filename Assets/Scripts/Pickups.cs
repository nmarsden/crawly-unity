using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    Main main;

    GameObject food;
    GameObject poison;
    GameObject shield;

    public void Init(Main main) {
        this.main = main;
        food = CreateFood();
        poison = CreatePoison();
        shield = CreateShield();
    }

    public List<Vector3> GetPositions() {
        List<Vector3> positions = new List<Vector3>();
        AddNonNullPosition(positions, food.GetComponent<Pickup>());
        AddNonNullPosition(positions, poison.GetComponent<Pickup>());
        AddNonNullPosition(positions, shield.GetComponent<Pickup>());
        return positions;
    }

    void AddNonNullPosition(List<Vector3> positions, Pickup pickup) {
        var position = pickup.GetPosition();
        if (position != null) {
            positions.Add((Vector3) position);
        }
    }

    GameObject CreateFood() {
        var food = new GameObject();
        food.name = "Food";
        food.transform.parent = transform;    
        food.AddComponent<Pickup>();
        food.GetComponent<Pickup>().Init(main, Pickup.PickupType.FOOD);
        return food;
    }

    GameObject CreatePoison() {
        var poison = new GameObject();
        poison.name = "Poison";
        poison.transform.parent = transform;    
        poison.AddComponent<Pickup>();
        poison.GetComponent<Pickup>().Init(main, Pickup.PickupType.POISON);
        return poison;
    }

    GameObject CreateShield() {
        var shield = new GameObject();
        shield.name = "Shield";
        shield.transform.parent = transform;    
        shield.AddComponent<Pickup>();
        shield.GetComponent<Pickup>().Init(main, Pickup.PickupType.SHIELD);
        return shield;
    }

}
