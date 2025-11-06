using System.Collections.Generic;
using UnityEngine;

public class WagonData
{
    public string name;
    public int model;
    public Color color;
    public List<Vector3> path;
}

public class OldWagonManager : MonoBehaviour
{
    public static OldWagonManager Instance { get; private set; }

    [Header("Wagon Settings")]
    [SerializeField, Range(0, 30)] int numRemainingWagons;

    private Queue<WagonData> lastWagons;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        lastWagons = new Queue<WagonData>(numRemainingWagons);
    }

    public void AddTrain(WagonData wagon)
    {
        if (lastWagons.Count >= numRemainingWagons)
        {
            WagonData oldWagon = lastWagons.Dequeue();
            DestroyOldTrain(oldWagon);
        }

        lastWagons.Enqueue(wagon);
        SpawnReplayTrain(wagon);
    }

    private void DestroyOldTrain(WagonData wagon)
    {
        // supprime le GameObject correspondant au train
    }

    private void SpawnReplayTrain(WagonData wagon)
    {
        // instancie le prefab train
        // lui fait suivre train.path en boucle
    }
}
