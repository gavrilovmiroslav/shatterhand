using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelSelection : MonoBehaviour
{
    private void OnMouseDown()
    {
        GameManager.Instance.CancelSelection();
    }
}
