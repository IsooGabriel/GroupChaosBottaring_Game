using Photon.Pun;
using UnityEngine;

public class Net_MyRigView : MonoBehaviourPun
{
    const float MinTimer = 0.05f;

    [SerializeField] Rigidbody Rig;
    [SerializeField, Tooltip("重力影響%")] float GravityPer;
    [SerializeField, Tooltip("同期頻度(秒)")] float StreamTime = 0.1f;

    float Timer = 0;

    Vector3 targetPosition;
    Vector3 currentPosition;

    Vector3 receivedVelocity;
    float timeSinceLastUpdate;

    Quaternion targetRotation;
    Quaternion currentRotation;

    void Awake()
    {
        targetPosition = currentPosition = Rig.position;
        targetRotation = currentRotation = Rig.rotation;
        receivedVelocity = Vector3.zero;
        timeSinceLastUpdate = 0f;
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            timeSinceLastUpdate += Time.fixedDeltaTime;

            // velocity から予測位置を計算
            Vector3 predictedTarget = targetPosition + receivedVelocity * timeSinceLastUpdate;
            predictedTarget += 0.5f * Physics.gravity * timeSinceLastUpdate * timeSinceLastUpdate * GravityPer * 0.01f;
            // 補間
            currentPosition = Vector3.Lerp(currentPosition, predictedTarget, Time.fixedDeltaTime * 10f);
            currentRotation = Quaternion.Lerp(currentRotation, targetRotation, Time.fixedDeltaTime * 10f);

            Rig.MovePosition(currentPosition);
            Rig.MoveRotation(currentRotation);
        }
    }

    void Update()
    {
        if (PhotonNetwork.OfflineMode) return;
        if (photonView.IsMine)
        {
            Timer -= Time.unscaledDeltaTime;
            if (Timer <= 0)
            {
                Timer = Mathf.Max(MinTimer, StreamTime);
                photonView.RPC(nameof(RPC_Stream), RpcTarget.Others, Rig.position, Rig.linearVelocity, Rig.rotation);
            }
        }
    }

    [PunRPC]
    void RPC_Stream(Vector3 Pos, Vector3 Vel, Quaternion Rot)
    {
        targetPosition = Pos;
        receivedVelocity = Vel;
        targetRotation = Rot;

        timeSinceLastUpdate = 0f;
    }
}
