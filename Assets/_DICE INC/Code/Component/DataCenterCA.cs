using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DICEINC.Global;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DataCenterCA : MonoBehaviour
{
    [SerializeField] private bool printLog;
    
    [Header("Grid")]
    [SerializeField] private RawImage display;     
    [SerializeField] private int gridWidth = 47;
    [SerializeField] private int gridHeight = 127;
    [SerializeField] private Color32 backgroundColor = new Color32(0, 0, 0, 255);
    [SerializeField] private Color32 trailColor = new Color32(0, 255, 255, 255);
    [SerializeField] private Color32 dataColor = new Color32(255, 100, 100, 255);

    [Header("Settings")]
    [Tooltip("50 ticks = 1 second.")]
    [SerializeField] private int ticksUntilMove = 5;
    [SerializeField] private int ticksUntilWipe = 800;
    [SerializeField] private int ticksUntilSpawn = 50;
    [SerializeField] private bool spawnImmediatelyOnStart = true; // spawn first agent at t=0
    [SerializeField] private int cluster;

    //Tick tracking
    private int ticksUntilMoveCurrent;
    private int ticksUntilWipeCurrent;
    private int ticksUntilSpawnCurrent;
    private bool isRunning;
    
    private float simTickInterval;
    private float tickTimer;
    
    // Internal buffers
    private Texture2D _tex;
    private Color32[] _pixels;  // size = gridWidth * gridHeight
    private byte[] _occ;        // 0 = empty, 1 = occupied

    private struct Agent
    {
        public Vector2Int pos; // in cell space, continuous
        public Vector2Int vel; // normalized direction
    }
    private readonly List<Agent> _agents = new List<Agent>(128);
    private List<Vector2Int> _spawnPositions = new List<Vector2Int>();
    
    private int _spawnedCount = 0; // how many agents have been spawned so far
    private int _movementTryCounter;
    

    #region |-------------- INIT --------------|
    
    private void Start()
    {
        if (display == null) Debug.LogError("DataCenter Texture is missing");
        
        ticksUntilWipeCurrent = ticksUntilWipe;
        ticksUntilMoveCurrent = ticksUntilMove;
        ticksUntilSpawnCurrent = ticksUntilSpawn;
    }

    public void initializeCA()
    {
        Debug.Log("Initializing DataCenter CA");
        
        simTickInterval = 1f / 100; //0.01s per tick
        
        InitTextureAndGrid();
        
        if (spawnImmediatelyOnStart) SpawnAgent();
        
        //Start Update
        isRunning = true;
    }
    
    private void InitTextureAndGrid()
    {
        _tex = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        _tex.wrapMode = TextureWrapMode.Clamp;

        _pixels = new Color32[gridWidth * gridHeight];
        _occ = new byte[gridWidth * gridHeight];

        ClearGrid(backgroundColor);
        display.texture = _tex;

        CreateBorder();
        CreateSpawner();

        _tex.SetPixels32(_pixels);
        _tex.Apply(false, false);
    }
    
    #endregion
    
    #region |-------------- LOOP --------------|
    
    private void Update()
    {
        /*
        if (!isRunning) return;
        
        tickTimer += Time.deltaTime;

        while (tickTimer >= simTickInterval)
        {
            tickTimer -= simTickInterval;
            RunSimulation();
        }
        */
        
    }
    private void FixedUpdate()
    {
        if (!isRunning) return;
        
        //Evaluate Wipe
        ticksUntilWipeCurrent--;
        if (ticksUntilWipeCurrent <= 0)
        {
            //wipe
            Wipe();
            //Reset ticks
            ticksUntilWipeCurrent = ticksUntilWipe;
            ticksUntilMoveCurrent = ticksUntilMove;
            ticksUntilSpawnCurrent = ticksUntilSpawn;
            return;
        }

        //Evaluate Spawn
        ticksUntilSpawnCurrent--;
        if (ticksUntilSpawnCurrent <= 0)
        {
            //Spawn
            SpawnAgent();
            ticksUntilSpawnCurrent = ticksUntilSpawn;
        }
          
        //Evaluate Movement
        ticksUntilMoveCurrent--;
        if (ticksUntilMoveCurrent <= 0)
        {
            MoveAgent();
            ticksUntilMoveCurrent = ticksUntilMove;
        }
        
        // Upload pixels once per frame
        DrawTexture();
    }

    #endregion
   
    #region |-------------- HELPER --------------|

    void DrawTexture()
    {
        _tex.SetPixels32(_pixels);
        _tex.Apply(false, false);
    }
    void CreateBorder()
    {
        //Draw invisible border (so that wall hit and trail hit can be computed as the same thing)
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                bool isBorder =
                    y == 0 || y == gridHeight - 1 ||
                    x == 0 || x == gridWidth  - 1;

                if (isBorder)
                {
                    int index = y * gridWidth + x;
                    _occ[index] = 1;
                }
            }
        }
    }
    
    private void ClearGrid(Color32 c)
    {
        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = c;
            _occ[i] = 0;
        }
    }
    
    void CreateSpawner()
    {
        _spawnPositions.Clear(); 
        for (int x = 2; x < gridWidth - 2; x++)
        {
            if (x % 5 == 0)
            {
                _spawnPositions.Add(new Vector2Int(x, 1));
                _spawnPositions.Add(new Vector2Int(x, gridHeight - 2));
            }
            
        }
        
        if (printLog) Debug.Log($"DataCenter: Created {_spawnPositions.Count} data spawn positions");
    }

    void CreateCluster()
    {
        int rndX = Random.Range(10, gridWidth-10);
        int rndY = Random.Range(10, gridHeight-10);
        
        
    }

    private void SpawnAgent()
    {
        if (_spawnedCount >= 18) return;
        
        if (printLog) Debug.Log("DataCenter: Spawning Agent.");
        
        Agent agent = new Agent();

        int randomSpawnIndex = Random.Range(0, _spawnPositions.Count);
        Vector2Int nextAgentSpawn = _spawnPositions[randomSpawnIndex];
        _spawnPositions.RemoveAt(randomSpawnIndex);
        
        agent.pos = nextAgentSpawn;
        agent.vel.x = Random.Range(-1, 2);
        
        if (agent.pos.y > 10) agent.vel.y = -1;
        else agent.vel.y = 1;
        
        _agents.Add(agent);
        _spawnedCount++;
    }
    
    private void MoveAgent()
    {
        if (_tex == null) return;

        // Move each agent and write trails
        for (int i = 0; i < _agents.Count; i++)
        {
            Agent agent = _agents[i];

            if (agent.vel == Vector2Int.zero) continue;
            
            
            Vector2Int currentPos = agent.pos;
            int currentIndex = currentPos.y * gridWidth + currentPos.x;
            _occ[currentIndex] = 1;
            _pixels[currentIndex] = trailColor;

            Vector2Int nextPos = currentPos + agent.vel;

            var hitData = HitSomething(agent.vel, currentPos);
            if (hitData.hit)
            {
                //Add Data
                CPU.instance.ChangeResource(Resource.Data, 1);
                
                agent.vel = hitData.newVelocity;
                nextPos = currentPos + agent.vel;
                
                // Checks if the next pos is outside the grid
                //If yes, agent is disabled
                if ((uint)nextPos.x >= (uint)gridWidth || (uint)nextPos.y >= (uint)gridHeight)
                {
                    agent.vel = Vector2Int.zero;
                    _agents[i] = agent;
                    continue;
                }
            }
            
            //Set and draw point in Grid
            int nextPosIndex = nextPos.y * gridWidth + nextPos.x;
            _occ[nextPosIndex] = 1;
            _pixels[nextPosIndex] = dataColor;
            
            agent.pos = nextPos;
            _agents[i] = agent;
            
        }
        
    }
    
    private void Wipe()
    {
        if (printLog) Debug.Log("DataCenter: Perform wipe.");
        isRunning = false;

        StartCoroutine(WipeAnimation());
        
    }

    private IEnumerator WipeAnimation()
    {

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int nextPosIndex = y * gridWidth + x;
                _pixels[nextPosIndex] = trailColor;
                DrawTexture();
            }

            yield return new WaitForSeconds(0.01f);
        }
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int nextPosIndex = y * gridWidth + x;
                _pixels[nextPosIndex] = backgroundColor;
                DrawTexture();
            }

            yield return new WaitForSeconds(0.01f);
        }
        
        ClearGrid(backgroundColor);
        // Full reset: clear agents
        _agents.Clear();
        _spawnedCount = 0;
        
        CreateBorder();
        CreateSpawner();
        
        if (spawnImmediatelyOnStart) SpawnAgent();
        
        isRunning = true;
    }

    
    private (bool hit, Vector2Int newVelocity) HitSomething(Vector2Int velocity, Vector2Int currentPos)
{
    Vector2Int newVel = Vector2Int.zero;

    int x = currentPos.x;
    int y = currentPos.y;

    /*
     * Hit Alignment:
     * / — \   0 1 2
     * | 0 |   3 4 5
     * \ — /   6 7 8
     */
    
    switch (velocity.x, velocity.y)
    {
        case (0, 1): // ↑
        {
            bool ahead = IsBlocked(x, y + 1);
            if (ahead)
            {
                bool right = IsBlocked(x + 1, y);
                bool left  = IsBlocked(x - 1, y);

                if (right)       newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(6); // left / lower-left
                else if (left)   newVel = Random.value > 0.5f ? GetDirection(5) : GetDirection(8); // right / lower-right
                else             newVel = Random.value > 0.5f ? GetDirection(6) : GetDirection(8); // lower-left / lower-right

                return (true, newVel);
            }
            break;
        }

        case (-1, 0): // ←
        {
            bool ahead = IsBlocked(x - 1, y);
            if (ahead)
            {
                bool up   = IsBlocked(x, y + 1);
                bool down = IsBlocked(x, y - 1);

                if (up)         newVel = Random.value > 0.5f ? GetDirection(1) : GetDirection(2); // up / upper-right
                else if (down)  newVel = Random.value > 0.5f ? GetDirection(7) : GetDirection(8); // down / lower-right
                else            newVel = Random.value > 0.5f ? GetDirection(2) : GetDirection(8); // upper-right / lower-right

                return (true, newVel);
            }
            break;
        }

        case (1, 0): // →
        {
            bool ahead = IsBlocked(x + 1, y);
            if (ahead)
            {
                bool up   = IsBlocked(x, y + 1);
                bool down = IsBlocked(x, y - 1);

                if (up)         newVel = Random.value > 0.5f ? GetDirection(7) : GetDirection(8); // down / lower-right
                else if (down)  newVel = Random.value > 0.5f ? GetDirection(1) : GetDirection(2); // up / upper-right
                else            newVel = Random.value > 0.5f ? GetDirection(0) : GetDirection(6); // upper-left / lower-left

                return (true, newVel);
            }
            break;
        }

        case (0, -1): // ↓
        {
            bool ahead = IsBlocked(x, y - 1);
            if (ahead)
            {
                bool right = IsBlocked(x + 1, y);
                bool left  = IsBlocked(x - 1, y);

                if (right)      newVel = Random.value > 0.5f ? GetDirection(5) : GetDirection(2); // right / upper-right
                else if (left)  newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(0); // left / upper-left
                else            newVel = Random.value > 0.5f ? GetDirection(0) : GetDirection(2); // upper-left / upper-right

                return (true, newVel);
            }
            break;
        }

        case (1, 1): // ↗
        {
            bool diag = IsBlocked(x + 1, y + 1);
            if (diag)
            {
                bool up  = IsBlocked(x,     y + 1);
                bool rt  = IsBlocked(x + 1, y);

                if (up)        newVel = Random.value > 0.5f ? GetDirection(7) : GetDirection(8); // down / lower-right
                else if (rt)   newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(0); // left / upper-left
                else           newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(7); // left / down

                return (true, newVel);
            }
            else if (IsBlocked(x, y + 1))
            {
                newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(7); // left / down
                return (true, newVel);
            }
            break;
        }

        case (-1, 1): // ↖
        {
            bool diag = IsBlocked(x - 1, y + 1);
            if (diag)
            {
                bool up  = IsBlocked(x,     y + 1);
                bool lf  = IsBlocked(x - 1, y);

                if (up)        newVel = Random.value > 0.5f ? GetDirection(7) : GetDirection(6); // down / lower-left
                else if (lf)   newVel = Random.value > 0.5f ? GetDirection(5) : GetDirection(2); // right / upper-right
                else           newVel = Random.value > 0.5f ? GetDirection(7) : GetDirection(5); // down / right

                return (true, newVel);
            }
            else if (IsBlocked(x, y + 1))
            {
                newVel = Random.value > 0.5f ? GetDirection(7) : GetDirection(5); // down / right
                return (true, newVel);
            }
            break;
        }

        case (-1, -1): // ↙
        {
            bool diag = IsBlocked(x - 1, y - 1);
            if (diag)
            {
                bool dn  = IsBlocked(x,     y - 1);
                bool lf  = IsBlocked(x - 1, y);

                if (dn)        newVel = Random.value > 0.5f ? GetDirection(0) : GetDirection(1); // upper-left / up
                else if (lf)   newVel = Random.value > 0.5f ? GetDirection(5) : GetDirection(8); // right / lower-right
                else           newVel = Random.value > 0.5f ? GetDirection(1) : GetDirection(5); // up / right

                return (true, newVel);
            }
            else if (IsBlocked(x, y - 1))
            {
                newVel = Random.value > 0.5f ? GetDirection(1) : GetDirection(5); // up / right
                return (true, newVel);
            }
            break;
        }

        case (1, -1): // ↘
        {
            bool diag = IsBlocked(x + 1, y - 1);
            if (diag)
            {
                bool dn  = IsBlocked(x,     y - 1);
                bool rt  = IsBlocked(x + 1, y);

                if (dn)        newVel = Random.value > 0.5f ? GetDirection(1) : GetDirection(2); // up / upper-right
                else if (rt)   newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(6); // left / lower-left
                else           newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(1); // left / up

                return (true, newVel);
            }
            else if (IsBlocked(x, y - 1))
            {
                newVel = Random.value > 0.5f ? GetDirection(3) : GetDirection(1); // left / up
                return (true, newVel);
            }
            break;
        }
    }

    return (false, newVel);
}
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsBlocked(int x, int y)
    {
        //Checks if the target coordinate is either outside of the grid or occupied
        if ((uint)x >= (uint)gridWidth || (uint)y >= (uint)gridHeight)
            return true;
        return _occ[y * gridWidth + x] == 1;
    }

    private Vector2Int GetDirection(int directionIndex)
    {
        Vector2Int direction = new Vector2Int();

        switch (directionIndex)
        {
            case 0: //Upper left
                direction.x = -1;
                direction.y = 1;
                break;
            
            case 1: // Up
                direction.x = 0;
                direction.y = 1;
                break;
            
            case 2: //Upper Right
                direction.x = 1;
                direction.y = 1;
                break;
            
            case 3: //Left
                direction.x = -1;
                direction.y = 0;
                break;
            
            case 4: //None
                direction.x = 0;
                direction.y = 0;
                break;
            
            case 5: //Right
                direction.x = 1;
                direction.y = 0;
                break;
            
            case 6: //Lower left
                direction.x = -1;
                direction.y = -1;
                break;
            
            case 7: //Down
                direction.x = 0;
                direction.y = -1;
                break;
            
            case 8: //Lower Right
                direction.x = 1;
                direction.y = -1;
                break;
                
                
        }
        
        return direction;
    }
    
    #endregion
    
}