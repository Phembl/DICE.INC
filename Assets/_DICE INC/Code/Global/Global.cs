using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DICEINC.Global
{
    public enum Statistic
    {
      totalDiceRolls,
    }

    public enum InteractionAreaType
    {
        None,
        Import,
        Lab,
        Factory,
        Transformer,
        Technology,
        Stockmarket,
        Datacenter,
      
    }
    
    public enum Resource
    {
        Pips,
        Dice,
        Material,
        Luck,
        mDICE,
        Data
    }

    public struct TooltipData
    {
        public string areaTitle;
        public string areaDescription;
    }
    
    [Serializable]
    public struct MessageData
    {
        public string messageSubject;
        public string messageBody;
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
        
        public static bool Roll(float percentChance)
        {
            return Random.value < (percentChance / 100f);
        }

        public static IEnumerator WriteText(string text, TMP_Text textField)
        {
            textField.text = "";
            
            for (int i = 0; i < text.Length; i++)
            {
                textField.text += text[i];
                yield return new WaitForSeconds(0.05f);
            }
            
        }
    }
   
    
    
}
