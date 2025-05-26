using UnityEditor;

using UnityEngine;

[CreateAssetMenu]
public class Piece : ScriptableObject
{
    public Sprite Image;
    public int Cost = 1;
    public string Name;
    [TextArea]
    public string Description = "";
    [Space(10)]
    public Spaces Effect;
    public CardAbility[] Abilities;
}
