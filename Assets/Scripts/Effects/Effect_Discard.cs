
using System.Collections;

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Discard")]
public class Effect_Discard : CardEffect
{
    public override IEnumerator Run(BoardTile tile)
    {
        Destroy(GameObject.Instantiate(VFXRegistry.Instance.Discard, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity), 2.0f);
        if (tile.Phase == TurnPhase.Left)
        {
            GameManager.Instance.LeftPlayerHand.DiscardCard(0);
            GameManager.Instance.LeftPlayerHand.FillHand();
        }
        else if (tile.Phase == TurnPhase.Right)
        {
            GameManager.Instance.RightPlayerHand.DiscardCard(0);
            GameManager.Instance.RightPlayerHand.FillHand();
        }

        yield return new WaitForSeconds(0.5f);
    }
}

