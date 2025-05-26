using System.Collections;

using UnityEngine;

public class CardEffect : ScriptableObject
{
    public virtual IEnumerator Run(BoardTile tile) 
    {
        yield return null;
    }
}
