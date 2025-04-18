using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using static Manifesto;
using static BattleManager;
public class Enemy_WaveSpawne : MonoBehaviourPun,IPunObservable
{
    public bool Clear;
    [SerializeField]int Wave;
    [SerializeField]int GizmosWave;
    [SerializeField]Class_Wave[] Waves;

    List<State_Base> Enemys = new List<State_Base>();

    private void Start()
    {
        Wave = 0;
        Clear = false;
        BTManager.WaveSpList.Add(this);
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        bool Check=true;
        for(int i = 0; i < Enemys.Count; i++)
        {
            if (Enemys[i] != null && Enemys[i].HP > 0) Check = false;
        }
        if (!Check) return;
        if (Wave < Waves.Length)
        {
            Spawne();
            Wave++;
        }
        else Clear = true;
    }
    void Spawne()
    {
        for (int i = 0; i < Enemys.Count; i++)
        {
            if (Enemys[i] != null) Enemys[i].Boss = false;
        }
        Enemys.Clear();
        for (int i = 0; i < Waves[Wave].Enemys.Length; i++)
        {
            var Waved = Waves[Wave];
            var Enemy = Waved.Enemys[i];
            var Pos = transform.position;
            if (Waved.Pos != null && i < Waved.Pos.Length) Pos += Waved.Pos[i];
            var EnemyIns = PhotonNetwork.InstantiateRoomObject(Enemy.name, Pos, Quaternion.identity);
            var EnemyState = EnemyIns.GetComponent<State_Base>();
            if (EnemyState != null)
            {
                Enemys.Add(EnemyState);
                if (Waved.HPMult != null && i < Waved.HPMult.Length)
                {
                    EnemyState.MHP = Mathf.RoundToInt(EnemyState.MHP * (1f + Waved.HPMult[i] * 0.01f));
                    EnemyState.HPRegene = Mathf.RoundToInt(EnemyState.HPRegene * (1f + Waved.HPMult[i] * 0.01f));
                }
                if (Waved.AtkMult != null && i < Waved.AtkMult.Length) EnemyState.Atk = Mathf.RoundToInt(EnemyState.Atk * (1f + Waved.AtkMult[i] * 0.01f));
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (GizmosWave < 0 || GizmosWave >= Waves.Length) return;
        var Waved = Waves[GizmosWave];
        Gizmos.color = new Color(1, 0, 1, 0.5f);
        for (int i = 0; i < Waved.Enemys.Length; i++)
        {
            var Pos = transform.position;
            if (Waved.Pos != null && i < Waved.Pos.Length) Pos += Waved.Pos[i];
            Gizmos.DrawSphere(Pos, 0.5f);
        }

    }
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Wave);
            stream.SendNext(Clear);
            List<int> EnemyViewIDs = new List<int>();
            for (int i = 0; i < Enemys.Count; i++)
            {
                if (Enemys[i] == null) continue;
                EnemyViewIDs.Add(Enemys[i].photonView.ViewID);
            }
            stream.SendNext(EnemyViewIDs.ToArray());
        }
        else
        {
            Wave = (int)stream.ReceiveNext();
            Clear = (bool)stream.ReceiveNext();
            Enemys.Clear();
            var EnemyViewIDs = (int[])stream.ReceiveNext();
            for(int i = 0; i < EnemyViewIDs.Length; i++)
            {
                var PView = PhotonNetwork.GetPhotonView(EnemyViewIDs[i]);
                if (PView == null) continue;
                var ESta = PView.GetComponent<State_Base>();
                if (ESta != null) Enemys.Add(ESta);
            }
        }
    }
}
