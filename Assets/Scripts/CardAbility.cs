using UnityEngine;

[System.Serializable]
public class CardAbility
{
    [SerializeField]
    public CardEffectTrigger Trigger;

    [SerializeField]
    public CardEffect Effect;
}
