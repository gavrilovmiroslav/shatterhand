
using System.Collections;

using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Effects/Sustain")]
public class Effect_Sustain : CardEffect
{
    public override IEnumerator Run(BoardTile tile)
    {
        tile.BreakOn++;
        Destroy(GameObject.Instantiate(VFXRegistry.Instance.Sustain, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.Euler(90, 0, 0)), 0.5f);
        yield return new WaitForSeconds(0.5f);
    }
}

