using UnityEngine;

namespace Script.Item
{
    public class Item : InteractableObject
    {
        SpriteRenderer game_map_bound;// Kéo vùng Checkerboard vào
        SpriteRenderer spawn_bound;// Kéo vùng Trắng vào
        public static int itemCount;
        public const int maxItem = 8;
        public bool is_bird;
        public Vector2 GetRandomPositionOutside()
        {
            Bounds L = game_map_bound.bounds;
            Bounds W = spawn_bound.bounds;
            // Chia làm 4 vùng để chọn ngẫu nhiên
            // 0: Trái, 1: Phải, 2: Trên, 3: Dưới
            int zone = Random.Range(0, 4);
            Vector2 finalPos = Vector2.zero;

            switch (zone)
            {
                case 0: // Vùng bên trái vùng trắng
                    finalPos.x = Random.Range(L.min.x, W.min.x);
                    finalPos.y = Random.Range(L.min.y, L.max.y);
                    break;
                case 1: // Vùng bên phải vùng trắng
                    finalPos.x = Random.Range(W.max.x, L.max.x);
                    finalPos.y = Random.Range(L.min.y, L.max.y);
                    break;
                case 2: // Vùng phía trên vùng trắng (giới hạn chiều ngang để không lặp vùng 0,1)
                    finalPos.x = Random.Range(W.min.x, W.max.x);
                    finalPos.y = Random.Range(W.max.y, L.max.y);
                    break;
                case 3: // Vùng phía dưới vùng trắng
                    finalPos.x = Random.Range(W.min.x, W.max.x);
                    finalPos.y = Random.Range(L.min.y, W.min.y);
                    break;
            }

            return finalPos;
        }
        void Pick(Player p)
        {
            p.SetItem(this);
            DeleteInteraction(0);
            itemCount--;
            if (is_bird)
                p.sendFamilyToGM();
        }
        
        protected override void Start()
        {
            base.Start();
            AddInteraction("Pick",Pick);
            spawn_bound = GameManager.instance.spawn;
            game_map_bound  = GameManager.instance.game_map; 
            transform.position = GetRandomPositionOutside();
            itemCount++;
        }
        
        public void Rotate()
        {
            // Xoay quanh trục (0, 0, 1) một góc 90 độ
            transform.localRotation *= Quaternion.Euler(0, 0, 90);
            Debug.Log("ROTATE");
        }
    }
}