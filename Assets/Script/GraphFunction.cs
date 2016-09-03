using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GraphFunction : MonoBehaviour {

    public bool IsEnabled = true;

    public enum FunctionOption { Linear, Exponential, Parabolic, Sine }
    public FunctionOption Function;

    delegate float FunctionDelegate(float x);
    static FunctionDelegate[] functions = { Linear, Exponential, Parabolic, Sine };

    [RangeAttribute(10, 100)]
    public int Resolution = 10;
    int currentResolution;

    ParticleSystem pSystem;
    ParticleSystem.Particle[] points;

    void Start () {
        CreatePoints();
    }

    void Update () {
        if (IsEnabled) {
            PlotGraph ();
        } else {
            DisableGraph();
        }
    }

    void CreatePoints () {

        pSystem = GetComponent<ParticleSystem> ();

        if (Resolution < 10 || Resolution > 100) {
            Debug.LogWarning("[GraphFunction] Resolution is out of bounds, setting to default value: 10");
            Resolution = 10;
        }

        if (pSystem.emission.enabled == false){
            EnableGraph();
        }

        points = new ParticleSystem.Particle[Resolution];

        float increment = 1f / (Resolution - 1);
        for (int i = 0; i < Resolution; i++) {
            float x = i * increment;
            points[i].position   = new Vector3(x, 0f, 0f);
            points[i].startColor = new Color(x, 0f, 0f);
            points[i].startSize  = 0.1f;
        }
    }

    void PlotGraph () {

        if (currentResolution != Resolution || points == null) {
            CreatePoints();
        }

        FunctionDelegate f = functions[(int)Function];

        for (int i = 0; i < Resolution; i++) {
            Vector3 p = points[i].position;
            Color c = points[i].startColor;

            p.y = f(p.x);
            c.g = p.y;

            points[i].position = p;
            points[i].startColor = c;
        }

        pSystem.SetParticles (points, points.Length);
    }

    void EnableGraph () {
        ParticleSystem.EmissionModule em = pSystem.emission;
        pSystem.loop = false;
        em.enabled = true;
    }

    void DisableGraph () {
        ParticleSystem.EmissionModule em = pSystem.emission;
        pSystem.loop = true;
        em.enabled = false;
    }

    static float Linear (float x) {
        return x;
    }

    static float Exponential (float x) {
        return x * x;
    }

    static float Parabolic (float x) {
        x = 2f * x - 1f;
        return x * x;
    }

    static float Sine (float x) {
        return 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * x + Time.timeSinceLevelLoad);
    }
}
