using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Events;

public class Board : MonoBehaviour, IBoard, IBoardTileSelection
{
    public static Board Instance;

    public Dictionary<(int, int), BoardTile> m_Tiles = new();

    public HashSet<BoardTile> m_SelectionFilter = new();
    public HashSet<BoardTile> m_SelectionContent = new();

    public UnityEvent<Board> OnBoardStart = new();

    public UnityEvent<BoardTile> OnTileSelected = new();
    public UnityEvent<BoardTile> OnTileDeselected = new();

    public UnityEvent<BoardTile> OnTileSetInFilter = new();
    public UnityEvent<BoardTile> OnTileUnsetInFilter = new();

    public void Awake()
    {
        Instance = this;
        foreach (var tile in this.GetComponentsInChildren<BoardTile>())
        {
            m_Tiles[tile.Coords] = tile;
            tile.Board = this;
        }
    }

    public void Start()
    {
        OnBoardStart?.Invoke(this);
    }

    public void UpdateValues(TurnPhase phase)
    {
        foreach (var tile in GetComponentsInChildren<BoardTile>())
        {
            tile.Points = 0;
        }

        foreach (var tile in GetComponentsInChildren<BoardTile>())
        {
            if (tile.Piece != null)
            {
                for (int i = 0; i < 49; i++)
                {
                    if (tile.Piece.Effect[i])
                    {
                        var dx = i % 7 - 3;
                        var dy = i / 7 - 3;
                        var target = this.GetAtOffset(tile, ((tile.Phase == TurnPhase.Left ? 1 : -1) * dx, -dy));
                        if (target != null)
                        {
                            if (tile.Phase == phase)
                            {
                                target.Points++;
                            }
                            else if (tile.Phase != phase)
                            {
                                target.Points--;
                            }
                        }
                    }
                }
            }
        }

        var sacrifice = (phase == TurnPhase.Left ? GameManager.Instance.LeftSacrifice : GameManager.Instance.RightSacrifice);
        foreach (var tile in GetComponentsInChildren<BoardTile>())
        {
            tile.Points += sacrifice;
        }
    }

    public BoardTile GetAtOffset(BoardTile tile, (int, int) offset)
    {
        var (x, y) = tile.Coords;
        return GetTileAt(x + offset.Item1, y + offset.Item2);
    }

    public IEnumerable<BoardTile> GetAtOffsets(BoardTile tile, IEnumerable<(int, int)> offsets)
    {
        var (x, y) = tile.Coords;
        return GetAtOffsets(x, y, offsets);
    }

    public IEnumerable<BoardTile> GetAtOffsets(int x, int y, IEnumerable<(int, int)> offsets)
    {
        foreach (var (dx, dy) in offsets)
        {
            yield return GetTileAt(x + dx, y + dy);
        }
    }

    public IEnumerable<BoardTile> GetNeighbors(BoardTile tile)
    {
        var (x, y) = tile.Coords;
        return GetNeighbors(x, y);
    }

    public IEnumerable<BoardTile> GetNeighbors(int x, int y)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0) continue;
                yield return GetTileAt(x + i, y + j);
            }
        }
    }

    public IEnumerable<BoardTile> GetNeighborhoods(IEnumerable<BoardTile> tiles)
    {
        var results = new HashSet<BoardTile>();
        foreach (var tile in tiles)
        {
            results.AddRange(GetNeighbors(tile));
        }

        return results;
    }

    public IEnumerable<BoardTile> GetPrimNeighborhoods(IEnumerable<BoardTile> tiles)
    {
        var results = new HashSet<BoardTile>();
        foreach (var tile in tiles)
        {
            results.AddRange(GetPrimNeighbors(tile));
        }

        return results;
    }

    public IEnumerable<BoardTile> GetPrimNeighbors(BoardTile tile)
    {
        var (x, y) = tile.Coords;
        return GetPrimNeighbors(x, y);
    }

    public IEnumerable<BoardTile> GetPrimNeighbors(int x, int y)
    {
        yield return GetTileAt(x - 1, y);
        yield return GetTileAt(x + 1, y);
        yield return GetTileAt(x, y - 1);
        yield return GetTileAt(x, y + 1);
    }

    public BoardTile GetTileAt(int x, int y)
    {
        if (m_Tiles.TryGetValue((x, y), out var tile))
        {
            return tile;
        }

        return null;
    }

    public void AddSelection(BoardTile tile)
    {
        if (m_SelectionFilter.Contains(tile))
        {
            m_SelectionContent.Add(tile);
            OnTileSelected?.Invoke(tile);
        }
    }

    public void RemoveSelection(BoardTile tile)
    {
        if (m_SelectionFilter.Contains(tile))
        {
            m_SelectionContent.Remove(tile);
            OnTileDeselected?.Invoke(tile);
        }
    }

    public void ClearSelection()
    {
        foreach (var tile in m_SelectionContent)
        {
            OnTileDeselected?.Invoke(tile);
        }

        m_SelectionContent.Clear();
    }

    public bool SelectionContains(BoardTile tile)
    {
        return m_SelectionContent.Contains(tile);
    }

    public void ToggleSelection(BoardTile tile)
    {
        if (SelectionContains(tile))
        {
            RemoveSelection(tile);
        }
        else
        {
            AddSelection(tile);
        }
    }

    public void SetSelectionFilterAll()
    {
        SetSelectionFilter(m_Tiles.Values);
    }

    public void SetSelectionFilter(IEnumerable<BoardTile> tiles)
    {
        foreach (var tile in m_SelectionFilter)
        {
            OnTileUnsetInFilter?.Invoke(tile);
        }

        m_SelectionFilter.Clear();
        m_SelectionFilter.AddRange(tiles);

        foreach (var tile in m_SelectionFilter)
        {
            OnTileSetInFilter?.Invoke(tile);
        }
    }

    public static void CancelPutUnit()
    {
        Board.Instance.OnTileSelected.RemoveAllListeners();
        Board.Instance.OnTileSelected.AddListener(BoardFunctions.ShowSelected);
    }

    public void AddSelectionFilter(IEnumerable<BoardTile> tiles)
    {
        m_SelectionFilter.AddRange(tiles);

        foreach (var tile in m_SelectionFilter)
        {
            OnTileSetInFilter?.Invoke(tile);
        }
    }

    public void RemoveSelectionFilter(IEnumerable<BoardTile> tiles)
    {
        foreach (var tile in tiles)
        {
            OnTileUnsetInFilter?.Invoke(tile);
            m_SelectionFilter.Remove(tile);
        }
    }

    public void ClearSelectionFilter()
    {
        foreach (var tile in m_SelectionFilter)
        {
            OnTileUnsetInFilter?.Invoke(tile);
        }

        m_SelectionFilter.Clear();
    }

    public void FeatherSelectionFilter()
    {
        AddSelectionFilter(GetNeighborhoods(m_SelectionFilter));
    }
}
