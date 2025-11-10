using UnityEngine;

public class GhostController : MonoBehaviour
{
    private WagonPathing data;
    private float elapsedTime = 0f;
    private int currentIndex = 0;

    public void Play(WagonPathing recordedData)
    {
        data = recordedData;
    }

    private void FixedUpdate()
    {
        if (data == null || data.paths.Count == 0) return;

        elapsedTime += Time.fixedDeltaTime;

        while (currentIndex < data.paths.Count - 1 && elapsedTime > data.paths[currentIndex + 1].elapsedTime)
        {
            currentIndex++;
        }

        WagonPathing.PathPoint step = data.paths[currentIndex];
        transform.SetPositionAndRotation(step.position, step.rotation);

        if (elapsedTime > data.paths[data.paths.Count - 1].elapsedTime)
        {
            elapsedTime = 0f;
            currentIndex = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetState(GameState.Lose);
            GhostManager.Instance.DestroyGhost(gameObject);
        }
        else if (other.CompareTag("Ghost"))
        {
            GhostManager.Instance.DestroyGhost(gameObject);
        }
    }
}
