using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class IS_PlayerMover : MonoBehaviour, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float moveSpeed = 1000f;

        NavMeshAgent navMeshAgent;
        Health health;
        public Transform cameraTransform;
        Vector3 previousPosition;
        float currentSpeed = 0;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();

            cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            previousPosition = transform.position;
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();

            UpdateAnimator();
        }

        void FixedUpdate()
        {
            //Calculate Speed for animator
            Vector3 moved = transform.position - previousPosition;
            currentSpeed = moved.magnitude / Time.deltaTime;
            previousPosition = transform.position;

            //Get Player Input for Movement
            Vector3 movementDirection = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) movementDirection += cameraTransform.forward;
            if (Input.GetKey(KeyCode.S)) movementDirection -= cameraTransform.forward;
            if (Input.GetKey(KeyCode.A)) movementDirection -= cameraTransform.right;
            if (Input.GetKey(KeyCode.D)) movementDirection += cameraTransform.right;

            //TODO change this to jump
            movementDirection.y = 0f;

            //Move on the navmesh
            navMeshAgent.Move(movementDirection.normalized * moveSpeed * Time.deltaTime);

            //Rotate toward movement direction
            if (movementDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementDirection), 0.15F);
            }
        }

        //KEEP
        private void UpdateAnimator()
        {
            GetComponent<Animator>().SetFloat("forwardSpeed", currentSpeed);
        }

        //KEEP
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        //KEEP
        public void RestoreState(object state)
        {
            SerializableVector3 position = state as SerializableVector3;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

}
