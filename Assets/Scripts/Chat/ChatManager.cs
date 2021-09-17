using UnityEngine;

namespace Chat
{
    public class ChatManager : MonoBehaviour
    {
        
        [SerializeField] public GameObject container;
        [SerializeField] public GameObject prefab;

        public void ShowMessage(bool isMine, string sender, string text)
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(isMine, sender, text);
        }
        
    }
}
