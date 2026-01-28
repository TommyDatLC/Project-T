using System.Collections.Generic;
using UnityEngine;

namespace Script.God
{
    public class GodObject : Interactable
    {
       [SerializeField]  List<int> productOfferRandomList = new List<int>();
        public int moneyForSucessful;
        public int RandomOfferList()
        {
            int randomOffer = productOfferRandomList[Random.Range(0, productOfferRandomList.Count)];
            return randomOffer;
        }
        
    }
}