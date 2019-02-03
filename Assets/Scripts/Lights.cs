using UnityEngine;
using UnityEngine.Rendering;

public class Lights : MonoBehaviour
{

    public void Start() {
        CreateDirectionalLight(new Vector3(0, 100, 0));
    }

    void CreateDirectionalLight(Vector3 position) {
        var spotLight = new GameObject();
        spotLight.name = "Directional Light";
        spotLight.transform.parent = transform;    

        var light = spotLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.position = position;
        light.transform.rotation = Quaternion.Euler(90, 0, 0);
        light.color = new Color32(248, 248, 248, 255);
        light.intensity = 1;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = 1;
        light.shadowResolution = LightShadowResolution.FromQualitySettings;
        light.shadowBias = 0.05f;
        light.shadowNormalBias = 0.4f;
        light.shadowNearPlane = 0.2f;
    }
}
