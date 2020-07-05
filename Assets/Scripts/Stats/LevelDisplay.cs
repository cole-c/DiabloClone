﻿using UnityEngine.UI;
using UnityEngine;
using System;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            GetComponent<Text>().text = String.Format("{000:##}", baseStats.GetLevel());
        }
    }
}