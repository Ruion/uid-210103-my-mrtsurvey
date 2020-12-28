using UnityEngine;
using UnityEngine.Events;


    public class GameManager : MonoBehaviour
    {

        public UnityEvent StartGameEvents;
        public UnityEvent GameOverEvents;

        [ReadOnly]
        public float timeScale;

        public float gameTimeScale { get { return timeScale; } set { timeScale = value; } }

        [HideInInspector]
        public bool isGameEnded;

        // Use this for initialization
        public void StartGame()
        {
            StartGameEvents.Invoke();
        }

        // Update is called once per frame
        public void GameOver()
        {
            isGameEnded = true ;
            GameOverEvents.Invoke();
        }
    }
