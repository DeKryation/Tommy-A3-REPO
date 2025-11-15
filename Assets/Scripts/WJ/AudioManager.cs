using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameSFX
{
    ConnectionError,
    ConnectionSuccess,
    LinePull,
    LineCancel,
    HoverPort
}

[Serializable]
public class GameSoundEntry
{
    public GameSFX id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
}

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }
    
    [SerializeField] private List<GameSoundEntry> soundLibrary = new List<GameSoundEntry>();
    [SerializeField] private int poolSize = 8;
    
    private Dictionary<GameSFX, GameSoundEntry> _lookup;
    private List<AudioSource> _pool;
    private Transform _poolRoot;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        BuildLookup();
        CreatePool();
    }
    
    private void BuildLookup()
    {
        _lookup = new Dictionary<GameSFX, GameSoundEntry>();
        foreach (var s in soundLibrary)
        {
            if (s.clip == null) continue;
            if (!_lookup.ContainsKey(s.id))
                _lookup.Add(s.id, s);
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
        return _pool[0];
    }
    
    public void PlaySFX(GameSFX id)
    {
        if (_lookup == null) BuildLookup();
        
        if (!_lookup.TryGetValue(id, out var entry))
        {
            Debug.LogWarning($"GameAudioManager: SFX not found: {id}");
            return;
        }
        
        if (entry.clip == null) return;
        
        var src = GetAvailableSource();
        src.clip = entry.clip;
        src.volume = entry.volume;
        src.Play();
    }
    
    public void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        
        var src = GetAvailableSource();
        src.clip = clip;
        src.volume = volume;
        src.Play();
    }
}
