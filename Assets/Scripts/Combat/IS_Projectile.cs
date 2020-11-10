using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class IS_Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        //[SerializeField] bool homing = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] UnityEvent onHit;

        GameObject target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());

            Destroy(gameObject, maxLifeTime);
        }

        private void Update()
        {
            //HOMING LOGIC CURRENTLY DISABLED
            //if (homing && !target.IsDead())
            //{
            //    transform.LookAt(GetAimLocation());
            //}

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(GameObject target, float damage, GameObject instigator)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
        }

        private Vector3 GetAimLocation()
        {
            return target.transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) return;
            if (other.GetComponent<Health>().IsDead()) return;

            Health hitHealth = other.GetComponent<Health>();

            if (hitHealth == null) return;
            hitHealth.TakeDamage(instigator, damage);

            speed = 0;

            onHit.Invoke();

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
