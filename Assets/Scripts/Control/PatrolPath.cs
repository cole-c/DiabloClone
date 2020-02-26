using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            for(int i=0; i<transform.childCount; i++)
            {
                Gizmos.DrawSphere(transform.GetChild(i).transform.position, 0.25f);
            }
        }
    }
}
