using System.Collections.Generic;

public interface IBoard
{
    public IEnumerable<BoardTile> GetPrimNeighbors(BoardTile tile);
    public IEnumerable<BoardTile> GetPrimNeighbors(int x, int y);
    public IEnumerable<BoardTile> GetNeighbors(BoardTile tile);
    public IEnumerable<BoardTile> GetNeighbors(int x, int y);
    public IEnumerable<BoardTile> GetNeighborhoods(IEnumerable<BoardTile> tiles);
    public IEnumerable<BoardTile> GetPrimNeighborhoods(IEnumerable<BoardTile> tiles);
    public BoardTile GetTileAt(int x, int y);
    public BoardTile GetAtOffset(BoardTile tile, (int, int) offset);
    public IEnumerable<BoardTile> GetAtOffsets(BoardTile tile, IEnumerable<(int, int)> offsets);
    public IEnumerable<BoardTile> GetAtOffsets(int x, int y, IEnumerable<(int, int)> offsets);
}
