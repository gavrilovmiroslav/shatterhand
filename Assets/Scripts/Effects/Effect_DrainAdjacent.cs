
using System.Collections;
using System.Linq;

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Drain Adjacent")]
public class Effect_DrainAdjacent : CardEffect
{
    public override IEnumerator Run(BoardTile tile)
    {
        var neighbors = Board.Instance.GetNeighbors(tile).Where(n => n != null && !n.Piece.Empty && n.Phase != tile.Phase).ToArray();
        bool anyNeighbors = neighbors.Any();
        
        if (anyNeighbors)
        {
            Destroy(GameObject.Instantiate(VFXRegistry.Instance.Debuff, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
            yield return new WaitForSeconds(0.5f);
            foreach (var n in neighbors)
            {
                n.Piece.Cost--;
                Destroy(GameObject.Instantiate(VFXRegistry.Instance.Shrink, n.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}

