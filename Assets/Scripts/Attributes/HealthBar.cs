using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        void Update()
        {
            if(Mathf.Approximately(healthComponent.GetFractionHP(), 0) 
            || Mathf.Approximately(healthComponent.GetFractionHP(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }
            else
            {
                rootCanvas.enabled = true;
            }
            
            foreground.localScale = new Vector3(healthComponent.GetFractionHP(), 1, 1);
        }
    }

}