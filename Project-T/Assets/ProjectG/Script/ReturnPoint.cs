using DG.Tweening;
using UnityEngine;

namespace Script
{
    public class ReturnPoint : MonoBehaviour
    {
        Player p;
        public bool isNose;
        void OnTriggerEnter2D(Collider2D collision)
        {
            
            Player player = collision.GetComponent<Player>();
            if (player == null)
                return;
            if (!player.isHoldingItem())
                return;
            player.StopTime();
            player.transform.position = transform.position;
            if (isNose)
                player.RotateItem();
            Debug.Log($"is nose {isNose}");
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutBack).onComplete += () =>
            {
                gameObject.SetActive(false);
            };
          
        }

        public void Init(Player player)
        {
            p = player;
        }
    }
}