using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public List<Piece> Cards = new();
    public List<Piece> Discard = new();
    private static readonly int CARD_COUNT = 3;

    public void Load(Deck deck)
    {
        Discard.AddRange(deck.Units);
        GameManager.ShuffleDeck(ref Discard);
        GameManager.Transfer(ref Discard, ref Cards, CARD_COUNT);
        UpdateUI();
    }

    public void DiscardCard(int index)
    {
        if (Cards.Count > 0)
        {
            var piece = Cards[index];
            Cards.RemoveAt(index);
            Discard.Add(piece);
        }
    }

    public void FillHand()
    {
        if (Cards.Count < CARD_COUNT)
        {
            GameManager.Transfer(ref Discard, ref Cards, CARD_COUNT - Cards.Count);
            for (int i = 0; i < Cards.Count; i++)
            {
                var card = this.transform.GetChild(i).GetComponent<CardDetails>();
                card.gameObject.SetActive(true);
                card.Piece.Set(Cards[i]);
            }
        }
    }

    public void UpdateUI()
    {
        for (int i = 0; i < CARD_COUNT; i++)
        {
            if (Cards[i] != null)
            {
                var t = this.transform.GetChild(i).gameObject;
                t.SetActive(true);
                t.GetComponent<CardDetails>().Piece.Set(Cards[i]);
            }
            else
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHandAfterUsing(CardDetails card)
    {
        DiscardCard(card.Index);
        card.gameObject.SetActive(false);
        this.FillHand();
        GameManager.Instance.TurnPhaseDone();
    }
}
