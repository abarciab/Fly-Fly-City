using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] float transitionSmoothness = 0.02f;
    [SerializeField] List<Sound> DistrictTracks = new List<Sound>(), barTracks = new List<Sound>();
    Sound current, currentBar;
    int currentDistrict = -1;

    private void Start()
    {
        for (int i = 0; i < DistrictTracks.Count; i++) {
            DistrictTracks[i] = Instantiate(DistrictTracks[i]);
            DistrictTracks[i].PlaySilent();
        }
        for (int i = 0; i < barTracks.Count; i++) {
            barTracks[i] = Instantiate(barTracks[i]);
        }
    }

    private void Update()
    {
        if (currentDistrict != Directory.gMan.currentDistrict) TransitionToNewDistrict();
        if (currentDistrict == -1) return;

        DistrictTracks[currentDistrict].PercentVolume(Directory.gMan.insideLocation ? 0.05f : 1, transitionSmoothness);
        DistrictTracks[currentDistrict].Play(restart: false);
        foreach (var t in DistrictTracks) {
            t.PercentVolume(0, transitionSmoothness);
            t.Play(restart: false);
        }

        if (Directory.gMan.insideLocation) PlayBarInterior();
        else foreach (var t in barTracks) t.PercentVolume(0, 0.5f);
    }

    void PlayBarInterior()
    {
        if (currentBar != null && currentBar.isPlaying()) currentBar.PercentVolume(1, 0.5f);
        else {
            currentBar = barTracks[Random.Range(0, barTracks.Count)];
            currentBar.Play();
        }
    }

    void TransitionToNewDistrict()
    {
        currentDistrict = Directory.gMan.currentDistrict;
        current = DistrictTracks[currentDistrict];
    }

    public void FadeOutCurrent(float time)
    {
        StartCoroutine(FadeOut(time));
    }

    IEnumerator FadeOut(float time)
    {
        float timePassed = 0;
        while (timePassed < time) {
            current.PercentVolume(Mathf.Lerp(1, 0, timePassed / time));
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
