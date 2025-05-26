using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameManager.Instance.HardEndTurn();
    }
}
