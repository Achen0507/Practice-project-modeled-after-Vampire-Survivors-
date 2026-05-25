using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Œ‰∆˜π•ª˜“Ù–ß")]
    [SerializeField] private AudioClip knifeAttack;
    [SerializeField] private AudioClip orbitAttack;
    [SerializeField] private AudioClip heavenStrikeAttack;
    [SerializeField] private AudioClip whipAttack;
    [SerializeField] private AudioClip fireWandAttack;

    [Header("ÕÊº““Ù–ß")]
    [SerializeField] private AudioClip playerHurt;
    [SerializeField] private AudioClip playerDeath;

    [Header(" ∞»°“Ù–ß")]
    [SerializeField] private AudioClip expPickup;
    [SerializeField] private AudioClip coinPickup;
    [SerializeField] private AudioClip healthPickup;

    [Header("UI“Ù–ß")]
    [SerializeField] private AudioClip uiClick;
    [SerializeField] private AudioClip uiError;
    [SerializeField] private AudioClip upgradeSelect;
    [SerializeField] private AudioClip achievementUnlock;

    [Header("±≥æ∞“Ù¿÷")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip defeatMusic;

    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        LoadVolumes();
    }

    private void LoadVolumes()
    {
        float sfxVol = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.7f);
        float musicVol = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
        SetSFXVolume(sfxVol);
        SetMusicVolume(musicVol);
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxSource.volume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicSource.volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
        SaveVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
        SaveVolumes();
    }

    public void PlaySound(AudioClip clip, float volume = 0.7f) {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    // Œ‰∆˜π•ª˜
    public void PlayKnifeAttack() => PlaySound(knifeAttack);
    public void PlayOrbitAttack() => PlaySound(orbitAttack);
    public void PlayHeavenStrikeAttack() => PlaySound(heavenStrikeAttack);
    public void PlayWhipAttack() => PlaySound(whipAttack);
    public void PlayFireWandAttack() => PlaySound(fireWandAttack);

    // ÕÊº“
    public void PlayPlayerHurt() => PlaySound(playerHurt);
    public void PlayPlayerDeath() => PlaySound(playerDeath);

    //  ∞»°
    public void PlayExpPickup() => PlaySound(expPickup);
    public void PlayCoinPickup() => PlaySound(coinPickup);
    public void PlayHealthPickup() => PlaySound(healthPickup);

    // UI
    public void PlayUIClick() => PlaySound(uiClick);
    public void PlayUIError() => PlaySound(uiError);
    public void PlayUpgradeSelect() => PlaySound(upgradeSelect);
    public void PlayAchievementUnlock() => PlaySound(achievementUnlock);

    // ±≥æ∞“Ù¿÷
    public void PlayMainMenuMusic() => PlayMusic(mainMenuMusic);
    public void PlayGameMusic() => PlayMusic(gameMusic);
    public void PlayBossMusic() => PlayMusic(bossMusic);
    public void PlayVictoryMusic() => PlayMusic(victoryMusic);
    public void PlayDefeatMusic() => PlayMusic(defeatMusic);

    private void PlayMusic(AudioClip clip) {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic() {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
}
