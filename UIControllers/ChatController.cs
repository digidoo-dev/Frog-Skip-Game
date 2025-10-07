using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChatController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioResource newMessageSound;
    [SerializeField] private GameObject chatMessageCardPrefab;

    [SerializeField] private float chatMessageHeight = 100f;
    [SerializeField] private float distanceBetweenChatMessages = 5f;
    [SerializeField] private float firstMessagePosY = -55f;

    [SerializeField] private ScrollRect chatScrollRect;
    [SerializeField] private RectTransform scrollViewContentTransform;
    [SerializeField] private TMP_InputField chatMessageInputField;
    [SerializeField] private Button sendMessageButton;

   private List<GameObject> shownMessagesList = new List<GameObject>();



    private void Start()
    {
        sendMessageButton.onClick.AddListener(() =>
        {
            SendMessage();
        });

        chatMessageInputField.onSubmit.AddListener((message) =>
        {
            SendMessage();
        });
    }




    private void SendMessage()
    {
        if (chatMessageInputField.text.Length == 0) return;
        LobbyManager.Instance.NewChatMessageRpc(NetworkManager.Singleton.LocalClientId, chatMessageInputField.text);
        chatMessageInputField.text = "";
    }

    public void AddMessageToChatView(string playerName, string message)
    {
        var instance = Instantiate(chatMessageCardPrefab, scrollViewContentTransform);
        instance.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, chatMessageHeight);


        int index = shownMessagesList.Count;
        Vector3 anchPos = new Vector3(0, firstMessagePosY - index * (chatMessageHeight + distanceBetweenChatMessages), 0);

        instance.GetComponent<RectTransform>().anchoredPosition = anchPos;
        instance.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = playerName + " :";
        instance.transform.Find("PlayerMessageText").GetComponent<TextMeshProUGUI>().text = message;

        shownMessagesList.Add(instance);

        float contentHeight = shownMessagesList.Count * (chatMessageHeight + distanceBetweenChatMessages);

        scrollViewContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentHeight);

        if (shownMessagesList.Count > 2)
        {
            scrollViewContentTransform.anchoredPosition = new Vector3(0, contentHeight - 200, 0);
        }

        audioSource.resource = newMessageSound;
        audioSource.Play();
    }
}
