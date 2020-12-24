using UnityEngine;
using UnityEngine.UI;
//Supress default null warning
#pragma warning disable 0649

namespace AIBehaviorExamples
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        //for breath scare sound
        [SerializeField] private AudioSource loopAudio;
        [SerializeField] AudioClip hurtSound;
        [SerializeField] AudioClip deadSound;
        [SerializeField] GameObject hurtImg;
        [SerializeField] Text scoreText;
        [SerializeField] private int score;

        private float health = 100.0f;
        private float maxHealth = 100.0f;

        public int Score1 { get => score; set => score = value; }

        //health must not be allowed to be larger than maxHealth
        public float Health { get => health; set => health = value > MaxHealth ? MaxHealth : value; }
        public float MaxHealth { get => maxHealth; set => maxHealth = value; }

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
        }

        public void SubtractHealth(float amount)
        {
            health -= amount;

            if (health <= 0.0f)
            {
                health = 0.0f;
                Debug.LogWarning("You're Dead!");
                //Disable loop audio
                loopAudio.enabled = false;
                //Play dead sound
                audioSource.PlayOneShot(deadSound);
                //Find Game Manager and put an end to this
                FindObjectOfType<GameManager>().GameOver();
            }
            else
            {
                Debug.Log("Health is now: " + health);
                //Play hurt sound
                audioSource.PlayOneShot(hurtSound);
                if (health <= (MaxHealth / 2))
                {
                    loopAudio.Play();
                    Color color = hurtImg.GetComponent<Image>().color;
                    color.a = 0.5f;
                    hurtImg.GetComponent<Image>().color = color;
                }
            }
        }

        public void Damage(float damage)
        {
            Debug.Log("Got hit with " + damage + " damage points");
            SubtractHealth(damage);
        }

        public void Score(int score)
        {
            this.score += score;
            scoreText.text = this.score.ToString();
        }
    }
}