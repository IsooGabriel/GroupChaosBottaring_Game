using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Net_BaseUI : MonoBehaviourPunCallbacks
{
    [Tooltip("��������v���t�@�u"), SerializeField] Net_PhotonPrefabs Prefabs;
    [Tooltip("�I�t���C��UI"), SerializeField] GameObject OfflineUI;
    [Tooltip("�I�����C��UI"), SerializeField] GameObject OnlineUI;
    [Tooltip("�ڑ��ҋ@UI"), SerializeField] GameObject ConnectWait;
    [Tooltip("�ڑ�����UI"), SerializeField] GameObject ConnectEnd;
    [Tooltip("���[���I������"), SerializeField] UIChange Selects;
    [Tooltip("���[���I��pUI"), SerializeField] GameObject SelectUI;
    [Tooltip("�v���C���[������"), SerializeField] TMP_InputField PlayerNameIn;
    [Tooltip("�쐬���[��������"), SerializeField] TMP_InputField CreateNameIn;
    [Tooltip("�쐬���b�Z�[�W����"), SerializeField] TMP_InputField MessageIn;
    [Tooltip("�쐬�v���C�x�[�g"),SerializeField] Toggle PrivateT;
    [Tooltip("�Q�����[��������"), SerializeField] TMP_InputField JoinNameIn;
    [Tooltip("���r�[�p�T�uUI"), SerializeField] List<SinsUI_LobbyRoomUIs> LobbySinUIs;
    [Tooltip("���[����UI"), SerializeField] GameObject InRoomUI;

    private void Start()
    {
        Prefabs.PrefabPoolSet();
        Net_MyCustomTypes.Register();
        Application.targetFrameRate = 60;
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.EnableCloseConnection = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        PlayerNameIn.text = PlayerPrefs.GetString("PlayerName", "");
        Net_PlayerNameSet();
    }
    void LateUpdate()
    {
        if (!PlayerNameIn.isFocused) PlayerNameIn.text = PhotonNetwork.NickName;
        UIActives();
        LobbyIns();
    }
    /// <summary>UI�\��</summary>
    void UIActives()
    {
        bool Connect = !PhotonNetwork.IsConnected || !PhotonNetwork.IsConnectedAndReady;
        OfflineUI.SetActive(PhotonNetwork.OfflineMode);
        OnlineUI.SetActive(!PhotonNetwork.OfflineMode);
        ConnectWait.SetActive(Connect);
        ConnectEnd.SetActive(!Connect);
        SelectUI.SetActive(!PhotonNetwork.InRoom);
        InRoomUI.SetActive(PhotonNetwork.InRoom);
    }
    /// <summary>���r�[�Q��</summary>
    void LobbyIns()
    {
        if (Selects.UIID == 3 && !PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();
        if (Selects.UIID != 3 && PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
    }
    //�v���C���[���ύX
    public void Net_PlayerNameSet()
    {
        if (PlayerNameIn.text != "") PhotonNetwork.NickName = PlayerNameIn.text;
        else PhotonNetwork.NickName = "����" + Random.Range(1000,10000);
        PlayerPrefs.SetString("PlayerName", PhotonNetwork.NickName);
    }
    //�T�[�o�[�ڑ�
    public void Net_Connects()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    //�T�[�o�[�ؒf
    public void Net_Disconnect()
    {
        PhotonNetwork.Disconnect();
    }
    //�V�K���[���쐬
    public void Net_RoomCreate()
    {
        var RoomIDs = CreateNameIn.text;
        if (RoomIDs == "")
        {
            if (Random.value >= 0.1f) RoomIDs = "CreateRoom";
            else RoomIDs = "NaNameRoom";
            RoomIDs += Random.Range(0, 10000).ToString("D4");
        }
        PhotonNetwork.CreateRoom(RoomIDs,RoomOptionGet(MessageIn.text,PrivateT.isOn));
    }
    //���O���[���Q��
    public void Net_RoomJoin()
    {
        PhotonNetwork.JoinRoom(JoinNameIn.text);
    }
    //�����_�����[���Q��
    public void Net_RandomJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    //�����_�����[���p�쐬
    public override void OnJoinRandomFailed(short returnCode, string message)
    {

        string RoomIDs;
        if (Random.value >= 0.1f) RoomIDs = "RandomRoom";
        else RoomIDs = "BackRoom";
        RoomIDs += Random.Range(0, 10000).ToString("D4");
        PhotonNetwork.CreateRoom(RoomIDs, RoomOptionGet("�����_�����[��",false));
    }
    /// <summary>���[���I�v�V�����ݒ�</summary>
    public RoomOptions RoomOptionGet(string Message, bool Private)
    {
        var RoomOP = new RoomOptions();
        var RoomHash = new ExitGames.Client.Photon.Hashtable();
        RoomHash["Message"] = Message;
        RoomHash["GameVer"] = Application.version;
        RoomHash["GameStarts"] = false;
        RoomOP.MaxPlayers = 20;
        RoomOP.CustomRoomProperties = RoomHash;
        RoomOP.CustomRoomPropertiesForLobby = new string[] { "Message", "GameVer", "GameStarts" };
        RoomOP.IsVisible = !Private;
        return RoomOP;
    }
    //�I�t���C����
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.OfflineMode = true;
    }
    //���r�[�p
    Net_RoomList roomList = new Net_RoomList();
    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        roomList.Update(changedRoomList);
        if (PhotonNetwork.InLobby)
        {
            List<RoomInfo> Rooms = roomList.ToList();
            for (int i = 0; i < Mathf.Max(LobbySinUIs.Count, Rooms.Count); i++)
            {
                if (i < Rooms.Count)
                {
                    if (LobbySinUIs.Count <= i)
                    {
                        LobbySinUIs.Add(Instantiate(LobbySinUIs[0], LobbySinUIs[0].transform.parent));
                    }
                    var RoomSin = LobbySinUIs[i];
                    RoomSin.gameObject.SetActive(true);
                    RoomSin.Disp(Rooms[i]);
                }
                else LobbySinUIs[i].gameObject.SetActive(false);
            }
        }
    }

}
