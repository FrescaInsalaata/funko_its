using UnityEngine;

public class MusicLooper : MonoBehaviour
{
    public AudioClip[] tracks;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (tracks.Length > 0)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentTrackIndex = (currentTrackIndex + 1) % tracks.Length;
            PlayTrack(currentTrackIndex);
        }
    }

    void PlayTrack(int index)
    {
        audioSource.clip = tracks[index];
        audioSource.Play();
    }
}
