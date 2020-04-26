﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }

        public void Save()
        {
            //save current gamestate into the save file
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }


        public void Load()
        {
            //load data from save file
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }
    }

}