using System;
using System.Collections;
using System.Linq;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardFunctions : MonoBehaviour
{
    public static void ChooseFromWholeGrid(Board board)
    {
        board.SetSelectionFilterAll();
    }

    public static void ChooseStartingPoints(Board board)
    {
        board.ClearSelectionFilter();
        board.AddSelectionFilter(new BoardTile[] { board.GetTileAt(-2, 0), board.GetTileAt(2, 0) });
    }

    public static void ChooseAllEmptyWithPoints(Board board, int points)
    {
        board.ClearSelectionFilter();
        board.AddSelectionFilter(board.m_Tiles.Values.Where(t => t.Points >= points && t.Piece.Empty));
    }

    public static void ChooseNothing(Board board)
    {
        board.ClearSelectionFilter();
    }

    public static void ChooseAroundPrevious(Board board)
    {
        board.FeatherSelectionFilter();
    }

    public static void TintGray(BoardTile tile)
    {
        tile.Tint(Color.gray);
    }

    public static void Untint(BoardTile tile)
    {
        tile.Untint();
    }

    public static void TintGreen(BoardTile tile)
    {
        tile.Tint(Color.green);
    }

    public static void ShowSelectionEdge(BoardTile tile)
    {
        tile.ShowSelectionEdge();
    }

    public static void HideSelectionEdge(BoardTile tile)
    {
        tile.HideSelectionEdge();
    }

    public static void ShowSelected(BoardTile tile)
    {
        tile.ShowSelected();
    }

    public void HideSelected(BoardTile tile)
    {
        tile.HideSelected();
    }

    public static void AnimateNewTargetsOfTile(BoardTile tile, CardDetails card)
    {
        GameManager.Instance.ClearSelectedCard();
        Destroy(GameObject.Instantiate(VFXRegistry.Instance.FriendlySummon, tile.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity), 2.0f);

        for (int i = 0; i < 49; i++)
        {
            if (tile.Piece.Effect[i])
            {
                var dx = i % 7 - 3;
                var dy = i / 7 - 3;
                var target = Board.Instance.GetAtOffset(tile, ((tile.Phase == TurnPhase.Left ? 1 : -1) * dx, -dy));
                if (target != null && target.Piece.Empty && target.gameObject.activeSelf)
                {
                    Destroy(GameObject.Instantiate(VFXRegistry.Instance.FriendlySummon, target.gameObject.transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity), 2.0f);
                    var value = target.Points + 1;
                    target.Points = value;
                }
            }
        }
    }
}
