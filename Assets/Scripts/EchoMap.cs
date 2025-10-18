public enum TileType
{
    Empty = 0,
    Wall = 1,
    Goal = 2
}

public class EchoMap
{
    public TileType[,] tiles;
    public int width, height;

    public EchoMap(int w, int h)
    {
        width = w;
        height = h;
        tiles = new TileType[w, h];
    }
}
