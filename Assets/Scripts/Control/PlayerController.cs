using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        enum CursorType
        {
            None, 
            Movement,
            Combat
        }

        struct CursorMapping
        {
            public CursorType type;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;

        private void Start()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead()) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach(RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                GameObject targetObject = target.gameObject;
                if(!GetComponent<Fighter>().CanAttack(targetObject))
                {
                    continue;
                }
                
                if(Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(targetObject);    
                }
                SetCursor(CursorType.Combat);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {

        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
