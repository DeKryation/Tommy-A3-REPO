using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameSFX
{
    // --- System / UI Feedback ---
    ConnectionError,
    ConnectionSuccess,
    LinePull,
    LineCancel,
    HoverPort,

    // --- Music / Background ---
    AmbientBGM,
    MainMenu,

    // --- Environment Sounds ---
    DoorSlam,
    DoorOpen,
    Fan,
    CrabScuttle,
    CCTVPress,

    // --- Player Actions ---
    PlayerFootsteps,
    Vaulting,
    PickUpSecrewdriver,
    UnscrewVent
}

[Serializable]
public class GameSoundEntry
{
    public GameSFX id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Sound Library")]
    [SerializeField] private List<GameSoundEntry> soundLibrary = new List<GameSoundEntry>();
    
    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 8;
    
    [Header("Volume Control")]
    [SerializeField][Range(0f, 1f)] private float masterVolume = 1f;
    
    private Dictionary<GameSFX, GameSoundEntry> _lookup;
    private List<AudioSource> _pool;
    private Transform _poolRoot;
    
    void Awake()
    {
        // Allow multiple instances - don't destroy, just set as instance
        if (Instance == null)
        {
            Instance = this;
        }
        
        BuildLookup();
        CreatePool();
    }
    
    void OnDestroy()
    {
        // Clear instance reference if this is the current instance
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void BuildLookup()
    {
        _lookup = new Dictionary<GameSFX, GameSoundEntry>();
        foreach (var s in soundLibrary)
        {
            if (s.clip == null) continue;
            if (!_lookup.ContainsKey(s.id))
                _lookup.Add(s.id, s);
            else
                Debug.LogWarning($"GameAudioManager: Duplicate SFX entry: {s.id}");
        }
    }
    
    private void CreatePool()
    {
        _pool = new List<AudioSource>(poolSize);
        _poolRoot = new GameObject("AudioPool").transform;
        _poolRoot.SetParent(transform);
        
        for (int i = 0; i < poolSize; i++)
        {
            var go = new GameObject($"SFXSource_{i}");
            go.transform.SetParent(_poolRoot);
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            _pool.Add(src);
        }
    }
    
    private AudioSource GetAvailableSource()
    {
        foreach (var s in _pool)
        {
            if (!s.isPlaying) return s;
        }
        return _pool[0]; // Fallback to first source
    }
    
    public void PlaySFX(GameSFX id)
    {
        if (_lookup == null) BuildLookup();
        
        if (!_lookup.TryGetValue(id, out var entry))
        {
            Debug.LogWarning($"GameAudioManager: SFX not found: {id}");
            return;
        }
        
        if (entry.clip == null)
        {
            Debug.LogWarning($"GameAudioManager: No clip assigned for {id}");
            return;
        }
        
        var src = GetAvailableSource();
        src.clip = entry.clip;
        src.volume = entry.volume * masterVolume;
        src.Play();
    }
    
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        
        var src = GetAvailableSource();
        src.clip = clip;
        src.volume = volume * masterVolume;
        src.Play();
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }
    
    public float GetMasterVolume()
    {
        return masterVolume;
    }
}
