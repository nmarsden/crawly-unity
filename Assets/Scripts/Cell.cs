using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    Color activeColor = new Color32(74, 255, 0, 255); // lighter green

    bool isActivated;
    float activatedTime;
    Main main;
    Arena.CellType cellType;

    static IDictionary<Arena.CellType, Material> cellMaterials;
    ParticleSystem particleSystem;

    public void Init(Main main, Arena.CellType cellType) {
        this.main = main;
        this.cellType = cellType;

        var activatableMaterial = Resources.Load<Material>("Material/Cell_Activatable_Material");
        var basicMaterial = Resources.Load<Material>("Material/Cell_Basic_Material");
        var touchMaterial = Resources.Load<Material>("Material/Cell_Touch_Material");
        var wallMaterial = Resources.Load<Material>("Material/Cell_Wall_Material");

        cellMaterials = new Dictionary<Arena.CellType, Material>
        {
            { Arena.CellType.ACTIVATABLE,   activatableMaterial }, 
            { Arena.CellType.BASIC,         basicMaterial }, 
            { Arena.CellType.TOUCH,         touchMaterial }, 
            { Arena.CellType.WALL,          wallMaterial }, 
        };
    }

    void Start()
    {
        isActivated = cellType.Equals(Arena.CellType.BASIC);    
        gameObject.GetComponent<Renderer>().material = cellMaterials[cellType];

        if (cellType.Equals(Arena.CellType.ACTIVATABLE) || cellType.Equals(Arena.CellType.TOUCH))
        {
            particleSystem = AddParticleSystem(gameObject, gameObject.GetComponent<Renderer>().material);
            particleSystem.Stop();
        }

        if (cellType.Equals(Arena.CellType.WALL)) {
            // -- Wall Cell --
            // Adjust height & disable trigger
            var wallHeight = main.GetWallHeight();
            gameObject.transform.localScale = Vector3.Scale(gameObject.transform.localScale, new Vector3(1, wallHeight, 1));
            gameObject.transform.position += new Vector3(0, wallHeight/2 + 0.5f, 0);
            gameObject.GetComponent<Collider>().isTrigger = false;
        }
    }

    void Update()
    {
        if (isActivated)
        {
            if (cellType.Equals(Arena.CellType.ACTIVATABLE) || cellType.Equals(Arena.CellType.TOUCH))
            {
                var activatedDuration = cellType.Equals(Arena.CellType.TOUCH) ? 0.1f : 3;
                if (Time.time - activatedTime > activatedDuration) {
                    isActivated = false;
                    // Return cell to original (pre-active) material 
                    gameObject.GetComponent<Renderer>().material = cellMaterials[cellType];
                    particleSystem.Stop();
                }
            }
            else if (cellType.Equals(Arena.CellType.WALL)) 
            {
                isActivated = false;
                main.HandleHitWall();
            }
        }
    }

    public void TriggerEnter() {
        main.HandleHeadEnteredCell(gameObject.transform.position);
    }

    public void Activate() {
        activatedTime = Time.time;

        if (!isActivated) {
            isActivated = true;
           
            if (cellType.Equals(Arena.CellType.ACTIVATABLE) || cellType.Equals(Arena.CellType.TOUCH)) {
                // Indicate cell is active: Set material to basic with active color
                gameObject.GetComponent<Renderer>().material = cellMaterials[Arena.CellType.BASIC];
                gameObject.GetComponent<Renderer>().material.color = activeColor;
                particleSystem.Play();

                main.HandleCellActivated();
            }
        }
    }

    public bool IsActivated() {
        return (IsTouch() || IsActivatable()) && isActivated;
    }

    public bool IsWall() {
        return cellType.Equals(Arena.CellType.WALL);
    }

    private bool IsTouch() {
        return cellType.Equals(Arena.CellType.TOUCH);
    }

    private bool IsActivatable() {
        return cellType.Equals(Arena.CellType.ACTIVATABLE);
    }

    ParticleSystem AddParticleSystem(GameObject pickup, Material material) {

        var color = Color.yellow;
        var scale = new Vector3(5, 5, 5);
        var particleEmissionRate = 200;

        // Add ParticleSystem
        pickup.AddComponent<ParticleSystem>();
        var ps = pickup.GetComponent<ParticleSystem>();

        // -- ColorOverLifetimeModule --
        var colorOverLifetime = ps.colorOverLifetime;

        var colorKeys = new GradientColorKey[] {
            new GradientColorKey(color, 0),
            new GradientColorKey(color, 0.3f),
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
        var c = color;
        m.startSize3D = true;
        m.startSizeXMultiplier = 0.4f / scale.x;
        m.startSizeYMultiplier = 0.4f / scale.y;
        m.startSizeZMultiplier = 0.4f / scale.z;
        m.startLifetime = .3f;
        m.startSpeed = 20f / scale.y;

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

        // -- EmissionModule --
        var emission = ps.emission;
        emission.rateOverDistance = 0;
        emission.rateOverTime = particleEmissionRate; 

        return ps;
    }

}
