
using UnityEngine;

public class PieceInstance
{
    public bool Empty = true;
    public Sprite Image;
    public int Cost = 1;
    public string Name;
    public string Description = "";
    public Spaces Effect = new();
    public CardAbility[] Abilities;

    public void Set(Piece p)
    {
        if (p != null)
        {
            Empty = false;
            Image = p.Image;
            Cost = p.Cost;
            Name = p.Name;
            Description = p.Description;
            Effect.Assign(p.Effect);
            Abilities = new CardAbility[p.Abilities.Length];
            for (int i = 0; i < p.Abilities.Length; i++) Abilities[i] = p.Abilities[i];
        }
        else
        {
            Empty = true;
            Image = null;
            Cost = 0;
            Name = "";
            Description = "";
            Effect.Reset();
            Abilities = null;
        }
    }

    public void Set(PieceInstance p)
    {
        if (p != null)
        {
            Empty = false;
            Image = p.Image;
            Cost = p.Cost;
            Name = p.Name;
            Description = p.Description;
            Effect.Assign(p.Effect);
            Abilities = new CardAbility[p.Abilities.Length];
            for (int i = 0; i < p.Abilities.Length; i++) Abilities[i] = p.Abilities[i];
        }
        else
        {
            Empty = true;
            Image = null;
            Cost = 0;
            Name = "";
            Description = "";
            Effect.Reset();
            Abilities = null;
        }
    }
}

