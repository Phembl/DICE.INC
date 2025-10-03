using System.Collections.Generic;
using System.Linq;
using DICEINC.Global;
using RNGNeeds;
using UnityEngine;

public class DiceTable : MonoBehaviour
{
    private int currentSides = 6;
    private float weightMod;
    
    public ProbabilityList<int> diceResult;
    public ProbabilityList<int> explosionResult;

    public int GetExplosionResult() => explosionResult.PickValue();
    public int GetDiceResult()
    {
        //Gets the count of Advantage from CPU manager (=>Technology, 1)
        List<int> results = diceResult.PickValues((CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 1) + 1));
        return results.Max();
    }

    public void InitializeDiceTable(int _diceSides, float _weightMod)
    {
        currentSides += _diceSides;
        weightMod = _weightMod;

        //If this is initialized with some weight already purchased, calculate start weight
        int weightCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 2);
        if (weightCount > 0)
        {
            for (int i = 0; i < weightCount; i++)
            {
                AdjustWeight();
            }
        }
    }
    
    public void AddSide()
    {
        currentSides++;
        diceResult.AddItem(currentSides);
        AdjustWeight();
        
    }

    public void AdjustWeight()
    {
        int weightCount = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 2);

        if (weightCount == 0) return;
        
        float currentMedian = 100 / currentSides;
        float currentWeightMod = weightMod * weightCount;
        currentMedian /= 100;
        
        for (int i = 0; i < currentSides; i++)
        {
            diceResult.SetItemBaseProbability(i, currentMedian + (currentWeightMod * (i + 1 )));
        }

        
    }
}
