using System.Collections;
using Network;
using UnityEngine.SceneManagement;

namespace Game
{
    public static class Utils
    {

        public static IEnumerator ILoadScene(string name)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(name);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone && !Client.Instance.isReady) yield return null;

            asyncLoad.allowSceneActivation = true;
        }
        
    }
}