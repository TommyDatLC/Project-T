using System.Threading.Tasks;
using UnityEngine;

namespace Script.Field
{
    public class FieldObject : MonoBehaviour
    {
        public enum FieldStateEnum
        {
            EMPTY = 0,
            Planted = 1,
            Ripe = 2
        }
        FieldStateEnum state = FieldStateEnum.EMPTY;
        [SerializeField] Sprite empty_sprite,planted_sprite,ripe_sprite;
        private SpriteRenderer this_sprite_render;
        [SerializeField] float wait_co_hat_2_chin_time_sec = 30;
        
        public void SetState(FieldStateEnum newState)
        {
            state = newState;
            switch (state)
            {
                case FieldStateEnum.EMPTY:
                    this_sprite_render.sprite = empty_sprite;
                    break;
                case FieldStateEnum.Planted:
                    this_sprite_render.sprite = planted_sprite;
                    onCoHat();
                    break;
                case FieldStateEnum.Ripe:
                    this_sprite_render.sprite = ripe_sprite;
                    break;
            }
        }

        void Start()
        {
            this_sprite_render = GetComponent<SpriteRenderer>();
        }
        
        async void onCoHat()
        {
            await Task.Delay((int)(wait_co_hat_2_chin_time_sec * 1000));
            SetState(FieldStateEnum.Ripe);
        }
        public void Update()
        {
            
        }
    }
}