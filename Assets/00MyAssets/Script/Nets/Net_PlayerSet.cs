using UnityEngine;
using Photon.Pun;
public class Net_PlayerSet : MonoBehaviour
{
    [Tooltip("�v���C���[�I�u�W�F�N�g"), SerializeField] GameObject PlayerObj;
    bool Sets = false;
    void Update()
    {
        if (!PhotonNetwork.InRoom) return;
        if (Sets) return;
        Sets = true;
        PhotonNetwork.Instantiate(PlayerObj.name, transform.position, Quaternion.identity);
    }
}
