using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform ghostParent;
    [SerializeField] private int maxGhosts = 10;

    private Queue<GameObject> ghosts = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnGhost(WagonPathing data)
    {
        if (ghosts.Count >= maxGhosts)
        {
            GameObject oldGhost = ghosts.Dequeue();
            Destroy(oldGhost);
        }

        GameObject ghost = Instantiate(ghostPrefab, ghostParent);
        ghost.GetComponent<GhostController>().Play(data);
        ghosts.Enqueue(ghost);
    }
}
