using System.Collections;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private ParticleSystem loseVFX_1;
    [SerializeField] private ParticleSystem loseVFX_2;

    private WagonData data;
    private float elapsedTime = 0f;
    private int currentIndex = 0;

    private bool hasTriggered = false;

    private bool isPlaying = false;

    public void Create(WagonData recordedData)
    {
        data = recordedData;
    }

    public void Spawn()
    {
        elapsedTime = 0f;
        currentIndex = 0;
        WagonData.PathPoint step = data.paths[currentIndex];
        transform.SetLocalPositionAndRotation(step.position, step.rotation);
    }

    public void Play()
    {
        isPlaying = true;
    }

    public void Stop()
    {
        isPlaying = false;
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
            transform.SetLocalPositionAndRotation(step.position, step.rotation);

            if (elapsedTime > data.paths[data.paths.Count - 1].elapsedTime)
            {
                elapsedTime = 0f;
                currentIndex = 0;
                isPlaying = false;
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player") && isPlaying)
        {
            hasTriggered = true;
            StartCoroutine(LoseSequence());
        }
    }

    private IEnumerator LoseSequence()
    {
        Stop();
        
        loseVFX_1.Play();
        loseVFX_2.Play();

        yield return new WaitForSeconds(0.5f);

        GameManager.Instance.SetState(GameState.Lose);
        GhostManager.Instance.DestroyGhost(gameObject);
    }

    public Station GetStartingStation()
    {
        return data.startingStation;
    }
}
