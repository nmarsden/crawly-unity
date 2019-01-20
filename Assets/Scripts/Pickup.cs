using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { FOOD, POISON };

    static IDictionary<PickupType, Color32> pickupTypeToColors = new Dictionary<PickupType, Color32>
    {
        { PickupType.FOOD,   new Color32(18, 140, 30, 255) }, // dark green
        { PickupType.POISON, new Color32(140, 18, 30, 255) }, // dark red
    };

    public class PickupTrigger : MonoBehaviour
    {
        Main main;
        Pickup pickup;

        public void Init(Main main, Pickup pickup) 
        {
            this.main = main;
            this.pickup = pickup;
        }
        void OnTriggerEnter(Collider collider) 
        {
            if (collider.name == "Head" || collider.name.StartsWith("Tail")) {
                main.HandleHitPickup(pickup);
            }
        }

    }

    GameObject pickup;

    Main main;
    PickupType pickupType;
    Color32 color;

    float gridSpacing;
    float groundYPos;
    float fallingSpeed = 0.1f;
    float fallingDistance = 20;

    float activeDuration = 10;
    float activeStartTime;
    bool isActive;

    float inactiveDuration = 0.3f;
    float inactiveStartTime;

    bool isWaitToAppear;
    float waitToAppearDuration = 1;
    float waitToAppearStartTime;
    float waitToFlashDuration = 9;
    float waitToFlashStartTime;
    float flashDuration = 0.05f;
    float flashStartTime;

    public void Init(Main main, PickupType pickupType) 
    {
        this.main = main;
        this.pickupType = pickupType;
        color = Pickup.pickupTypeToColors[pickupType];
        gridSpacing = main.GetGridSpacing();
        groundYPos = 1 - main.GetPlayerHeight()/2; 
    }

    void Start()
    {
        pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickup.name = "Pickup";
        pickup.transform.parent = transform;    
        pickup.transform.localScale = new Vector3(2, 2, 2);      
        pickup.GetComponent<Renderer>().material = new Material(Shader.Find("Standard"));
        // pickup.GetComponent<Renderer>().material = new Material(Shader.Find("Transparent/Diffuse"));;
        pickup.GetComponent<Renderer>().material.color = color;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.AddComponent<PickupTrigger>();
        pickup.GetComponent<PickupTrigger>().Init(main, this);

        Hide();
    }

    public bool isFood() 
    {
        return pickupType.Equals(PickupType.FOOD);
    }

    public bool isPoison() 
    {
        return pickupType.Equals(PickupType.POISON);
    }

    public void Eat() {
        Hide();
    }

    public Vector3? GetPosition() {
        if (isActive) {
            return pickup.transform.position;
        } else {
            return null;
        }
    }

    void FixedUpdate()
    {
        if (isWaitToAppear) {
            if (Time.time - waitToAppearStartTime > waitToAppearDuration) {
                isWaitToAppear = false;
                Appear();
            }
        } else {
            if (isActive) {
                if (Time.time - activeStartTime > activeDuration) {
                    Disappear();
                } else {
                    // Update vertical position
                    var remainingFallDistance = pickup.transform.position.y - groundYPos;
                    if (remainingFallDistance > 0) {
                        pickup.transform.Translate(new Vector3(0, -fallingSpeed, 0));
                    }

                    // Check whether flashing is enabled
                    if (Time.time - waitToFlashStartTime > waitToFlashDuration) {
                        Flash();
                    } else {
                        flashStartTime = Time.time;
                    }
                }
            } else {
                if (Time.time - inactiveStartTime > inactiveDuration) {
                    Appear();
                }
            }
        }
    }

    void Disappear() 
    {
        main.HandlePickupDisappear(this);

        Hide();
    }

    void Appear() 
    {
        var emptyPosition = GetEmptyPosition();
        if (emptyPosition == null) {
            waitToAppearStartTime = Time.time;
            isWaitToAppear = true;
        } else {
            main.HandlePickupAppear(this);        
            Reposition((Vector3) emptyPosition);
            Show();
        }
    }

    void Flash() 
    {
        if (Time.time - flashStartTime > flashDuration) {
            pickup.GetComponent<Renderer>().enabled = !pickup.GetComponent<Renderer>().enabled;
            flashStartTime = Time.time;
        }
    }

    void Reposition(Vector3 position) 
    {
        pickup.transform.position = new Vector3(position.x, groundYPos + fallingDistance, position.z);
    }

    Vector3? GetEmptyPosition() {
        List<Vector3> emptyPositions = main.GetEmptyPositions();
        if (emptyPositions.Count == 0) {
            return null;
        }
        // Select random position from available empty position
        var position = emptyPositions[Random.Range(0, emptyPositions.Count)];
        // Return position
        return new Vector3(position.x, groundYPos + fallingDistance, position.z);
    }

    void Hide() 
    {
        inactiveStartTime = Time.time;
        isActive = false;
        pickup.SetActive(false);
    }

    void Show() 
    {
        // Reset flashing
        waitToFlashStartTime = Time.time;
        pickup.GetComponent<Renderer>().enabled = true;

        // Reset as active
        activeStartTime = Time.time;
        isActive = true;
        pickup.SetActive(true);
    }
}
