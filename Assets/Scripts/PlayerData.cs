using UnityEngine;

namespace Player
{
    // Class which stores player object data and stores/calculates required data
    public class PlayerData : MonoBehaviour
    {
        [SerializeField] private int curHealth = 1;
        [SerializeField] private int maxHealth = 1;

        [SerializeField] private float speed = 1.0f;

        public float Speed => speed;
        public int CurHealth => curHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            curHealth = maxHealth;
        }

        public void ReceiveDamage(int damage)
        {
            curHealth -= damage;
            if (curHealth <= 0)
                Death();
        }

        private void Death()
        {
            Debug.Log("The game is over!");
        }
    }
}