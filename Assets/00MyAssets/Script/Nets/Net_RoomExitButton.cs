using Photon.Pun;
using UnityEngine;

public class Net_RoomExitButton : MonoBehaviour
{
    //�ގ�
    public void ExitB()
    {
        PhotonNetwork.LeaveRoom();
    }
}
