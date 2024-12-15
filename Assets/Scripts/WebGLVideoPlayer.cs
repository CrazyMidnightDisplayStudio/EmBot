using UnityEngine;
using UnityEngine.Video;

public class WebGLVideoPlayer : MonoBehaviour
{
    private VideoPlayer _videoPlayer;
    [SerializeField] private string url;

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        PlayVideoWebGL();
    }

    private void PlayVideoWebGL()
    {
        var videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, url);
        _videoPlayer.url = videoPath;
        _videoPlayer.Play();
    }
}
