using UnityEngine;

public class GhostController : MonoBehaviour
{
    private WagonData data;
    private float elapsedTime = 0f;
    private int currentIndex = 0;

    private bool isPlaying = false;

    public void Create(WagonData recordedData)
    {
        data = recordedData;
    }

    public void Play()
    {
        elapsedTime = 0f;
        currentIndex = 0;
        isPlaying = true;
    }

    private void FixedUpdate()
    {
        if (isPlaying)
        {
            if (data == null || data.paths.Count == 0) return;

            elapsedTime += Time.fixedDeltaTime;

            while (currentIndex < data.paths.Count - 1 && elapsedTime > data.paths[currentIndex + 1].elapsedTime)
            {
                currentIndex++;
            }

            WagonData.PathPoint step = data.paths[currentIndex];
            transform.SetPositionAndRotation(step.position, step.rotation);

            if (elapsedTime > data.paths[data.paths.Count - 1].elapsedTime)
            {
                elapsedTime = 0f;
                currentIndex = 0;
                isPlaying = false;
                GetComponent<Renderer>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isPlaying == true)
        {
            GameManager.Instance.SetState(GameState.Lose);
            GhostManager.Instance.DestroyGhost(gameObject);
        }
    }

    public Station GetStartingStation()
    {
        return data.startingStation;
    }
}
