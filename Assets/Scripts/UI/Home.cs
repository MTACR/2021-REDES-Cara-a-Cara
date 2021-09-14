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

        public Client client;
        public GameObject loading;
        public TextMeshProUGUI subtext;
        
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
