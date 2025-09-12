using System;
using System.Collections;
using DICEINC.Global;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private ResourceManager resourceManager; 
   
    
    [HideInInspector] public float dicemakerGenerationInterval = 5f;
   
    /*
   public IEnumerator DicemakerGeneration()
   {
       bool dicemakerGeneration = true;
       while (dicemakerGeneration)
       {
           yield return new WaitForSeconds(dicemakerGenerationInterval);

          
           int diceToAdd = resourceManager.dicemaker; // 1 per dicemaker
           resourceManager.UpdateResource(Resource.Dice, diceToAdd);
           
           string buyDiceLogText = $"Your dicemakers just made <color=green>{diceToAdd}</color> dice.";
           logManager.UpdateLog(buyDiceLogText);
           
       }
   }
   */
   
}
