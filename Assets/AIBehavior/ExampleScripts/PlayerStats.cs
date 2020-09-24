using UnityEngine;
using UnityEngine.UI;

namespace AIBehaviorExamples
{
	public class PlayerStats : MonoBehaviour
	{
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource loopAudio;
		public float health = 100.0f;
		[SerializeField] AudioClip hurtSound;
		[SerializeField] AudioClip deadSound;
		[SerializeField] GameObject hurtImg;

		public void SubtractHealth(float amount)
		{
			health -= amount;

			if ( health <= 0.0f )
			{
				health = 0.0f;
				Debug.LogWarning("You're Dead!");
				//Play dead sound
				_audioSource.PlayOneShot(deadSound);
				//Remove tag
				gameObject.tag = "Untagged";
			}
			else
			{
				Debug.Log("Health is now: " + health);
				//Play hurt sound
				_audioSource.PlayOneShot(hurtSound);
                if (health<=50)
                {
					loopAudio.Play();
					var color = hurtImg.GetComponent<Image>().color;
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
	}
}