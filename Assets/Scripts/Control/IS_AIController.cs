using UnityEngine;
using RPG.Core;
using RPG.Combat;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class IS_AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float shoutDistance = 5f;
        [SerializeField] float fieldOfVision = 90f;
        [SerializeField] float visionDistance = 15f;

        Fighter fighter;
        Health health;
        GameObject player;
        Mover mover;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;
        int currentWaypointIndex = 0;
        float halfFOV = 45; //reset in Start but used for debugging

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
            halfFOV = fieldOfVision / 2;
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (IsAggro() && fighter.CanAttack(player))
            {
                //Aggro state
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                //Suspicion state
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;

                ai.Aggravate();
            }
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool IsAggro()
        {
            Vector3 playerDir = player.transform.position - transform.position;
            float angle = Vector3.Angle(playerDir, transform.forward);
            if(angle < halfFOV && playerDir.magnitude < visionDistance && IsPlayerVisible())
            {
                return true;
            }

            return false;
        }

        private bool IsPlayerVisible()
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, player.transform.position - transform.position, visionDistance); 
            foreach(RaycastHit hit in hits)
            {
                if(hit.transform.CompareTag("Player") || hit.transform.CompareTag("Enemy"))
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Transform playerHead = transform.Find("Health Bar");
            if(playerHead == null)
            {
                playerHead = transform;
            }

            Gizmos.color = Color.red;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV, Vector3.up);
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay(playerHead.position, leftRayDirection * visionDistance);
            Gizmos.DrawRay(playerHead.position, rightRayDirection * visionDistance);
        }
    }
}
