
using System.Collections;

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Gift")]
public class Effect_Gift : CardEffect
{
    public override IEnumerator Run(BoardTile tile)
    {
        if (tile.Phase == TurnPhase.Left)
        {
            GameManager.Instance.LeftSacrifice++;
        }
        else if (tile.Phase == TurnPhase.Right)
        {
            GameManager.Instance.RightSacrifice++;
        }

        Destroy(GameObject.Instantiate(VFXRegistry.Instance.Gift, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
}

