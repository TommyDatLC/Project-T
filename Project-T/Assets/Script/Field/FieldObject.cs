using System.Threading.Tasks;
using UnityEngine;

namespace Script.Field
{
    public class FieldObject : MonoBehaviour
    {
        public enum FieldStateEnum
        {
            EMPTY = 0,
            CO_HAT = 1,
            CHIN = 2
        }
        FieldStateEnum state = FieldStateEnum.EMPTY;
        [SerializeField] Sprite empty,co_hat,chin;
        private SpriteRenderer thisSpriteRender;
        [SerializeField] float wait_co_hat_2_chin_time_sec = 30;
        
        public void SetState(FieldStateEnum newState)
        {
            state = newState;
            switch (state)
            {
                case FieldStateEnum.EMPTY:
                    thisSpriteRender.sprite = empty;
                    break;
                case FieldStateEnum.CO_HAT:
                    thisSpriteRender.sprite = co_hat;
                    onCoHat();
                    break;
                case FieldStateEnum.CHIN:
                    thisSpriteRender.sprite = chin;
                    break;
            }
        }

        void Start()
        {
            thisSpriteRender = GetComponent<SpriteRenderer>();
        }
        
        async void onCoHat()
        {
            await Task.Delay((int)(wait_co_hat_2_chin_time_sec * 1000));
            SetState(FieldStateEnum.CHIN);
        }
        public void Update()
        {
            
        }
    }
}