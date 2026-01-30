using UnityEngine;
namespace ProjectG.Script
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector2 minBounds = new Vector2(-2880, -1620); // Half of 5760, 3240
        public Vector2 maxBounds = new Vector2(2880, 1620);

        void LateUpdate()
        {
            if (target == null) return;
        
            float posX = Mathf.Clamp(target.position.x, minBounds.x + 960, maxBounds.x - 960);
            float posY = Mathf.Clamp(target.position.y, minBounds.y + 540, maxBounds.y - 540);
        
            transform.position = new Vector3(posX, posY, -10);
        }
    }
}