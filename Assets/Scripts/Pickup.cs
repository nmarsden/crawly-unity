using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { FOOD, POISON, SHIELD };

    static IDictionary<PickupType, PickupProperties> pickupTypeToProperties = new Dictionary<PickupType, PickupProperties>
    {
        { PickupType.FOOD,   new PickupProperties(new Color32(18, 140,  30, 255), 0.3f) }, // dark green
        { PickupType.SHIELD, new PickupProperties(new Color32(0,   35, 102, 255),    5) }, // dark blue
        { PickupType.POISON, new PickupProperties(new Color32(140, 18,  30, 255),    7) }, // dark red
    };

    public class PickupProperties 
    {
        public Color32 color;
        public float inactiveDuration;

        public PickupProperties(Color32 color, float inactiveDuration) {
            this.color = color;
            this.inactiveDuration = inactiveDuration;
        }
    }

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

    float inactiveDuration;
    float inactiveStartTime;

    bool isWaitToAppear;
    float waitToAppearDuration = 1;
    float waitToAppearStartTime;
    float waitToFlashDuration = 9;

    public void Init(Main main, PickupType pickupType) 
    {
        this.main = main;
        this.pickupType = pickupType;
        var pickupProperties = Pickup.pickupTypeToProperties[pickupType];
        color = pickupProperties.color;
        inactiveDuration = pickupProperties.inactiveDuration;
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
        pickup.AddComponent<Flash>();

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

    public bool isShield() 
    {
        return pickupType.Equals(PickupType.SHIELD);
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
        // Turn off flashing
        pickup.GetComponent<Flash>().TurnOff();

        inactiveStartTime = Time.time;
        isActive = false;
        pickup.SetActive(false);
    }

    void Show() 
    {
        // Turn on flashing (after delay)
        pickup.GetComponent<Flash>().TurnOn(waitToFlashDuration);

        // Reset as active
        activeStartTime = Time.time;
        isActive = true;
        pickup.SetActive(true);
    }
}
