using System.Collections;
using System.Collections.Generic;
using Chat;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnMessageHelper : MonoBehaviour
{
    public GameObject container;
    public GameObject prefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Message message = Instantiate(prefab, container.transform).GetComponent<Message>();
            message.Setup(Random.value % 10 > 5, "Other", "Isso é uma pergunta");
        }
            
    }
}
