
using System.Collections;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Sustain Supported")]
public class Effect_SustainSupported : CardEffect
{
    public override IEnumerator Run(BoardTile tile)
    {
        Destroy(GameObject.Instantiate(VFXRegistry.Instance.Sustain, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < 49; i++)
        {
            if (tile.Piece.Effect[i])
            {
                var dx = i % 7 - 3;
                var dy = i / 7 - 3;
                var target = Board.Instance.GetAtOffset(tile, ((tile.Phase == TurnPhase.Left ? 1 : -1) * dx, -dy));
                if (target != null)
                {
                    target.BreakOn++;
                    Destroy(GameObject.Instantiate(VFXRegistry.Instance.Sustain, target.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
    }
}