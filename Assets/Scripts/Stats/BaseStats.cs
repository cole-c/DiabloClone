using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(0, 20)]
        [SerializeField] int startingLevel = 0;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticles = null;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;

        Experience experience;

        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            if(levelUpParticles != null)
            {
                Instantiate(levelUpParticles, transform);
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + (GetPercentageModifier(stat)/100));
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, currentLevel.value);
        }

        public int CalculateLevel()
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

        private float GetAdditiveModifier(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
    }
}
