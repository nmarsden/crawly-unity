using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    Material mat;
    float startTime;
    float duration = 1;

    private void Start() 
    {
        mat = Resources.Load<Material>("Material/DissolveEmissionMaterial");

        GetComponent<Renderer>().material = mat;
        startTime = Time.time;
    }

    private void Update() {
        var dissolveAmount = Mathf.Lerp(0, 1, (Time.time - startTime) / duration);

        mat.SetFloat("_DissolveAmount", dissolveAmount);
    }
}
