using System.Collections.Generic;
using DICEINC.Global;
using TMPro;
using UnityEngine;
using System.Linq;

public class DiceManager : MonoBehaviour
{
   [SerializeField] private TMP_Text diceRollCounter;
   [SerializeField] private DiceTable diceTable;

   public bool printLog;
   
   public static DiceManager instance;
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
     
      int diceSides = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 0) + 6;
      int advantageValue = CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 1) + 1; //Uses Advantage + 1, because if Advantage = 0 it needs to simply roll 1 time
      float explosionChance = (float)CPU.instance.GetAreaInteractorCount(InteractionAreaType.Technology, 3);
      
      int diceToRollOriginal = diceToRoll;
      int diceRolled = 0;
      int explosionCount = 0;
      int explosionVolume = 0;
      int luckGenerated = 0;
      
     
     if (printLog) Debug.Log("|--------------DICE ROLLS: --------------|");
     if (printLog) Debug.Log($"Rolling {diceToRoll} dice");
     if (printLog) Debug.Log($"Every dice has {diceSides} sides and is rolled {advantageValue} times.");
     if (printLog) Debug.Log($"Chance for dice to explode: {explosionChance}%");
     /*
         List<int> diceResultsNew =  new List<int>();

         for (int i = 0; i < diceToRoll; i++)
         {
            //Rolling every DiceManager Advantage-times
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

           */
      
      List<int> diceResultsNew =  new List<int>();
      
      for (int i = 0; i < diceToRoll; i++)
      {
         int currentResult = diceTable.GetDiceResult();
         diceResultsNew.Add(currentResult);
         diceRolled++;
         
         //If Luck is unlocked and the dice result is >= 6: Add Luck
         if (currentResult >= 6)
         {
            CPU.instance.ChangeResource(Resource.Luck, 1);
            luckGenerated++;
         }

         
         //Roll for explosion (will always be false if explosion is not unlocked)
         if (Utility.Roll(explosionChance))
         {
            int explosionGeneratedDice = diceTable.GetExplosionResult();

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
      
      //if (printLog) Debug.Log($"{explosionCount} dice exploded, adding {explosionVolume} new dice.");
      if (printLog) Debug.Log($"Actually rolled dice: {diceResultsNew.Count}.");
      if (printLog) Debug.Log($"Result of rolled dice: {pipsGenerated} Pips.");
      if (printLog) Debug.Log($"{luckGenerated} Luck has been generated.");
      
      //Check if Stockmarket is unlocked
      if (CPU.instance.GetInteractorUnlockState(InteractionAreaType.Stockmarket, 0))
      {
         //Mult with DiceManager Roll Stockmarket value
         pipsGenerated = (int)(pipsGenerated * CPU.instance.GetDiceRollStockValue());
         if (printLog) Debug.Log($"Result of rolled dice after stockmarket: {pipsGenerated} Pips.");
      }
      
      CPU.instance.ChangeResource(Resource.Pips, pipsGenerated);
      CPU.instance.ChangeDiceRolledTotal(diceRolled); //Uses this value in case rolls are multiplied
      CPU.instance.ChangeResource(Resource.Dice, -diceToRollOriginal); //Uses this value because exploded dice should not be counted here
      
      Technology.instance.UpdateRollCounter(diceRolled);
      
      if (printLog) Debug.Log("|--------------DICE ROLLS FINISHED--------------|");
      
   }

   void UpdateRollCounter()
   {
      diceRollCounter.text = CPU.instance.GetDiceRolledTotal().ToString();
   }
   
}
