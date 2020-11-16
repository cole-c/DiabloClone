using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class IS_Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] IS_WeaponConfig defaultWeapon = null;
        [SerializeField] GameObject rangedTarget = null;

        //Health target;
        float timeSinceLastAttack = 5f;
        IS_WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        private List<Health> enemiesInRange = new List<Health>();

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            //if (target == null || target.IsDead()) return;

            //GetComponent<Mover>().Cancel();
            
            if(Input.GetMouseButtonDown(0))
            {
                AttackBehavior();
            }

        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void AttackBehavior()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            //This will trigger the Hit() event
            GetComponent<Animator>().SetTrigger("attack");
            GetComponent<Animator>().ResetTrigger("stopAttack");
        }

        //Animation Event
        void Hit()
        {

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeaponConfig.HasProjectile())
            {
                Debug.Log("Attempting to launch projectile");
                if (currentWeapon.value != null)
                {
                    //Calls the method inside Weapon.cs, ususally a sound effect
                    currentWeapon.value.OnHit();
                }
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, rangedTarget, gameObject, damage);
            }
            else
            {
                //Damage each enemy within melee range
                foreach(Health enemy in enemiesInRange)
                {
                    if (currentWeapon.value != null)
                    {
                        //Calls the method inside Weapon.cs, ususally a sound effect
                        currentWeapon.value.OnHit();
                    }
                    enemy.TakeDamage(gameObject, currentWeaponConfig.GetDamage());
                }
                
            }
        }

        //Animation Event
        void Shoot()
        {
            Hit();
        }

        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public void EquipWeapon(IS_WeaponConfig weapon)
        {
            if (weapon == null) return;
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(IS_WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void OnTriggerEnter(Collider other)
        {
            Health otherHealth = other.GetComponent<Health>();

            if (other.CompareTag("Player") || otherHealth == null) return;

            if(!enemiesInRange.Contains(otherHealth))
            {
                Debug.Log("Added Enemy to Melee Damage List!");
                enemiesInRange.Add(otherHealth);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Health otherHealth = other.GetComponent<Health>();

            if(otherHealth != null && enemiesInRange.Contains(otherHealth))
            {
                enemiesInRange.Remove(otherHealth);
            }
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            String weaponName = (string)state;
            IS_WeaponConfig weapon = UnityEngine.Resources.Load<IS_WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }

}
