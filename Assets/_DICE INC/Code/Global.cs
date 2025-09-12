using UnityEngine;

namespace DICEINC.Global
{
    public enum Statistic
    {
      totalDiceRolls,
    }

    public enum InteractionAreaType
    {
        None,
        Shop,
        Lab,
        Workshop,
        Casino,
        Diceworld,
        Stockmarket,
        Datacenter
        
    }
    
    public enum Resource
    {
        Pips,
        Dice,
        Tools,
        Luck,
        mDice,
        Data
    }
    
    public static class Utility
    {
        public static string ShortenNumberToString(double value)
        {
            if (value < 1000) return value.ToString();
            
            string[] suffixes = { "", "k", "M", "B", "T", "Q" };
            
            int suffixIndex = 0;

            // Keep dividing by 1000 until value < 1000, or we run out of suffixes
            while (value >= 1000 && suffixIndex < suffixes.Length - 1)
            {
                value /= 1000;
                suffixIndex++;
            }

            // Always show 2 decimals, e.g. 1.20k instead of 1.2k
            return value.ToString("F2") + suffixes[suffixIndex];
        }
    }
   
    
    
}
