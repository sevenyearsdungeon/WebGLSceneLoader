using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneGetter : Singleton<SceneGetter>
{
    public CanvasGroup sliderGroup;
    public Slider progressSlider;
    public static float progress;
    public static Status status = Status.Idle;
    public static AsyncOperation sceneLoadOperation;
    public enum Status
    {
        Idle,
        Downloading,
        LoadingScene,
    }

    Dictionary<string,AssetBundle> loadedScenes;
    HashSet<string> loadingScenes;
    void Awake()
    {
        loadedScenes = new Dictionary<string, AssetBundle>();
        loadingScenes = new HashSet<string>();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        LoadScene("landingpage");
    }

    public void LoadScene(string name)
    {
        if (status != Status.Idle)
            return;
        name = name.ToLowerInvariant();
        if (loadedScenes.ContainsKey(name))
        {
            LoadSceneFromAssetBundle(loadedScenes[name]);
            
            return;
        }
        if (loadingScenes.Contains(name))
            return;
        StartCoroutine(FetchAndLoad(name));
    }

    IEnumerator FetchAndLoad(string name)
    {
        status = Status.Downloading;
        loadingScenes.Add(name);
#if UNITY_EDITOR
        string root = "http://localhost:80/scenes/windows";
#else
        string root = string.Join("/",Application.absoluteURL,"scenes/webgl");
#endif
        var path = string.Join("/",root,name);

        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(path))
        {
            sliderGroup.alpha = 1;
            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                yield return null;
                progress = uwr.downloadProgress;
                progressSlider.value = progress;
            }
            sliderGroup.alpha = 0;

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                loadedScenes.Add(name, bundle);
                LoadSceneFromAssetBundle(loadedScenes[name]);
                status = Status.LoadingScene;
            }
        }
        loadingScenes.Remove(name);
        status = Status.Idle;
    }

    void LoadSceneFromAssetBundle(AssetBundle bundle)
    {
                sceneLoadOperation = SceneManager.LoadSceneAsync(bundle.GetAllScenePaths().First(), LoadSceneMode.Single);
    }

}
