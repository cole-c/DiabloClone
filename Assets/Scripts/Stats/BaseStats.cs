using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(0, 20)]
        [SerializeField] int startingLevel = 0;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        private void Update()
        {
            
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, startingLevel);
        }

        public int GetLevel()
        {
            Experience exp = GetComponent<Experience>();

            if (exp is null) return startingLevel; //enemies don't have experience
            
            float currentXP = exp.GetExperience();
            int penultLevel = progression.GetLevels(Stat.ExperienceToLevel, characterClass);
            for (int level = startingLevel; level < penultLevel; level++)
            {
                float XpToLevel = progression.GetStat(Stat.ExperienceToLevel, characterClass, level);
                if(XpToLevel > currentXP)
                {
                    return level;
                }
            }

            return penultLevel + 1; //Last level to have "lvl up xp" will be second to last lvl
        }
    }
}
