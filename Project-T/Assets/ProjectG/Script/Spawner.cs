using System.Collections.Generic;
using UnityEngine;

namespace Script
{
    [System.Serializable]
    public class SpawnItem
    {
        [Range(1, 100)] public float spawn_rate;
        public GameObject spawnItem; // Changed to GameObject for standard Instantiate
    }

    public class Spawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] List<SpawnItem> spawn_items;
        
        [Header("Debug & Clamping Info")]
        [SerializeField] private BoxCollider2D mapCollider; // Drag your 'sprite_Checker_0_map' here

        public void Spawn(int count)
        {
            // Debug Map Size
            if (mapCollider != null)
            {
                Debug.Log($"<color=cyan>Map Bounds Check:</color> Size: {mapCollider.size} | " +
                          $"Min: {mapCollider.bounds.min} | Max: {mapCollider.bounds.max}");
            }
            else
            {
                Debug.LogWarning("Spawner: No Map Collider assigned for debugging!");
            }

            if (spawn_items == null || spawn_items.Count == 0) return;

            float totalRate = 0f;
            foreach (var item in spawn_items)
            {
                totalRate += item.spawn_rate;
            }

            for (int i = 0; i < count; i++)
            {
                SpawnSingleItem(totalRate, i);
            }
        }

        private void SpawnSingleItem(float totalRate, int index)
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
                        GameObject spawned = Instantiate(item.spawnItem);
                        // Debug Item Generation
                        Debug.Log($"<color=yellow>Spawned Item {index}:</color> {spawned.name} at Position: {spawned.transform.position}");
                    }
                    return; 
                }
            }
        }

        // Draw the map area in the Scene View so you can see if the collider is actually covering the art
        private void OnDrawGizmos()
        {
            if (mapCollider != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(mapCollider.bounds.center, mapCollider.bounds.size);
            }
        }
    }
}