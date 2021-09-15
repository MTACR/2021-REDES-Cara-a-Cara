﻿using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Cards
{
    public class Deck : MonoBehaviour
    {
        public CardModel[] models;
        public GameObject prefab;
        private Card[] cards;

        private void Start()
        {
            Shuffle(models);
            cards = new Card[models.Length];
            int i = 0;
            int j = 0;
            foreach (var c in models)
            {
                Card card = Instantiate(prefab, transform).GetComponent<Card>();
                card.Setup(c, i);
                card.transform.position += new Vector3((i % 7f) * 11f, j * 15f, 0f);
                cards[i++] = card;

                if (i % 7 == 0)
                    j++;
            }
        }
        
        private static void Shuffle(IList<CardModel> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                CardModel tmp = array[i];
                int r = Random.Range(i, array.Count);
                array[i] = array[r];
                array[r] = tmp;
            }
        }

        public void FlipAll()
        {
            foreach (var card in cards)
            {
                card.Flip();
            }
        }
        
    }
}