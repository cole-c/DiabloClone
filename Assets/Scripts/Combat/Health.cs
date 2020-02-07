using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoints = 100f;

        bool dead = false;

        public bool IsDead()
        {
            return dead;
        }

        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            if(healthPoints == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if(!dead)
            {
                GetComponent<Animator>().SetTrigger("die");
                dead = true;
            }
        }
    }
}
