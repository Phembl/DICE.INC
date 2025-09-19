using Shapes;
using UnityEngine;

public class Stockmarket_Entry : MonoBehaviour
{
    private int frameCounter = 5;
    private Canvas parentCanvas;
    private Line lineComponent;
    
  
    void Start()
    {
        lineComponent = GetComponent<Line>();
        parentCanvas = transform.parent.transform.parent.transform.parent.GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (frameCounter == 5)
        {
            frameCounter = 0;
            lineComponent.SortingOrder = parentCanvas.sortingOrder + 1;
        }
        frameCounter++;
        
    }
}
