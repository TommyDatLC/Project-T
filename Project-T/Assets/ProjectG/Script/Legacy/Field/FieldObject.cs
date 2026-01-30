using System.Threading.Tasks;
using UnityEngine;

namespace Script.Field
{
    public class FieldObject : Interactable
    {
        
        public enum FieldStateEnum
        {
            EMPTY = 0,
            Planted = 1,
            Ripe = 2
        }
        FieldStateEnum state = FieldStateEnum.EMPTY;
        [SerializeField] Sprite empty_sprite,planted_sprite,ripe_sprite;
     
        [SerializeField] float wait_planted_2_ripe_time_sec = 30;

        public FieldStateEnum GetState()
        {
            return state;
        }
        public void SetState(FieldStateEnum newState)
        {
            state = newState;
            switch (state)
            {
                case FieldStateEnum.EMPTY:
                    ChangeSpriteWithAnimation(empty_sprite);
                    break;
                case FieldStateEnum.Planted:
                    ChangeSpriteWithAnimation(planted_sprite);
                    onPlant();
                    break;
                case FieldStateEnum.Ripe:
                    ChangeSpriteWithAnimation(ripe_sprite);
                    onRipe();
                    break;
            }
        }

        private void onRipe()
        {
            EditInteraction(0,"Harvest",Harvest);
        }
        async void onPlant()
        {
            // Đợi hạt , disable nút bấm bởi không thể tương tác được
            EditInteraction(0,"Waiting for ripe",null);
            await Task.Delay((int)(wait_planted_2_ripe_time_sec * 1000));
            SetState(FieldStateEnum.Ripe);
        }
 
        
    
        public void Plant(Player player)
        {
            if (player.seed <= 0)
            {
                Debug.Log("Khong du hat de trong");
                return;
            }
            if (GetState() != FieldStateEnum.EMPTY)
            {
                Debug.Log("Cannot Plant field that not EMPTY:");
                return;
            }
            player.seed--;
            // neu tim thay field object
            SetState(FieldObject.FieldStateEnum.Planted);
        }

        public void Harvest(Player player)
        {
            if (GetState() != FieldObject.FieldStateEnum.Ripe)
            {
                Debug.Log("Cannot harvest field that not ripe:");
                return;
            }
            Debug.Log("Harvest sucessfullly: " + name);
            player.seed += player.seed_inc;
            player.product += player.product_inc;
            SetState(FieldObject.FieldStateEnum.EMPTY);
        }

        void Switcher(Player p)
        {
            Plant(p);
            Harvest(p);
        }
        protected override void Start()
        {
            base.Start();
            AddInteraction("Plant",Plant);
        }

    }
}