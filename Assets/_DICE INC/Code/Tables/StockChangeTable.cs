using RNGNeeds;
using UnityEngine;

public class StockChangeTable : MonoBehaviour
{
    public ProbabilityList<float> changeValue;
    
    public float GetChangeValue() => changeValue.PickValue();
    
}
