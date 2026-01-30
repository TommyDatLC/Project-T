using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script.God
{
    public class GodObject : Interactable
    {
       [SerializeField]  List<int> productOfferRandomList = new List<int>();
        public int moneyForSucessful;

        private void Start()
        {
            AddInteraction("Offer",Offer);
        }

        public int RandomOfferList()
        {
            int randomOffer = productOfferRandomList[Random.Range(0, productOfferRandomList.Count)];
            return randomOffer;
        }
        public bool? AccptOffer = null;
        public async void Offer(Player p)
        {
           
            int offerProductInt = RandomOfferList();
            if (p.product - offerProductInt <= 0)
            {
                Debug.Log("Cannot Offer");
                return;
            }
            Debug.Log($"Offer cost {offerProductInt}");
            while (AccptOffer == null)
            {
                await Task.Delay(100);
            }

            if (AccptOffer == true)
            {
                p.product -= offerProductInt; 
                Debug.Log("offer sucessfully");
                p.money += moneyForSucessful;
            }
            else
            {
                Debug.Log("offer rejected");
            }

            AccptOffer = null;
        }
    }
}