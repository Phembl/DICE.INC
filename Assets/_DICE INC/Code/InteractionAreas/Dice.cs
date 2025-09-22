using System.Collections.Generic;
using DICEINC.Global;
using TMPro;
using UnityEngine;
using System.Linq;

public class Dice : MonoBehaviour
{
   [SerializeField] private TMP_Text diceRollCounter;

   public bool printLog;
   
   public static Dice instance;
   private void Awake()
   {
      if (instance == null) instance = this;
   }
   
   void OnEnable()
   {
      CPU.OnDiceRollTotalChanged += UpdateRollCounter;
   }
   
   public void RollDice() //Called by RollDice Button_Tooltip
   {
      int diceToRoll = (int)CPU.instance.GetDice();
      
      if (diceToRoll == 0) return;
      
      int diceUpperLimit = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 0) + 7; //Needs to be + 7 because of maxExclusive Random.Range
      int diceLowerLimit = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 2) + 1; //Uses highRoller + 1, otherwise highRoller (1) would useless
      int advantageValue = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 1) + 1; //Uses Advantage + 1, because if Advantage = 0 it needs to simply roll 1 time
      float explosionChance = (float)CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 3);
      
      int diceToRollOriginal = diceToRoll;
      int diceRolled = 0;
      int explosionCount = 0;
      int explosionVolume = 0;
      
      
      if (printLog) Debug.Log("|--------------DICE ROLLS --------------|");
      if (printLog) Debug.Log($"Rolling {diceToRoll} Dice");
      if (printLog) Debug.Log($"Every dice is rolled {advantageValue} times.");
      if (printLog) Debug.Log($"Every dice rolls between {diceLowerLimit} and {diceUpperLimit - 1}.");
      if (printLog) Debug.Log($"Chance for dice to explode: {explosionChance}%");
      
      List<int> diceResultsNew =  new List<int>();
      
      for (int i = 0; i < diceToRoll; i++)
      {
         //Rolling every Dice Advantage-times
         //Creating another int that stores all advantage rolls, then picking the highest
         int[] advantageDiceResults = new int[advantageValue];
         
         for (int advantageRoll = 0; advantageRoll < advantageValue; advantageRoll++)
         {
            //Here are the actual diceRolls
            advantageDiceResults[advantageRoll] = Random.Range(diceLowerLimit, diceUpperLimit);
         }
         
         //Gets the highest result from Advantage rolls
         int currentResult = advantageDiceResults.Max();
         diceResultsNew.Add(currentResult);
         
         //If Luck is unlocked and the dice result is >= 6: Add Luck
         if (CPU.instance.GetLuckUnlockState() && currentResult >= 6)
         {
            CPU.instance.ChangeResource(Resource.Luck, 1);
         }
         
         diceRolled++;

        //Roll for explosion (will always be false if explosion is not unlocked)
         if (Utility.Roll(explosionChance))
         {
            int explosionGeneratedDice = Random.Range(1, 7);
            
            //These are only for tracking
            explosionCount++;
            explosionVolume += explosionGeneratedDice;
            
            diceToRoll += explosionGeneratedDice;
         }
         
         
        
      }
        
      int pipsGenerated = 0;
      for (int i = 0; i < diceResultsNew.Count; i++)
      {
         pipsGenerated += diceResultsNew[i];
      }
      
      if (printLog) Debug.Log($"{explosionCount} dice exploded, adding {explosionVolume} new dice.");
      if (printLog) Debug.Log($"Actually rolled dice: {diceResultsNew.Count} Dice");
      if (printLog) Debug.Log($"Result of rolled dice: {pipsGenerated} Pips.");
      
      //Check if Stockmarket is unlocked
      if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 0))
      {
         //Mult with Dice Roll Stockmarket value
         pipsGenerated = (int)(pipsGenerated * CPU.instance.GetDiceRollStockValue());
         if (printLog) Debug.Log($"Result of rolled dice after stockmarket: {pipsGenerated} Pips.");
      }
      
      CPU.instance.ChangeResource(Resource.Pips, pipsGenerated);
      CPU.instance.ChangeDiceRolledTotal(diceRolled); //Uses this value in case rolls are multiplied
      CPU.instance.ChangeResource(Resource.Dice, -diceToRollOriginal); //Uses this value because exploded dice should not be counted here
      
      Diceworld.instance.UpdateRollCounter(diceRolled);
      
      if (printLog) Debug.Log("|--------------DICE ROLLS FINISHED--------------|");
      
   }

   void UpdateRollCounter()
   {
      diceRollCounter.text = CPU.instance.GetDiceRolledTotal().ToString();
   }
   
}
