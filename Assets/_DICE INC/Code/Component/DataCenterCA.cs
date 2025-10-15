using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DICEINC.Global;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DataCenterCA : MonoBehaviour
{
    [SerializeField] private bool printLog;
    [SerializeField] private Datacenter datacenter;
    
    [Header("Grid")]
    [SerializeField] private RawImage display;     
    [SerializeField] private int gridWidth = 47;
    [SerializeField] private int gridHeight = 127;
    [SerializeField] private Color32 backgroundColor = new Color32(0, 0, 0, 255);
    [SerializeField] private Color32 trailColor = new Color32(0, 255, 255, 255);
    [SerializeField] private Color32 dataColor = new Color32(255, 100, 100, 255);

    [Header("Settings")]
    [Tooltip("50 ticks = 1 second.")]
    [SerializeField, MinValue(1)] private int ticksUntilMove;
    [SerializeField, MinValue(1)] private int ticksUntilWipe;
    public void ChangeWipeTicks(int change) => ticksUntilWipe += change;
    public int GetWipeTicks() => ticksUntilWipe;
    
    [SerializeField, MinValue(1)] private int ticksUntilSpawn;
    public void ChangeSpawnTicks(int change) => ticksUntilSpawn += change;
    public int GetSpawnTicks() => ticksUntilSpawn;

    //Tick tracking
    private int ticksUntilMoveCurrent;
    private int ticksUntilWipeCurrent;
    private int ticksUntilSpawnCurrent;
    
    //State tracking
    private bool isRunning;
    
    //crystal
    private bool crystalActive;
    private Vector2Int crystalSpawn;
    private int crystalSteps;
    [ShowInInspector, ReadOnly] private float crystalSpawnChance;
    public void ChangeCrystalSpawnChance(float newValue) => crystalSpawnChance = newValue;
    
    
    private float crystalGrowthChance;
    
    // Internal buffers
    private Texture2D tex;
    private Color32[] pixels;  // size = gridWidth * gridHeight
    private byte[] occ;        // 0 = empty, 1 = occupied

    private struct Agent
    {
        public Vector2Int pos; // in cell space, continuous
        public Vector2Int vel; // normalized direction
    }
    private readonly List<Agent> agents = new List<Agent>(128);
    private List<Vector2Int> spawnPositions = new List<Vector2Int>();
    
    private int spawnedCount; // how many agents have been spawned so far
    

#region |-------------- INIT --------------|
    

    public void initializeCA()
    {
        Debug.Log("Initializing DataCenter CA");
        if (display == null) Debug.LogError("DataCenter Texture is missing");
        
        ticksUntilWipeCurrent = ticksUntilWipe;
        ticksUntilMoveCurrent = ticksUntilMove;
        ticksUntilSpawnCurrent = ticksUntilSpawn;
        
        
        InitTextureAndGrid();
        
        SpawnAgent();
        
        //Start Loop
        isRunning = true;
    }
    
    private void InitTextureAndGrid()
    {
        tex = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        pixels = new Color32[gridWidth * gridHeight];
        occ = new byte[gridWidth * gridHeight];

        display.color = Color.white;
        ClearGrid(backgroundColor);
        display.texture = tex;

        CreateBorder();
        CreateSpawner();

        DrawTexture();
    }
    
#endregion
    
#region |-------------- LOOP --------------|
    
    private void FixedUpdate()
    {
        if (!isRunning) return;
        
        //Evaluate WipeGrid
        ticksUntilWipeCurrent--;
        if (ticksUntilWipeCurrent <= 0)
        {
            //wipe
            WipeGrid();
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
            if (crystalActive) RunCrystal();    //Cluster is created at the same speed as agents move
            
            ticksUntilMoveCurrent = ticksUntilMove;
        }
        
        // Upload pixels once per frame
        DrawTexture();
    }

#endregion

#region |-------------- GRID --------------|

    void DrawTexture()
    {
        tex.SetPixels32(pixels);
        tex.Apply(false, false);
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
                    occ[index] = 1;
                }
            }
        }
    }
    
    void ClearGrid(Color32 c)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = c;
            occ[i] = 0;
        }
    }
    
    void CreateSpawner()
    {
        spawnPositions.Clear(); 
        for (int x = 2; x < gridWidth - 2; x++)
        {
            if (x % 5 == 0)
            {
                spawnPositions.Add(new Vector2Int(x, 1));
                spawnPositions.Add(new Vector2Int(x, gridHeight - 2));
            }
            
        }
        
        if (printLog) Debug.Log($"DataCenter: Created {spawnPositions.Count} data spawn positions");
    }
    
    void WipeGrid()
    {
        if (printLog) Debug.Log("DataCenter: Perform wipe.");
        isRunning = false;

        StartCoroutine(WipeAnimation());
        
    }
    
    IEnumerator WipeAnimation()
    {

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int nextPosIndex = y * gridWidth + x;
                pixels[nextPosIndex] = trailColor;
                DrawTexture();
            }

            yield return new WaitForSeconds(0.01f);
        }
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int nextPosIndex = y * gridWidth + x;
                pixels[nextPosIndex] = backgroundColor;
                DrawTexture();
            }

            yield return new WaitForSeconds(0.01f);
        }
        
        ClearGrid(backgroundColor);
        // Full reset: clear agents
        agents.Clear();
        spawnedCount = 0;
        
        CreateBorder();
        CreateSpawner();
        
        SpawnAgent();


        if (Random.value < crystalSpawnChance) InitCrystal();
        
        isRunning = true;
    }

    void SetCell(int x, int y, Color32 color)
    {
        if (IsOutsideGrid(x,y)) return;
        
        int index = y * gridWidth + x;
        occ[index] = 1;
        pixels[index] = color;
    }
    
