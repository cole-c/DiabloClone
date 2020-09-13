using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            BridgeGate, BossGate
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(sceneToLoad < 0)
            {
                Debug.LogError("Scene to Load not set in Portal");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper saveSystem = FindObjectOfType<SavingWrapper>();
            PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            //Remove control from player to prevent them re-entering a portal while loading the new scene
            playerController.enabled = false;

            yield return fader.FadeOut(fadeOutTime);

            //Save current level
            saveSystem.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            //Technically a new player object in the new scene, disable control again here
            PlayerController newPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            //Load current level
            saveSystem.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            //Save again so we can load from current scene
            saveSystem.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            //Restore control to the player
            newPlayerController.enabled = true;

            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}
