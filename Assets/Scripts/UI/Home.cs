using System.Collections;
using System.Linq;
using Callbacks;
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
            if (FindObjectOfType<TaskManager>() == null)
                new GameObject().AddComponent<TaskManager>();

            client = Client.Instance;
            client.SetListeners(() =>
            {
                var msg = isHost ? "Waiting for opponent" : "Connecting to server";
                subtext.text = msg;
            }, ShowError);
            iptext.Select();
            iptext.text = "26.158.168.172";
        }

        public void StartConnection(bool isHost)
        {
            this.isHost = isHost;
            var ip = iptext.text.Trim();

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
            subtext.text = "";
            loading.SetActive(false);
            Client.Instance.Dispose();
        }

        private void ShowError(string message)
        {
            subtext.text = "";
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
            if (string.IsNullOrWhiteSpace(ip)) return false;

            var splitValues = ip.Split('.');

            return splitValues.Length == 4 && splitValues.All(r => byte.TryParse(r, out _));
        }

        private static IEnumerator LoadScene()
        {
            var asyncLoad = SceneManager.LoadSceneAsync("Game");
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone && !Client.Instance.isReady) yield return null;

            asyncLoad.allowSceneActivation = true;
        }
    }
}