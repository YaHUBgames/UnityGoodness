using UnityEngine;
using System.Collections.Generic;

public class WaterController : MonoBehaviour
{
    [SerializeField] private Material waterMaterial;
    private int waveTimerPropertyID = 0;

    [SerializeField] private float waterVertexWaveSpeed = 0.1f;
    [SerializeField] private bool simulateWaves = true;

    [SerializeField] private Transform splashParticle;
    [SerializeField] private Transform splashTrail;
    [SerializeField] private float splashY = 0f;

    private List<WaterBehaviour> waterBehaviours = new List<WaterBehaviour>();

    private float waveTime = 0f;
    private float waveChoppiness = 0f;

    private void Start()
    {
        if (waterMaterial == null)
            Debug.LogWarning("No Water Material Assigned");
        waveChoppiness = waterMaterial.GetFloat("_Choppiness");

        waveTimerPropertyID = Shader.PropertyToID("_WavesTime");
    }

    private void Update()
    {
        if (!simulateWaves)
            return;
        waveTime += Time.deltaTime * waterVertexWaveSpeed;
        waterMaterial.SetFloat(waveTimerPropertyID, waveTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        WaterBehaviour newWB = other.GetComponent<WaterBehaviour>();
        if (newWB == null)
            return;
        waterBehaviours.Add(newWB);
        Vector3 pos = newWB.GetObjectPosition();
        pos.y = splashY;
        newWB.SetupDistanceTraveled();
        Instantiate(splashParticle, pos , Quaternion.identity, transform)
            .localScale = newWB.GetObjectsSplashSize()  * Vector3.one;
    }

    private void OnTriggerExit(Collider other)
    {
        WaterBehaviour newWB = other.GetComponent<WaterBehaviour>();
        if (newWB == null)
            return;
        waterBehaviours.Remove(newWB);
    }

    private void FixedUpdate() 
    {
        if (!simulateWaves)
            return;
        foreach (WaterBehaviour WB in waterBehaviours)
        {
            Vector3 pos = WB.GetObjectPosition();
            WB.MoveObject(GetWorldHeightAtPosition(pos));
            if(!WB.CheckDistanceTraveled())
                continue;
            pos.y = splashY;
            Instantiate(splashTrail, pos , Quaternion.identity, transform)
                .localScale = WB.GetObjectsSplashSize()  * Vector3.one;
        }
    }

    public float GetWorldHeightAtPosition(Vector3 position)
    {
        //Calculation like in shader - ToonWaterVertexWaves
        float XZoffset = position.x + position.z;
        float sinAtPosition = Mathf.Sin(waveTime + XZoffset) * waveChoppiness;

        // This expects the water material is on flat plane with constant Y position.
        return sinAtPosition + transform.position.y;
    }
}
