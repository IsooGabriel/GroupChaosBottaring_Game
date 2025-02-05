using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Net_GameTimeOut : MonoBehaviourPunCallbacks
{
    [Tooltip("�^�C���A�E�g�܂ł̕b��"),SerializeField] float TimeLim;
    float times = 0;
    void Update()
    {
        if (!PhotonNetwork.InRoom)
        {
            times += Time.unscaledDeltaTime;
            if (times > TimeLim)
            {
                SceneManager.LoadScene(0);
            }
        }
        else times = 0;
    }
    public override void OnLeftRoom()
    {
        times = TimeLim;
    }
}
