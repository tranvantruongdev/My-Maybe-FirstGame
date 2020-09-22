using UnityEngine;


namespace AIBehaviorExamples
{
	public class PlayerStats : MonoBehaviour
	{
		public float health = 100.0f;
		public Texture tex1;

		public void SubtractHealth(float amount)
		{
			health -= amount;

			if ( health <= 0.0f )
			{
				health = 0.0f;
				Debug.LogWarning("You're Dead!");
			}
			else
			{
				Debug.Log("Health is now: " + health);
			}
		}

        private void OnGUI()
        {
            if (health<=90)
            {
				GUI.DrawTexture(Rect.zero, tex1, ScaleMode.ScaleToFit);
            }
        }

        public void Damage(float damage)
		{
			Debug.Log("Got hit with " + damage + " damage points");
			SubtractHealth(damage);
		}
	}
}