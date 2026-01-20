using UnityEngine;
using UnityEngine.Tilemaps;

public class PerlinGroundGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public RuleTile groundTile;

    public int width = 100;
    public int maxHeight = 10;
    public float noiseScale = 0.1f;

    public int seed = 0;

    void Start()
    {
        if (seed == 0)
            seed = Random.Range(0, 10000);

        Generate();
    }

    void Generate()
    {
        for (int x = 0; x < width; x++)
        {
            float noiseValue = Mathf.PerlinNoise(
                (x + seed) * noiseScale,
                0f
            );

            int height = Mathf.RoundToInt(noiseValue * maxHeight);

            for (int y = 0; y <= height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
            }
        }
    }
}
