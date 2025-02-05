using Photon.Pun;
using TMPro;
using UnityEngine;

public class Net_JoinPlayerUI : MonoBehaviour
{
    Photon.Realtime.Player PlayerD;
    [Tooltip("�ԍ��e�L�X�g"), SerializeField] TextMeshProUGUI IndexTx;
    [Tooltip("�v���C���[���e�L�X�g"), SerializeField] TextMeshProUGUI NameTx;
    [Tooltip("�}�X�^�[�p�Ǘ�UI"), SerializeField] GameObject MasterOnlyUI;
    [Tooltip("�}�X�^�[�\��"), SerializeField] GameObject IsMasterUI;

    public void UISet(int Index,Photon.Realtime.Player Player)
    {
        PlayerD = Player;
        IndexTx.text = "[" + Index + "]";
        NameTx.text = Player.NickName;
        MasterOnlyUI.SetActive(PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer != Player);
        IsMasterUI.SetActive(Player.IsMasterClient);
    }
    public void MasterChange()
    {
        PhotonNetwork.SetMasterClient(PlayerD);
    }
    public void KickPlayer()
    {
        PhotonNetwork.CloseConnection(PlayerD);
    }
}
