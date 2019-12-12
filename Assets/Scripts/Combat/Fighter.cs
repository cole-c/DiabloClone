using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] float weaponRange = 2f;

        Transform target;

        private void Update()
        {
            if (target == null) return;

            bool isInRange = Vector3.Distance(transform.position, target.position) < weaponRange;
            if (!isInRange)
            {
                GetComponent<Mover>().MoveTo(target.position);
            }
            else
            {
                GetComponent<Mover>().Stop();
            }

        }

        public void Attack(CombatTarget target)
        {
            this.target = target.transform;
            Debug.Log("Combat Initiated!");
        }

        public void Cancel()
        {
            if (target != null)
                Debug.Log("Bravely Ran Away!");
            target = null;
        }
    }

}