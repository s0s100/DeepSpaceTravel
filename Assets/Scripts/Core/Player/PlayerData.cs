using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Player
{
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
            RestartGame();
        }

        private void RestartGame()
        {
            var curSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(curSceneIndex);
        }

        public Vector2 PlayerModelSize()
        {
            if (TryGetComponent<BoxCollider2D>(out var collider))
                return collider.size * transform.localScale;

            Debug.LogError("Unable to find player size");
            return Vector2.zero;
        }
    }
}