#endregion

#region |-------------- AGENTS --------------|
    
    private void SpawnAgent()
    {
        if (spawnedCount >= 18) return;
        
        if (printLog) Debug.Log("DataCenter: Spawning Agent.");
        
        Agent agent = new Agent();

        int randomSpawnIndex = Random.Range(0, spawnPositions.Count);
        Vector2Int nextAgentSpawn = spawnPositions[randomSpawnIndex];
        spawnPositions.RemoveAt(randomSpawnIndex);
        
        agent.pos = nextAgentSpawn;
        agent.vel.x = Random.Range(-1, 2);
        
        if (agent.pos.y > 10) agent.vel.y = -1;
        else agent.vel.y = 1;
        
        agents.Add(agent);
        spawnedCount++;
    }
    
    private void MoveAgent()
    {
        if (tex == null) return;

        // Move each agent and write trails
        for (int i = 0; i < agents.Count; i++)
        {
            Agent agent = agents[i];

            if (agent.vel == Vector2Int.zero) continue;
            
            //Create initial data agent
            Vector2Int currentPos = agent.pos;
            SetCell(currentPos.x, currentPos.y, trailColor);

            Vector2Int nextPos = currentPos + agent.vel;

            var hitData = HitSomething(agent.vel, currentPos);
            
            if (hitData.hit) //Agent Collision
            {
                //Add Data Resource
                CPU.instance.ChangeResource(Resource.Data, 1);
                datacenter.IncreaseLevel();
                
                agent.vel = hitData.newVelocity;
                nextPos = currentPos + agent.vel;
                
                //Checks if the next pos is outside the grid
                //If yes, agent is disabled
                if ((uint)nextPos.x >= (uint)gridWidth || (uint)nextPos.y >= (uint)gridHeight)
                {
                    agent.vel = Vector2Int.zero;
                    agents[i] = agent;
                    continue;
                }
            }
            
            //Set and draw point in Grid
            SetCell(nextPos.x, nextPos.y, dataColor);
            agent.pos = nextPos;
            agents[i] = agent;
            
        }
        
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
                bool up  = IsBlocked(x, y + 1);
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
                bool dn  = IsBlocked(x, y - 1);
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
                bool dn  = IsBlocked(x, y - 1);
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
    
    private bool IsBlocked(int x, int y)
    {
        //Checks if the target coordinate is either outside of the grid or occupied
        if ((uint)x >= (uint)gridWidth || (uint)y >= (uint)gridHeight)
            return true;
        return occ[y * gridWidth + x] == 1;
    }

    private bool IsOutsideGrid(int x, int y)
    {
        //Checks if the target coordinate is either outside of the grid or occupied
        if ((uint)x >= (uint)gridWidth || (uint)y >= (uint)gridHeight)
            return true;
        
        else return false;
    }

    private Vector2Int GetDirection(int directionIndex)
    {
        switch (directionIndex)
        {
            case 0: return new Vector2Int(-1, 1);   //Upper left
            
            case 1: return new Vector2Int(0, 1);    // Up
            
            case 2: return new Vector2Int(1, 1);    //Upper Right
            
            case 3: return new Vector2Int(-1, 0);   //Left
            
            case 4: return new Vector2Int(0, 0);    //None
            
            case 5: return new Vector2Int(1, 0);    //Right
            
            case 6: return new Vector2Int(-1, -1);  //Lower left
            
            case 7: return new Vector2Int(0, -1);   //Down
            
            case 8: return new Vector2Int(1, -1);   //Lower Right
            
            default: return new Vector2Int(Random.Range(-1,2), Random.Range(-1,2)); //Random
        }
    }
    
    #endregion
    
#region |-------------- CRYSTALS --------------|

    void InitCrystal()
    {
        // Create random Cluster seed away from border
        int rndX = Random.Range(10, gridWidth - 10);
        int rndY = Random.Range(10, gridHeight - 10);
        if (IsBlocked(rndX, rndY)) return;
        
        crystalSpawn = new Vector2Int(rndX, rndY);
        SetCell(crystalSpawn.x, crystalSpawn.y, dataColor);
        
        crystalSteps = 0;
        crystalGrowthChance = 1f;
        
        crystalActive = true;
        
    }

    void RunCrystal()
    {
        if (crystalSteps == 0) 
        {
            SetCell(crystalSpawn.x, crystalSpawn.y, trailColor);
            CreateNextCrystalStep(dataColor, 0);                     //Create initial Stage
        }
        else
        {
            CreateNextCrystalStep(trailColor, crystalSteps - 1);     //Set darker color for last stage
            CreateNextCrystalStep(dataColor, crystalSteps);                     //Create next Stage
        }
            
        crystalSteps++;
        
        if (Random.value > crystalGrowthChance)                          //Check for growthStop
        {
            CreateNextCrystalStep(trailColor, crystalSteps);                    //Set darker color for this stage
            crystalActive = false;
        }

        crystalGrowthChance -= 0.015f;

    }

    void CreateNextCrystalStep(Color32 clusterColor, int clusterStage)
    {
        int x = crystalSpawn.x;
        int y = crystalSpawn.y;
        int st = clusterStage + 1;

        if (st % 2 == 0) //Even
        {
            if (!IsOutsideGrid(x + st, y + st)) SetCell(x + st, y + st, clusterColor);
            if (!IsOutsideGrid(x - st, y - st)) SetCell(x - st, y - st, clusterColor);
            if (!IsOutsideGrid(x + st, y - st)) SetCell(x + st, y - st, clusterColor);
            if (!IsOutsideGrid(x - st, y + st)) SetCell(x - st, y + st, clusterColor);
        }
        else //Odd
        {
            if (!IsOutsideGrid(x, y + st)) SetCell(x, y + st, clusterColor);
            if (!IsOutsideGrid(x , y - st)) SetCell(x , y - st, clusterColor);
            if (!IsOutsideGrid(x + st, y)) SetCell(x + st, y, clusterColor);
            if (!IsOutsideGrid(x - st, y)) SetCell(x - st, y, clusterColor); 
        }
        
    }
    #endregion
    
    
}