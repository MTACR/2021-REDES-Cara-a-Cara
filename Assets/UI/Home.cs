using System.Collections;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Home : MonoBehaviour
    {

        public Client client;
        public GameObject loading;
        public GameObject waiting;

        public void Host()
        {
            TextMeshPro text = waiting.GetComponent<TextMeshPro>();
            
            loading.SetActive(true);
            client.StartClient(true, () => waiting.SetActive(true), ip =>
                    text.text = "Connection from " + ip
            , ShowError);
            StartCoroutine(LoadScene("Game"));
        }

        public void Join()
        {
            //client.StartClient(false, () => { }, ShowError);
            //StartCoroutine(LoadScene("Game"));
        }

        private void ShowError()
        {
            
        }
    
        private IEnumerator LoadScene(string name)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone && !client.isReady)
            {
                yield return null;
            }
        
            asyncLoad.allowSceneActivation = true;
        }
    
    }
}
