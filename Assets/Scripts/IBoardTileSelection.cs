using System.Collections.Generic;

public interface IBoardTileSelection
{
    public void AddSelection(BoardTile tile);
    public void RemoveSelection(BoardTile tile);
    public void ClearSelection();
    public bool SelectionContains(BoardTile tile);
    public void ToggleSelection(BoardTile tile);

    public void AddSelectionFilter(IEnumerable<BoardTile> tiles);
    public void SetSelectionFilter(IEnumerable<BoardTile> tiles);
    public void RemoveSelectionFilter(IEnumerable<BoardTile> tiles);
    public void ClearSelectionFilter();
    public void FeatherSelectionFilter();
}
