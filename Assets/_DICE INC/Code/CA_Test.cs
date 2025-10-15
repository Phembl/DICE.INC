using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CA_Test : MonoBehaviour
{
    [SerializeField] private RawImage display;     
    [SerializeField] private int gridWidth = 100;
    [SerializeField] private int gridHeight = 100;
    
    [SerializeField] private Color32 colorDark = new Color32(0, 0, 0, 255);
    [SerializeField] private Color32 colorBright = new Color32(0, 255, 255, 255);
    
    
    private Texture2D tex;
    private Color32[] pixels;  // size = gridWidth * gridHeight
    private byte[] occ;        // 0 = empty, 1 = occupied
    
    private struct Agent
    {
        public Vector2Int pos; // in cell space, continuous
        public Vector2Int vel; // normalized direction
    }
    private readonly List<Agent> agents = new List<Agent>(128);
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tex = new Texture2D(gridWidth, gridHeight, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        pixels = new Color32[gridWidth * gridHeight];
        occ = new byte[gridWidth * gridHeight];
        
        ClearGrid(colorDark);
        display.texture = tex;
        
        //RandomPoints();
        CreateStartAgent();
        
        DrawTexture();
    }
    void ClearGrid(Color32 color)
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
            occ[i] = 0;
        }
    }
    
    void DrawTexture()
    {
        tex.SetPixels32(pixels);
        tex.Apply(false, false);
    }

    void RandomPoints()
    {
        for (int i = 0; i < pixels.Length; i++)
        {
            if (Random.value < 0.5f) //Active
            {
                pixels[i] = colorBright;
                occ[i] = 1;
            }
        }
    }

    void CreateStartAgent()
    {
        int middle = 50 * gridWidth + 50;
        pixels[middle] = colorBright;
        occ[middle] = 1;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
