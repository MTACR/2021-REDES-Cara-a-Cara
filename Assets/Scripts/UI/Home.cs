using System;
using System.Collections;
using System.Linq;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Home : MonoBehaviour
    {
        [SerializeField] public GameObject loading;
        [SerializeField] public GameObject error;
        [SerializeField] public TextMeshProUGUI subtext;
        [SerializeField] public TMP_InputField iptext;
        [SerializeField] public TextMeshProUGUI errortext;
        private Client client;
        private bool isHost;

        private void Awake()
        {
            client = Client.Instance;
            client.SetListeners(() =>
            {
                string msg = isHost ? "Waiting for opponent" : "Connecting to server";
                subtext.text = msg;
            }, ShowError);
            iptext.Select();
            iptext.text = "26.158.168.172";
        }

        public void StartConnection(bool isHost)
        {
            this.isHost = isHost;
            string ip = iptext.text.Trim();
            //ip = ip.Substring(0, ip.Length - 1);
            
            if (IsIpValid(ip))
            {
                loading.SetActive(true);
                Client.Instance.StartClient(isHost, ip);
                StartCoroutine(LoadScene());
            }
            else
            {
                Debug.LogError("Invalid IP format: " + ip);
                ShowError("Invalid IP format");
            }
        }

        public void Cancel()
        {
            loading.SetActive(false);
            Client.Instance.Dispose();
        }

        private void ShowError(string message)
        {
            errortext.text = message;
            loading.SetActive(false);
            error.SetActive(true);
        }

        public void HideError()
        {
            error.SetActive(false);
        }

        private bool IsIpValid(string ip)
        {
            if (String.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            string[] splitValues = ip.Split('.');

            return splitValues.Length == 4 && splitValues.All(r => byte.TryParse(r, out _));
        }
        
        private IEnumerator LoadScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Game");
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone && !Client.Instance.isReady)
            {
                yield return null;
            }
        
            asyncLoad.allowSceneActivation = true;
        }
    
    }
}
