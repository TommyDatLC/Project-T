using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    [System.Serializable]
    public class SpawnItem
    {
        [Range(1, 100)] public float spawn_rate;
        public Item.Item spawnItem; 
    }

    public class Spawner : MonoBehaviour
    {
        [SerializeField] List<SpawnItem> spawn_items;

        /// <summary>
        /// Gọi hàm này từ bên ngoài để spawn số lượng item mong muốn.
        /// </summary>
        /// <param name="count">Số lượng item cần spawn một lúc.</param>
        public void Spawn(int count)
        {
            if (spawn_items == null || spawn_items.Count == 0) return;

            // 1. Tính tổng rate (Chỉ cần tính 1 lần ở đầu hàm để tối ưu)
            float totalRate = 0f;
            foreach (var item in spawn_items)
            {
                totalRate += item.spawn_rate;
            }

            // 2. Lặp lại việc spawn theo số lượng "count" yêu cầu
            for (int i = 0; i < count; i++)
            {
                SpawnSingleItem(totalRate);
            }
        }

        private void SpawnSingleItem(float totalRate)
        {
            float randomValue = Random.Range(0, totalRate);
            float currentRate = 0f;

            foreach (var item in spawn_items)
            {
                currentRate += item.spawn_rate;
                
                if (randomValue <= currentRate)
                {
                    if (item.spawnItem != null)
                    {
                        // Instantiate mà không truyền vị trí/xoay
                        // Logic vị trí sẽ do hàm Start() của Item tự xử lý
                        Instantiate(item.spawnItem);
                    }
                    return; // Đã spawn xong item này, thoát vòng lặp foreach
                }
            }
        }
    }
}