using System.Collections.Generic;
using Texell.CandyCoolSummer;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnCandy : MonoBehaviour
{
    public GameObject[] CandiesForSpawn;

    public Tilemap TileBoard;
    /// <summary>
    /// The area covered by tiles.
    /// </summary>
    public BoundsInt Bounds;

    /// <summary>
    /// Because of the TileAnchor set to (0.5f, 0.5f), we use Offset value to position Candies 
    /// exactly.
    /// </summary>
    public Vector2 Offset = new(0.5f, 0.5f);

    private readonly List<GameObject> _hintCandies = new();
    private readonly Dictionary<CandyType, Candy> _candyLookup = new();

    void Start()
    {
        BoundsInt Bounds = TileBoard.cellBounds;
        Debug.Log("Hello");
        int x = 0;
        foreach (var pos in Bounds.allPositionsWithin)
        {
            if (TileBoard.HasTile(pos))
            {
                x++;
            }
        }
        Debug.Log("x = " + x);
        //Fill a lookup of candy type to candy.
        foreach (var candy in CandiesForSpawn)
        {
            var temp = candy.GetComponent<Candy>();
            _candyLookup.Add(temp.Type, temp);
        }
    }

    /// <summary>
    /// Spawn a candy in every cell, making sure we don't have any match.
    /// </summary>
    public void Spawn()
    {

    }
}
