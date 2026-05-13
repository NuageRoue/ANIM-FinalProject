using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        UpdateVolume();
    }

    public void UpdateVolume()
    {
        audioSource.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }
}