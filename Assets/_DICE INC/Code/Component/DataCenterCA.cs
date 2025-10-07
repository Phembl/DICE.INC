using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DataCenterCA : MonoBehaviour
{
    [Header("Display / Grid")]
    [SerializeField] private RawImage display;       // auto-filled in Awake if on same GO
    [SerializeField] private int gridWidth = 48;
    [SerializeField] private int gridHeight = 128;
    [SerializeField] private Color32 backgroundColor = new Color32(0, 0, 0, 255);
    [SerializeField] private Color32 trailColor = new Color32(0, 255, 255, 255);

    [Header("Agents")]
    [SerializeField] private int agentCount = 24;
    [SerializeField] private float speedCellsPerSecond = 12f;
    [Tooltip("Random spread in degrees around 'down' for initial directions.")]
    [SerializeField] private float initialAngleSpreadDeg = 35f;
    [Tooltip("Small randomness added on trail bounce to avoid ping-ponging.")]
    [SerializeField] private float bounceNoiseDeg = 10f;

    [Header("Lifecycle")]
    [Tooltip("Wipes the entire board every X seconds.")]
    [SerializeField] private float wipeIntervalSeconds = 3.5f;

    [Header("Timing")]
    [SerializeField] private bool useFixedUpdate = true; // recommended
    [SerializeField] private int maxCellsPerSegment = 256; // safety cap per agent step

    // Optional event: subscribe if you want to count 'Data' on trail hits.
    public event Action OnTrailHit;

    // Internal buffers
    private Texture2D _tex;
    private Color32[] _pixels;  // size = gridWidth * gridHeight
    private byte[] _occ;        // 0 = empty, 1 = occupied

    private struct Agent
    {
        public Vector2 pos; // in cell space, continuous
        public Vector2 vel; // normalized direction
    }
    private readonly List<Agent> _agents = new List<Agent>(64);

    private float _wipeTimer = 0f;
    private System.Random _rng; // deterministic if you want (seed it)

    private void Awake()
    {
        if (display == null) display = GetComponent<RawImage>();
        _rng = new System.Random(); // set a seed if you want reproducibility
        InitTextureAndGrid();
        SpawnAgents();
    }

    private void OnEnable()
    {
        // Ensure crisp pixel look when RawImage scales us up
        if (_tex != null)
        {
            _tex.filterMode = FilterMode.Point;
            _tex.wrapMode = TextureWrapMode.Clamp;
        }
    }

    private void InitTextureAndGrid()
    {
        _tex = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        _tex.wrapMode = TextureWrapMode.Clamp;

        _pixels = new Color32[gridWidth * gridHeight];
        _occ = new byte[gridWidth * gridHeight];

        FillAll(backgroundColor);
        _tex.SetPixels32(_pixels);
        _tex.Apply(false, false);

        display.texture = _tex;
    }

    private void FillAll(Color32 c)
    {
        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = c;
            _occ[i] = 0;
        }
    }

    private void SpawnAgents()
    {
        _agents.Clear();

        for (int i = 0; i < agentCount; i++)
        {
            Agent a = new Agent();

            // Spawn along the top row (just inside the wall)
            a.pos.x = UnityEngine.Random.Range(1f, gridWidth - 1f);
            a.pos.y = gridHeight - 2f;

            // Aim roughly downward with some spread
            float angle = 270f + UnityEngine.Random.Range(-initialAngleSpreadDeg, initialAngleSpreadDeg);
            a.vel = AngleToDir(angle);

            _agents.Add(a);
        }
    }

    private static Vector2 AngleToDir(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        Vector2 v = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        if (v.sqrMagnitude < 1e-6f) v = Vector2.down;
        return v.normalized;
    }

    private void Update()
    {
        if (!useFixedUpdate)
            Step(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (useFixedUpdate)
            Step(Time.fixedDeltaTime);
    }

    private void Step(float dt)
    {
        if (_tex == null) return;

        float stepDist = speedCellsPerSecond * dt;

        // Move each agent and write trails
        for (int i = 0; i < _agents.Count; i++)
        {
            Agent a = _agents[i];

            Vector2 prevPos = a.pos;
            Vector2 candidate = a.pos + a.vel * stepDist;

            // Wall reflection (analytical)
            ReflectOffBounds(ref a, ref candidate);

            // Traverse path from prevPos to candidate across grid cells
            bool hitTrail = TraverseAndStamp(prevPos, candidate, ref a);

            if (hitTrail)
            {
                // Flip direction (simple, arcade-y), add tiny noise to avoid infinite ping-pong
                a.vel = -a.vel;
                if (bounceNoiseDeg > 0f)
                {
                    float jitter = UnityEngine.Random.Range(-bounceNoiseDeg, bounceNoiseDeg);
                    a.vel = Quaternion.Euler(0, 0, jitter) * a.vel;
                    a.vel.Normalize();
                }
                // Keep position at prevPos on collision to avoid tunneling
                a.pos = prevPos;

                OnTrailHit?.Invoke();
            }
            else
            {
                a.pos = candidate;
            }

            _agents[i] = a;
        }

        // Periodic full wipe
        _wipeTimer += dt;
        if (_wipeTimer >= wipeIntervalSeconds)
        {
            _wipeTimer = 0f;
            FillAll(backgroundColor);
        }

        // Upload pixels once per frame
        _tex.SetPixels32(_pixels);
        _tex.Apply(false, false);
    }

    private void ReflectOffBounds(ref Agent a, ref Vector2 candidate)
    {
        // X bounds
        if (candidate.x < 0f)
        {
            candidate.x = 0f;
            a.vel.x = -a.vel.x;
        }
        else if (candidate.x >= gridWidth - 1e-3f)
        {
            candidate.x = gridWidth - 1e-3f;
            a.vel.x = -a.vel.x;
        }

        // Y bounds
        if (candidate.y < 0f)
        {
            candidate.y = 0f;
            a.vel.y = -a.vel.y;
        }
        else if (candidate.y >= gridHeight - 1e-3f)
        {
            candidate.y = gridHeight - 1e-3f;
            a.vel.y = -a.vel.y;
        }

        a.vel.Normalize();
    }

    /// <summary>
    /// Steps along the segment from start->end through grid cells.
    /// Returns true if we encountered an occupied cell (trail collision).
    /// Also stamps the path into the grid & pixel buffer.
    /// </summary>
    private bool TraverseAndStamp(Vector2 start, Vector2 end, ref Agent a)
    {
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        int steps = Mathf.CeilToInt(Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)));
        steps = Mathf.Clamp(steps, 1, maxCellsPerSegment);

        float inv = 1f / steps;
        bool hitTrail = false;

        // We read occupancy BEFORE writing, so crossing any existing trail triggers a bounce.
        for (int i = 1; i <= steps; i++)
        {
            float t = i * inv;
            float fx = Mathf.Lerp(start.x, end.x, t);
            float fy = Mathf.Lerp(start.y, end.y, t);

            int cx = (int)Mathf.Floor(fx);
            int cy = (int)Mathf.Floor(fy);

            if ((uint)cx >= (uint)gridWidth || (uint)cy >= (uint)gridHeight)
                continue;

            int idx = cy * gridWidth + cx;

            // Check first
            if (_occ[idx] != 0)
            {
                hitTrail = true;
                break;
            }

            // Then stamp (we always draw even if we bounced earlier in a previous frame)
            _occ[idx] = 1;
            _pixels[idx] = trailColor;
        }

        return hitTrail;
    }

    // ——— Utilities ———

    // Call this if you change grid size at runtime (Editor button etc.)
    public void Rebuild(int width, int height)
    {
        gridWidth = Mathf.Max(4, width);
        gridHeight = Mathf.Max(4, height);
        InitTextureAndGrid();
        SpawnAgents();
    }
}