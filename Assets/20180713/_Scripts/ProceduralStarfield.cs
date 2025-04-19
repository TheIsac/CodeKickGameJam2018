using UnityEngine;

public class ProceduralStarfield : MonoBehaviour
{
    public int numberOfStars = 500;
    public float sphereRadius = 100f; // Radius of the sphere the stars will be on
    public float minStarSize = 0.05f;
    public float maxStarSize = 0.1f;
    public Color starColor = Color.white;
    public float rotationSpeed = 0.1f; // Speed of the starfield rotation

    private Transform starfieldTransform;

    void Start()
    {
        CreateProceduralStarfield();
    }

    void CreateProceduralStarfield()
    {
        // Create a new GameObject to hold the Particle System
        GameObject starfieldObject = new GameObject("StarfieldBackground");
        starfieldTransform = starfieldObject.transform;
        starfieldTransform.SetParent(transform); // Make it a child of the camera
        starfieldTransform.localPosition = Vector3.zero; // Position it at the camera

        ParticleSystem particleSystem = starfieldObject.AddComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();

        // Configure the Renderer
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        // Create a simple white material if one doesn't exist
        Material starMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        starMaterial.color = starColor;
        renderer.material = starMaterial;

        // Get the Particle System's main module and set basic properties
        var mainModule = particleSystem.main;
        mainModule.loop = true; // Loop the emission (though the stars live forever)
        mainModule.startLifetime = 1000f;
        mainModule.startSpeed = 0f;
        mainModule.startSize3D = false;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(minStarSize, maxStarSize);
        mainModule.startColor = starColor;
        mainModule.maxParticles = numberOfStars;

        // Get the Emission module and set the burst
        var emissionModule = particleSystem.emission;
        emissionModule.rateOverTime = 0f; // Emit all at once at the start
        emissionModule.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, numberOfStars) });

        // Get the Shape module and configure the spawn on a sphere surface
        var shapeModule = particleSystem.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Sphere;
        shapeModule.radius = sphereRadius;
        shapeModule.randomDirectionAmount = 0f; // We want them on the surface
        shapeModule.position = Vector3.zero; // Center the sphere around the particle system's origin
        shapeModule.radiusMode = ParticleSystemShapeMultiModeValue.Random; // Randomly distribute particles on the sphere surface

        // Get the Velocity over Lifetime module for subtle movement
        var velocityModule = particleSystem.velocityOverLifetime;
        velocityModule.enabled = true;
        velocityModule.space = ParticleSystemSimulationSpace.World; // Make movement independent of camera rotation
        // Apply a very small, constant velocity in random directions
        velocityModule.x = new ParticleSystem.MinMaxCurve(0f, 0.01f);
        velocityModule.y = new ParticleSystem.MinMaxCurve(0f, 0.01f);
        velocityModule.z = new ParticleSystem.MinMaxCurve(0f, 0.01f);

        // Play the Particle System
        particleSystem.Play();
    }

    void Update()
    {
        // Optionally rotate the starfield for a subtle sense of movement
        if (starfieldTransform != null && rotationSpeed > 0f)
        {
            starfieldTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}