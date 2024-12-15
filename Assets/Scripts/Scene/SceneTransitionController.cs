using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scene
{
    public class SceneTransitionController : MonoBehaviour
    {
        private static SceneTransitionController _instance;
        
        [SerializeField] private GameObject fadeImagePrefab; 
        [SerializeField] private float fadeDuration = 6f;
        [SerializeField] private Transform imageCanvasTransform;
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            StartFadeIn();
        }
        


        private void StartFadeIn()
        {
            GameObject fadeObject = Instantiate(fadeImagePrefab, imageCanvasTransform);
            Image fadeImage = fadeObject.GetComponent<Image>();
            fadeImage.color = Color.black;

            fadeImage.DOFade(0f, fadeDuration).OnComplete(() => 
            {
                Destroy(fadeObject);
            });
        }
        
        public void LoadScene(string sceneName)
        {
            GameObject fadeObject = Instantiate(fadeImagePrefab);
            Image fadeImage = fadeObject.GetComponent<Image>();

            fadeImage.DOFade(1, fadeDuration).OnComplete(() => 
            {
                SceneManager.LoadScene(sceneName);
                Destroy(fadeObject);
            });
        }
    }
}
