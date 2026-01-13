using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource calmSource;
    [SerializeField] private AudioSource intenseSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip calmMusic;
    [SerializeField] private AudioClip intenseMusic;
    [SerializeField] private AudioClip stationJingle;
    [SerializeField] private AudioClip winJingle;
    [SerializeField] private AudioClip loseJingle;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private Coroutine transitionRoutine;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

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

    private void Start()
    {
        calmSource.clip = calmMusic;
        intenseSource.clip = intenseMusic;

        calmSource.loop = true;
        intenseSource.loop = true;

        calmSource.volume = musicVolume;
        intenseSource.volume = 0f;

        calmSource.Play();
        intenseSource.Play();
    }

    private void Update()
    {
        //Debug.Log($"Calm Volume: {calmSource.volume}, Intense Volume: {intenseSource.volume}");
    }

    public void SetIntensity(bool intense)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(CrossfadeIntensity(intense));
    }

    private IEnumerator CrossfadeIntensity(bool toIntense)
    {
        float duration = 1f;
        float timer = 0f;

        float startCalmVol = calmSource.volume;
        float startIntenseVol = intenseSource.volume;

        float targetCalmVol = toIntense ? 0f : musicVolume;
        float targetIntenseVol = toIntense ? musicVolume : 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            calmSource.volume = Mathf.Lerp(startCalmVol, targetCalmVol, t);
            intenseSource.volume = Mathf.Lerp(startIntenseVol, targetIntenseVol, t);

            yield return null;
        }

        calmSource.volume = targetCalmVol;
        intenseSource.volume = targetIntenseVol;

        transitionRoutine = null;
    }

    public void PlayStationJingle() => sfxSource.PlayOneShot(stationJingle, sfxVolume);
    public void PlayWinJingle() => sfxSource.PlayOneShot(winJingle, sfxVolume);
    public void PlayLoseJingle() => sfxSource.PlayOneShot(loseJingle, sfxVolume);

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (calmSource.volume > 0) calmSource.volume = musicVolume;
        if (intenseSource.volume > 0) intenseSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
    }

    public void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Title:
                SetIntensity(false);
                break;
            case GameState.Gameplay:
                SetIntensity(true);
                break;
            case GameState.Win:
                PlayWinJingle();
                break;
            case GameState.Lose:
                PlayLoseJingle();
                break;
        }
    }
}
