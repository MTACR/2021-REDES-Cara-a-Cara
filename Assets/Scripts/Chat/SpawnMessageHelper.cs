using UnityEngine;

public class SpawnMessageHelper : MonoBehaviour
{
    public GameObject container;
    public GameObject prefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            //message.Setup(Random.value % 10 > 5, "Other", "Isso é uma pergunta");
        }
    }
}