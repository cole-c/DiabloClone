﻿using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        public float GetHealth(CharacterClass characterClass, int level)
        {
            foreach(ProgressionCharacterClass progressionClass in characterClasses)
            {
                if(progressionClass.characterClass == characterClass)
                {
                     //return progressionClass.health[level];
                }

            }
            return 0f;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}