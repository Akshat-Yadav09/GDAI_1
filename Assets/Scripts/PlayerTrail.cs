using UnityEngine;

/// <summary>
/// Attaches to the Player. Creates a particle trail that moves LEFT,
/// giving the illusion the cube is rushing forward (like Geometry Dash).
/// Emits more particles while airborne for a jump trail effect.
/// </summary>
public class PlayerTrail : MonoBehaviour
{
    [Header("Trail Particle System")]
    [Tooltip("Drag the Trail ParticleSystem child here. If left empty, one will be created at runtime.")]
    public ParticleSystem trailParticles;

    [Header("Emission")]
    [Tooltip("Particles per second while grounded")]
    public float groundEmissionRate = 30f;
    [Tooltip("Particles per second while airborne")]
    public float airEmissionRate = 60f;

    [Header("Visuals")]
    public Color trailColor = new Color(0f, 1f, 1f, 0.8f);  // Cyan glow
    public float particleSpeed = 8f;      // How fast particles move left
    public float particleSize = 0.15f;
    public float particleLifetime = 0.3f;

    private bool isGrounded = true;

    void Start()
    {
        if (trailParticles == null)
            CreateTrailParticleSystem();

        if (trailParticles != null)
        {
            var em = trailParticles.emission;
            em.rateOverTime = groundEmissionRate;
        }
    }


    /// <summary>
    /// Call this from PlayerController when grounded state changes.
    /// </summary>
    public void SetGrounded(bool grounded)
    {
        if (isGrounded == grounded) return; // Skip if no change
        isGrounded = grounded;
        if (trailParticles != null)
        {
            var em = trailParticles.emission;
            em.rateOverTime = isGrounded ? groundEmissionRate : airEmissionRate;
        }
    }

    /// <summary>
    /// Call this on death — stops emission and clears all lingering particles immediately.
    /// </summary>
    public void ClearOnDeath()
    {
        if (trailParticles == null) return;
        var em = trailParticles.emission;
        em.rateOverTime = 0f;
        trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /// <summary>
    /// Creates a fully configured trail particle system as a child object.
    /// </summary>
    private void CreateTrailParticleSystem()
    {
        GameObject trailObj = new GameObject("TrailParticles");
        trailObj.transform.SetParent(transform, false);
        // Offset slightly behind and below the cube center
        trailObj.transform.localPosition = new Vector3(-0.5f, -0.2f, 0f);

        trailParticles = trailObj.AddComponent<ParticleSystem>();

        // Stop the default auto-play so we can configure first
        trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Main module
        var main = trailParticles.main;
        main.startLifetime = particleLifetime;
        main.startSpeed = particleSpeed;
        main.startSize = particleSize;
        main.startColor = trailColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World; // Trail stays in world space
        main.maxParticles = 200;
        main.gravityModifier = 0f;

        // Shape — emit from a small edge behind the player, shooting LEFT
        var shape = trailParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.scale = new Vector3(0.05f, 0.4f, 1f);
        shape.rotation = new Vector3(0f, -90f, 0f);
        shape.randomDirectionAmount = 0.1f; // Slight spread for natural look

        // Size over lifetime — shrink as they fade
        var sizeOverLifetime = trailParticles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Color over lifetime — fade out alpha
        var colorOverLifetime = trailParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(trailColor, 0f), new GradientColorKey(trailColor, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        // Emission
        var emissionModule = trailParticles.emission;
        emissionModule.rateOverTime = groundEmissionRate;

        // Renderer — use player's sprite material to ensure it exists in the build
        var renderer = trailParticles.GetComponent<ParticleSystemRenderer>();
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null && sr.sharedMaterial != null)
        {
            renderer.material = sr.sharedMaterial;
        }
        else
        {
            Shader sprShader = Shader.Find("Sprites/Default");
            if (sprShader != null) renderer.material = new Material(sprShader);
        }

        // Start playing
        trailParticles.Play();
    }
}
