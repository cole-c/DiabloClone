using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool played = false;

        private void OnTriggerEnter(Collider other)
        {
            if(!played && other.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                played = true;
            }
        }

        public object CaptureState()
        {
            return played;
        }

        public void RestoreState(object state)
        {
            played = (bool) state;
        }
    }
}
