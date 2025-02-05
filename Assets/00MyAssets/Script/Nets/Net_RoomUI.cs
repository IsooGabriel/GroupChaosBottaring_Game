using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Net_RoomUI : MonoBehaviour
{
    [Tooltip("���[�����e�L�X�g"), SerializeField]TextMeshProUGUI RoomName;
    [Tooltip("�Q���v���C���[�p�T�uUI"), SerializeField] List<Net_JoinPlayerUI> JoinPlayers;
    [Tooltip("�}�X�^�[�pUI"), SerializeField] GameObject MasterOnly;
    [Tooltip("�v���C�x�[�g�؂�ւ��g�O��"), SerializeField] Toggle PrivateT;
    [Tooltip("��}�X�^�[�pUI"), SerializeField] GameObject NoMaster;

    void LateUpdate()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        if (CRoom == null) return;
        UISet();
        PlayerDisp();
    }

    void PlayerDisp()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        var PlayerKeys = CRoom.Players.Keys.ToArray();
        for (int i = 0; i < Mathf.Max(PlayerKeys.Length, JoinPlayers.Count); i++)
        {
            if (JoinPlayers.Count <= i)
            {
                JoinPlayers.Add(Instantiate(JoinPlayers[0], JoinPlayers[0].transform.parent));
            }
            if (i < PlayerKeys.Length)
            {
                JoinPlayers[i].UISet(i + 1, CRoom.Players[PlayerKeys[i]]);
            }
            JoinPlayers[i].gameObject.SetActive(i < PlayerKeys.Length);
        }
    }
    void UISet()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        RoomName.text = CRoom.Name;
        MasterOnly.SetActive(PhotonNetwork.IsMasterClient);
        NoMaster.SetActive(!PhotonNetwork.IsMasterClient);
        PrivateT.isOn = !CRoom.IsVisible;
    }

    //���[���ގ�
    public void Net_RoomExit()
    {
        PhotonNetwork.LeaveRoom();
    }
    //�v���C�x�[�g�؂�ւ�
    public void Net_PrivateTChange()
    {
        var CRoom = PhotonNetwork.CurrentRoom;
        CRoom.IsVisible = !PrivateT.isOn;
    }
    //�Q�[���J�n
    public void Net_GameStart()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
