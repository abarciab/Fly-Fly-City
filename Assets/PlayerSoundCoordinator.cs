using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundCoordinator : MonoBehaviour
{
    [SerializeField] int ID;
    [SerializeField] bool testplay;
    AudioSource source;

    private void Start()
    {
        if (!gameObject.TryGetComponent(out source)) source = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (testplay && !source.isPlaying) AudioManager.instance.PlaySound(ID, source);
    }
}
