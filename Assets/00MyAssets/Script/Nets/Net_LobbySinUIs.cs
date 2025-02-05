using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class SinsUI_LobbyRoomUIs : MonoBehaviour
{
    [Tooltip("���[��ID�e�L�X�g"), SerializeField] TextMeshProUGUI RoomIDTx;
    [Tooltip("���b�Z�[�W�e�L�X�g"), SerializeField] TextMeshProUGUI MessageTx;
    [Tooltip("�Q�[���o�[�W�����e�L�X�g"), SerializeField] TextMeshProUGUI VersionTx;
    [Tooltip("�v���C���[���e�L�X�g"), SerializeField] TextMeshProUGUI PlayersTx;
    [System.NonSerialized] public RoomInfo Room;

    public void Disp(RoomInfo Rooms)
    {
        Room = Rooms;
        RoomIDTx.text = Rooms.Name;
        if (Rooms.CustomProperties.TryGetValue("Message", out var Message) && Message.ToString() != "") MessageTx.text = Message.ToString();
        else MessageTx.text = "�́[���b�Z�[�W";
        if (Rooms.CustomProperties.TryGetValue("GameVer", out var GameVer))
        {
            VersionTx.text = "�o�[�W����:" + GameVer.ToString();
            if (Application.version != GameVer.ToString()) VersionTx.color = Color.red;
            else VersionTx.color = Color.white;
        }
        else
        {
            VersionTx.text = "�o�[�W����:???";
            VersionTx.color = Color.magenta;
        }
        PlayersTx.text = Rooms.PlayerCount + "/" + Rooms.MaxPlayers;
    }
    public void Joins()
    {
        PhotonNetwork.JoinRoom(Room.Name);
    }
}
