using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [Tooltip("Audio source for background music.")]
    [SerializeField] private AudioSource musicAudio;

    [Tooltip("Audio source for fruit collection sound.")]
    [SerializeField] private AudioSource fruitAudio;

    [Tooltip("Audio source for damage sound.")]
    [SerializeField] private AudioSource damageAudio;

    private void Awake()
    {
        //In case there was a different Instance created, is destroyed and a new one is created
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Sets the volume of the background music (0 to 1).
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicAudio.volume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(GameConstants.MUSICVOLUME_KEY, musicAudio.volume); // Saves the value
    }

    /// <summary>
    /// Loads the saved volume.
    /// </summary>
    public void LoadVolume()
    {
        if (PlayerPrefs.HasKey(GameConstants.MUSICVOLUME_KEY))
        {
            musicAudio.volume = PlayerPrefs.GetFloat(GameConstants.MUSICVOLUME_KEY);
        }
    }

    /// <summary>
    /// Loads and plays the fruitAudio.
    /// </summary>
    public void FruitSound() => fruitAudio.Play();

    /// <summary>
    /// Loads and plays the damageAudio.
    /// </summary>
    public void PlayDamageSound() => damageAudio.Play();
}
