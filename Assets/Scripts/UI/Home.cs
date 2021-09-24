using System.Linq;
using Callbacks;
using Game;
using Network;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Home : MonoBehaviour
    {
        [SerializeField] public GameObject loading;
        [SerializeField] public GameObject error;
        [SerializeField] public TextMeshProUGUI subtext;
        [SerializeField] public TMP_InputField iptext;
        [SerializeField] public TextMeshProUGUI errortext;

        private void Awake()
        {
            if (FindObjectOfType<TaskManager>() == null)
                new GameObject().AddComponent<TaskManager>().name = "Tasks";
            
            iptext.Select();
            iptext.text = "26.158.168.172";
        }

        public void StartConnection(bool isHost)
        {
            var ip = iptext.text.Trim();

            if (IsIpValid(ip))
            {
                loading.SetActive(true);
                Client.Instance.SetListeners(() =>
                {
                    var msg = isHost ? "Waiting for opponent" : "Connecting to server";
                    subtext.text = msg;
                }, ShowError);
                Client.Instance.StartClient(isHost, ip);
                StartCoroutine(Utils.ILoadScene("Game"));
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
        
        private void OnApplicationQuit()
        {
            Debug.Log("Application ending after " + Time.time + " seconds");
            Client.Instance?.Dispose();
        }
        
    }
}