using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AudioManager.Sound;

public class AudioManager : MonoBehaviour {
    [System.Serializable]
    public class Sound {
        [System.Serializable]
        public class Clip
        {
            [HideInInspector] public string name;
            public AudioClip clip;
            [Range(0, 2)]
            public float volume = 1;
            [Range(0, 2)]
            public float pitch = 1;
            public bool looping;
        }

        [HideInInspector] public string name;
        [HideInInspector] public int ID;
        [SerializeField] string displayName;
        [SerializeField] List<Clip> clips = new List<Clip>();

        public void OnValidate(int i)
        {
            ID = i;
            for (i = 0; i < clips.Count; i++) {
                if (clips[i].clip == null) continue;
                clips[i].name = i + ": " + clips[i].clip.name;
                if (clips[i].volume == 0 && clips[i].pitch == 0) {
                    clips[i].volume = 1;
                    clips[i].pitch = 1;
                }
            }

            if (!string.IsNullOrEmpty(displayName)) {
                name = ID + ": " + displayName;
                return;
            }
            if (clips.Count <= 0 || clips[0].clip == null) {
                name = ID + ": NULL";
                return;
            }
            name = ID + ": " + clips[0].clip.name;
        }

        public Clip getClip(int _ID)
        {
            if (clips.Count == 0 || _ID != ID) return null;
            return clips[Random.Range(0, clips.Count)];
        }
    }

    public static AudioManager instance;
    [SerializeField] List<Sound> sounds = new List<Sound>();

    private void OnValidate()
    {
        for (int i = 0; i < sounds.Count; i++) {
            sounds[i].OnValidate(i);   
        }
    }

    private void Awake()
    {
        instance = this;
    }

    //--1/3
    public void PlaySound(int ID)
    {
        AudioSource source = GetFreeAudioSource(gameObject);
        PlaySound(ID, source);
    }
    //--2/3
    public void PlaySound(int ID, GameObject obj)
    {
        AudioSource source = null;
        if (!obj.TryGetComponent(out source)) source = obj.AddComponent<AudioSource>();
        PlaySound(ID, source);
    }
    //--3/3
    public void PlaySound(int ID, AudioSource source)
    {
        Clip clip = getClip(ID);
        ConfigureSource(clip, source);
        source.Play();
    }

    Clip getClip(int ID)
    {
        Clip clip = null; 
        for (int i = 0; i < sounds.Count; i++) {
            clip = sounds[i].getClip(ID);
            if (clip != null) break;
        }
        return clip;
    }

    AudioSource GetFreeAudioSource(GameObject obj)
    {
        List<AudioSource> sources = obj.GetComponents<AudioSource>().ToList();

        for (int i = 0; i < sources.Count; i++) {
            if (!sources[i].isPlaying) return sources[i];
        }
        return obj.AddComponent<AudioSource>();
    }

    void ConfigureSource(Clip clip, AudioSource source)
    {
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.pitch = clip.pitch;
        source.loop = clip.looping;
    }

}
