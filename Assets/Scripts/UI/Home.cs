using System;
using System.Collections;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Home : MonoBehaviour
    {

        private Client client;
        [SerializeField] public GameObject loading;
        [SerializeField] public TextMeshProUGUI subtext;

        private void Start()
        {
            client = Client.Instance;
        }

        public void Host()
        {
            loading.SetActive(true);
            client.StartClient(true, () =>
            {
                subtext.text = "Waiting for opponent";
            }, ip =>
            {
                subtext.text = "Connection from " + ip;
            }, ShowError);
            StartCoroutine(LoadScene("Game"));
        }

        public void Join()
        {
            loading.SetActive(true);
            client.StartClient(false, () =>
            {
                subtext.text = "Connecting to server";
            }, ip =>
            {
                subtext.text = "Connected to " + ip;
            }, ShowError);
            StartCoroutine(LoadScene("Game"));
        }

        public void Cancel()
        {
            client.Cancel();
            loading.SetActive(false);
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
