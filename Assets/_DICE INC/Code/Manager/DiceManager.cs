using DICEINC.Global;
using TMPro;
using UnityEngine;
using System.Linq;

public class DiceManager : MonoBehaviour
{
   [SerializeField] private TMP_Text diceRollCounter;
   [SerializeField] private Interactor_RollDice rollDiceButton;
   
   public static DiceManager instance;
   private void Awake()
   {
      if (instance == null) instance = this;
   }
   
   void OnEnable()
   {
      CPU.OnDiceRollTotalChanged += UpdateRollCounter;
   }
   
   public void RollDice() //Called by RollDice Button
   {
      int dicesToRoll = (int)CPU.instance.GetDice();
      
      if (dicesToRoll == 0) return;
        
      int diceRolled = 0;
      
      //TODO: Roll for explosive here
      
      int[] diceResults = new int[dicesToRoll];

      for (int i = 0; i < dicesToRoll; i++)
      {
         //Rolling every Dice Advantage-times
         //Creating another int that stores all advantage rolls, then picking the highest
         int advantageValue = (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 1) + 1);
         int[] advantageDiceResults = new int[advantageValue];
         for (int rolls = 0; rolls < advantageValue; rolls++)
         {
            //Here are the actual diceRolls
            //Highest value = 6 + extra sides created by Diceworld
            advantageDiceResults[rolls] = Random.Range(1,
               (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 0) + 7));
         }
         
         //Gets the highest result from Advantage rolls
         diceResults[i] = advantageDiceResults.Max();
         
         //Checks for high Roller value and uses it as lower limit for the roll result
         //Uses highRoller + 1, otherwise highRoller (1) would useless
         //TODO: Limit to dice Sides (make highRoller unpurchasable if it would be higher than sides)
         int lowerLimit = (CPU.instance.GetAreaInteractorCount(InteractionAreaType.Diceworld, 2) + 1);
         if (diceResults[i] < lowerLimit) diceResults[i] = lowerLimit;
         
         //Add Luck if Result was a 6 or Higher
         if (CPU.instance.GetLuckUnlockState() && diceResults[i] >= 6) CPU.instance.ChangeResource(Resource.Luck, 1);
         diceRolled++;
      }
        
      int pipsGenerated = 0;
      for (int i = 0; i < diceResults.Length; i++)
      {
         pipsGenerated += diceResults[i];
      }
      
      CPU.instance.ChangeResource(Resource.Pips, pipsGenerated);
      CPU.instance.ChangeDiceRolledTotal(diceRolled); //Uses this value in case rolls are multiplied
      CPU.instance.ChangeResource(Resource.Dice, -dicesToRoll);
      
      Diceworld.instance.UpdateRollCounter(diceRolled);
      
      rollDiceButton.ShowRollResult(diceRolled, pipsGenerated);
      
   }

   void UpdateRollCounter()
   {
      diceRollCounter.text = CPU.instance.GetDiceRolledTotal().ToString();
   }
   
}
