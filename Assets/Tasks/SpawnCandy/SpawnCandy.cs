using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnCandy : MonoBehaviour
{
    public GameObject BlueCandyPrefab;
    public GameObject YellowCandyPrefab;
    public GameObject RedCandyPrefab;
    public GameObject GreenCandyPrefab;
    public GameObject PurpleCandyPrefab;
    public GameObject PinkCandyPrefab;

    public Tilemap TileBoard;
    /// <summary>
    /// The area covered by tiles.
    /// </summary>
    public BoundsInt bounds;

    /// <summary>
    /// Because of the TileAnchor set to (0.5f, 0.5f), we use Offset value to position Candies 
    /// exactly.
    /// </summary>
    public Vector2 Offset = new(0.5f, 0.5f);

    void Start()
    {
        BoundsInt bounds = TileBoard.cellBounds;
        Debug.Log("Hello");
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (TileBoard.HasTile(pos))
            {
                GameObject go = Instantiate(BlueCandyPrefab);
                go.transform.position = new Vector2(pos.x + Offset.x, pos.y + Offset.y);

            }
        }
    }

}
