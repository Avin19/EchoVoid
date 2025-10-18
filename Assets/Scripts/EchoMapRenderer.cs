using UnityEngine;

public class EchoMapRenderer : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject goalPrefab;
    public Transform mapParent;

    public void Render(EchoMap map)
    {
        foreach (Transform child in mapParent)
            Destroy(child.gameObject);

        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                switch (map.tiles[x, y])
                {
                    case TileType.Wall:
                        Instantiate(wallPrefab, pos, Quaternion.identity, mapParent);
                        break;
                    case TileType.Goal:
                        Instantiate(goalPrefab, pos, Quaternion.identity, mapParent);
                        break;
                }
            }
        }
    }
}
