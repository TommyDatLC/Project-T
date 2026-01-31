using UnityEngine;

namespace Script
{
    public class ReturnPoint : MonoBehaviour
    {
        Player p;
        
        void OnTriggerEnter2D(Collider2D collision)
        {
            Player player = collision.GetComponent<Player>();
            if (player == null || p != player)
                return;
            player.StopTime();
        }

        public void Init(Player player)
        {
            p = player;
            gameObject.SetActive(false);
        }
    }
}