using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform ghostParent;

    private int maxGhosts;

    private readonly Queue<GameObject> ghosts = new();

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

    public void SetMaxGhosts(int max)
    {
        maxGhosts = max;
    }

    public void CreateGhost(WagonData data)
    {
        if (ghosts.Count >= maxGhosts)
        {
            GameObject oldGhost = ghosts.Dequeue();
            if (oldGhost != null)
            {
                Destroy(oldGhost);
            }
        }

        GameObject ghost = Instantiate(ghostPrefab, ghostParent);

        ghost.GetComponent<GhostController>().Create(data);

        ghost.GetComponent<GhostCustomize>().SetText(data.wagonName);
        ghost.GetComponent<GhostCustomize>().SetMaterial(data.wagonMaterial);
        ghost.GetComponent<GhostCustomize>().SetWagonIndex(data.wagonMeshIndex);

        ghost.GetComponent<GhostCustomize>().SetCustomization();

        ghosts.Enqueue(ghost);

        ghost.SetActive(false);
    }
    
    public void SpawnGhosts()
    {
        foreach (var ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.SetActive(true);
                ghost.GetComponent<GhostController>().Spawn();
            }
        }
    }

    public void LaunchGhosts()
    {
        foreach (var ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.GetComponent<GhostController>().Play();
            }
        }
    }

    public void StopGhosts()
    {
        foreach (var ghost in ghosts)
        {
            if (ghost != null)
            {
                ghost.GetComponent<GhostController>().Stop();
            }
        }
    }

    public void DestroyLastGhostIfFull()
    {
        if (ghosts.Count == 0) return;
        if (ghosts.Count < maxGhosts) return;

        GameObject ghost = ghosts.Dequeue();
        if (ghost != null)
        {
            GameManager.Instance.ReactiveStartingStations(ghost.GetComponent<GhostController>().GetStartingStation());
            Destroy(ghost);
        }
    }

    public void DestroyGhost(GameObject ghost)
    {
        if (ghost == null) return;

        if (ghosts.Contains(ghost))
        {
            List<GameObject> temp = new(ghosts);
            temp.Remove(ghost);
            ghosts.Clear();
            foreach (var g in temp)
            {
                ghosts.Enqueue(g);
            }
        }

        Destroy(ghost);
    }
}
