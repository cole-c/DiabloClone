using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        float destroyDelay = 2f;

        [SerializeField] Text damageText = null;

        public void Start()
        {
            //Destroy the object after 2 seconds so they don't clutter up our scene
            Destroy(gameObject, destroyDelay);
        }

        public void SetValue(float amount)
        {
            damageText.text = String.Format("{0:0}", amount);
        }
    }
}
