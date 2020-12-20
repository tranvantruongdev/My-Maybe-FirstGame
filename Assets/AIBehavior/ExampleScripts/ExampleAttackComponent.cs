using UnityEngine;
using AIBehavior;

namespace AIBehaviorExamples
{
    public class ExampleAttackComponent : MonoBehaviour
    {
        [SerializeField] private float touchDmg = 5.0f;

        private GameObject player;

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.unityLogger.logEnabled = true;
#else
            Debug.unityLogger.logEnabled = false;
#endif
            player = GameObject.FindGameObjectWithTag("Player");
        }

        public void MeleeAttack(AttackData attackData)
        {
            Debug.Log("Melee attack");
            player.SendMessage("Damage", CalculateDamage(attackData));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                player.SendMessage("Damage", touchDmg);
        }

        float CalculateDamage(AttackData attackData)
        {
            float minDamage = attackData.damage - attackData.plusOrMinusDamage;
            float maxDamage = attackData.damage + attackData.plusOrMinusDamage;

            return Random.Range(minDamage, maxDamage);
        }
    }
}