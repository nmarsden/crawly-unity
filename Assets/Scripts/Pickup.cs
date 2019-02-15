using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { FOOD, POISON, SHIELD };

    static IDictionary<PickupType, PickupProperties> pickupTypeToProperties = new Dictionary<PickupType, PickupProperties>
    {
        { PickupType.SHIELD, new PickupProperties(new Color32(0,   35, 102, 255),    5, new Vector3(1.5f,    4, 1.5f), 150) }, 
        { PickupType.FOOD,   new PickupProperties(new Color32(18, 140,  30, 255), 0.3f, new Vector3(2.5f, 2.5f, 2.5f), 165) }, 
        { PickupType.POISON, new PickupProperties(new Color32(140, 18,  30, 255),    7, new Vector3(   4, 1.5f,    4), 300) }, 
    };

    public class PickupProperties 
    {
        public Color32 color;
        public float inactiveDuration;
        public Vector3 scale;
        public float particleEmissionRate;

        public PickupProperties(Color32 color, float inactiveDuration, Vector3 scale, float particleEmissionRate) {
            this.color = color;
            this.inactiveDuration = inactiveDuration;
            this.scale = scale;
            this.particleEmissionRate = particleEmissionRate;
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
    PickupProperties pickupProperties;

    float gridSpacing;
    float groundYPos;
    float fallingSpeed = 0.1f;
    float fallingDistance = 20;

    float activeDuration = 10;
    float activeStartTime;
    bool isActive;

    float inactiveStartTime;

    bool isWaitToAppear;
    float waitToAppearDuration = 1;
    float waitToAppearStartTime;
    float waitToFlashDuration = 9;

    public void Init(Main main, PickupType pickupType) 
    {
        this.main = main;
        this.pickupType = pickupType;
        this.pickupProperties = Pickup.pickupTypeToProperties[pickupType];
        gridSpacing = main.GetGridSpacing();
        groundYPos = (pickupProperties.scale.y/2) - main.GetPlayerHeight()/2; 
    }

    void Start()
    {
        var material = new Material(Shader.Find("Standard"));
        material.color = pickupProperties.color;

        // Give pickup material an edge emission texture
        var emissionTexture =  Resources.Load<Texture>("Image/Edge_Emission");
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", pickupProperties.color);
        material.SetTexture("_EmissionMap", emissionTexture);

        pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickup.name = "Pickup";
        pickup.transform.parent = transform;    
        pickup.transform.localScale = pickupProperties.scale;
        pickup.GetComponent<Renderer>().material = material;
        pickup.GetComponent<Collider>().isTrigger = true;
        pickup.AddComponent<PickupTrigger>();
        pickup.GetComponent<PickupTrigger>().Init(main, this);
        pickup.AddComponent<Flash>();

        AddParticleSystem(pickup, material);

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
                if (Time.time - inactiveStartTime > pickupProperties.inactiveDuration) {
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

    void AddParticleSystem(GameObject pickup, Material material) {

        // Add ParticleSystem
        pickup.AddComponent<ParticleSystem>();
        var ps = pickup.GetComponent<ParticleSystem>();

        // -- ColorOverLifetimeModule --
        var colorOverLifetime = ps.colorOverLifetime;

        var colorKeys = new GradientColorKey[] {
            new GradientColorKey(pickupProperties.color, 0),
            new GradientColorKey(pickupProperties.color, 0.3f),
            new GradientColorKey(Color.white, 1)
        };
        var alphaKeys = new GradientAlphaKey[] {
            new GradientAlphaKey(0, 0),
            new GradientAlphaKey(1, 0.3f),
            new GradientAlphaKey(0, 1),
        };
        var gradient = new Gradient();
        gradient.SetKeys(colorKeys, alphaKeys);

        colorOverLifetime.enabled = true;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // -- MainModule --
        var m = ps.main;
        var c = pickupProperties.color;
        m.startSize3D = true;
        m.startSizeXMultiplier = 0.4f / pickupProperties.scale.x;
        m.startSizeYMultiplier = 0.4f / pickupProperties.scale.y;
        m.startSizeZMultiplier = 0.4f / pickupProperties.scale.z;
        m.startLifetime = .3f;
        m.startSpeed = 20f / pickupProperties.scale.y;

        // -- TrailModule --
        // var trails = ps.trails;
        // trails.enabled = true;
        // trails.dieWithParticles = true;
        // trails.mode = ParticleSystemTrailMode.PerParticle;

        // -- ParticleSystemRenderer --
        var psr = ps.GetComponent<ParticleSystemRenderer>();

        // Create cube mesh
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material = material;
        MeshFilter cubeMeshFilter = cube.GetComponent<MeshFilter>();
        var cubeMesh = cubeMeshFilter.mesh;
        GameObject.Destroy(cube);

        // Get default sprite material
        var psMaterial = new Material(Shader.Find("Sprites/Default"));;

        psr.material = psMaterial; 
        psr.trailMaterial = psMaterial;
        psr.renderMode = ParticleSystemRenderMode.Mesh;
        psr.mesh = cubeMesh;

        // -- ShapeModule --
        var shape = ps.shape;

        // Create quad mesh shape
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.parent = pickup.transform;
        quad.GetComponent<Renderer>().material = material;
        MeshFilter viewedModelFilter = quad.GetComponent<MeshFilter>();
        var quadMesh = viewedModelFilter.mesh;
        GameObject.Destroy(quad);

        shape.scale = new Vector3(1, 1, 1);
        shape.alignToDirection = true;
        shape.rotation = new Vector3(90, 0, 0);
        shape.shapeType = ParticleSystemShapeType.Mesh;
        shape.mesh = quadMesh;
        shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        shape.meshSpawnMode = ParticleSystemShapeMultiModeValue.Loop;
        shape.normalOffset = 0.3f;

        // Using texture for clipping
        shape.texture = Resources.Load<Sprite>("Image/Pickup_Particle").texture;
        shape.textureClipChannel = ParticleSystemShapeTextureChannel.Alpha;

        // -- EmissionModule --
        var emission = ps.emission;
        emission.rateOverDistance = 0;
        emission.rateOverTime = pickupProperties.particleEmissionRate; 
    }
}
