
using System.Collections;

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Grow")]
public class Effect_Grow : CardEffect
{
    public override IEnumerator Run(BoardTile tile)
    {
        tile.Piece.Cost++;
        Destroy(GameObject.Instantiate(VFXRegistry.Instance.Grow, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
}

