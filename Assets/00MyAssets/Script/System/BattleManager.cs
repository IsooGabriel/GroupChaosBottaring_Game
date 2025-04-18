using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DataBase;
using static PlayerValue;
using static Manifesto;

public class BattleManager : MonoBehaviourPunCallbacks,IPunObservable
{
    static public BattleManager BTManager;

    [SerializeField] public int TimeLimSec;
    public int TimeStar;
    public int DeathStar;

    public float EStaMults;
    public int Stage;
    public int Time;
    public int DeathCount;
    public int Star;
    public bool Win;
    public bool End;
    public List<string> Messages = new List<string>();
    [System.NonSerialized] public List<State_Base> StateList = new List<State_Base>();
    [System.NonSerialized] public List<State_Hit> HitList = new List<State_Hit>();
    [System.NonSerialized] public List<State_Base> PlayerList = new List<State_Base>();
    [System.NonSerialized] public List<State_Base> BossList = new List<State_Base>();
    [System.NonSerialized] public List<Enemy_WaveSpawne> WaveSpList = new List<Enemy_WaveSpawne>();

    float ListTimer = 0;
    bool EndSave = false;
    void Awake()
    {
        BTManager = this;
        if (!PhotonNetwork.InRoom) return;
        if (photonView.IsMine)
        {
            Time = TimeLimSec * 60;
            EStaMults = 1f + (PhotonNetwork.CurrentRoom.PlayerCount - 1) * 0.8f;
            DeathStar = Mathf.RoundToInt(DeathStar * (1f + (PhotonNetwork.CurrentRoom.PlayerCount - 1) * 0.3f));
            Stage = StageID;
        }
    }
    private void Update()
    {
        ListTimer += UnityEngine.Time.fixedUnscaledDeltaTime;
        if (ListTimer <= 5f) return;
        ListTimer = 0;
        ListReflesh();
    }
    void FixedUpdate()
    {
        if (!PhotonNetwork.InRoom) return;
        if (End)
        {
            if(PhotonNetwork.OfflineMode)PSaves.StageSoloStars[Stage] = Mathf.Max(PSaves.StageSoloStars[Stage], Star);
            else PSaves.StageMultStars[Stage] = Mathf.Max(PSaves.StageMultStars[Stage], Star);
            if (!EndSave)
            {
                EndSave = true;
                Save();
            }
        }
        if (!photonView.IsMine) return;
        var CRoom = PhotonNetwork.CurrentRoom;
        CRoom.IsOpen = false;
        if (!End)
        {
            if (Time > 0) Time--;
            if (WaveSpList.Count <= 0)
            {
                var BossCheck = BossList.Count > 0;
                for (int i = 0; i < BossList.Count; i++)
                {
                    if (BossList[i] == null) continue;
                    if (BossList[i].HP > 0) BossCheck = false;
                }
                Win = BossCheck;
                if (BossCheck || Time <= 0) End = true;
            }
            else
            {
                var WaveCheck = true;
                for(int i = 0; i < WaveSpList.Count; i++)
                {
                    if (WaveSpList[i] == null) continue;
                    if (!WaveSpList[i].Clear) WaveCheck = false;
                }
                Win = WaveCheck;
                if(WaveCheck || Time <= 0)End = true;
            }
            if (TimeLimSec <= 0) End = false;
            Star = 3;
            if (Time <= TimeStar * 60) Star--;
            if (DeathCount > DeathStar) Star--;
            if (Time <= 0) Star = 0;
        }

    }
    void ListReflesh()
    {
        StateList.RemoveAll(x => x == null);
        HitList.RemoveAll(x => x == null);
        PlayerList.RemoveAll(x => x == null);
        BossList.RemoveAll(x => x == null);
        WaveSpList.RemoveAll(x => x == null);
    }
    public void DeathAdd()
    {
        photonView.RPC(nameof(RPC_DeathAdd), RpcTarget.All);
    }
    public void SEPlay(Class_Base_SEPlay SEPlays,Vector3 Pos,bool Local = false)
    {
        SEPlay(SEPlays.Clip, Pos, SEPlays.Volume, SEPlays.Pitch, Local);
    }
    public void SEPlay(AudioClip SEClip, Vector3 Pos, float Volume, float Pitch,bool Local=false)
    {
        var SEID = DB.SEs.IndexOf(SEClip);
        if (SEID < 0) return;
        if (!Local) photonView.RPC(nameof(RPC_SEPlay), RpcTarget.All, SEID, Pos, Volume, Pitch);
        else RPC_SEPlay(SEID, Pos, Volume, Pitch);
    }
    public void MessageAdd(string Message)
    {
        photonView.RPC(nameof(RPC_MessageAdd), RpcTarget.All,Message);
    }
    [PunRPC]
    void RPC_DeathAdd()
    {
        if (!photonView.IsMine) return;
        DeathCount++;
    }
    [PunRPC]
    void RPC_SEPlay(int SEID, Vector3 Pos, float Volume, float Pitch)
    {
        int VCount = Mathf.CeilToInt(Volume / 100f);
        for (int i = 0; i < VCount; i++)
        {
            var SEObj = Instantiate(DB.SEObj, Pos, Quaternion.identity);
            SEObj.clip = DB.SEs[SEID];
            if (i == VCount - 1) SEObj.volume = (Volume * 0.01f) % 1f;
            else SEObj.volume = 1f;
            SEObj.pitch = Pitch / 100f;
            if (Pitch < 0) SEObj.time = DB.SEs[SEID].length - 0.01f;
            SEObj.Play();
            Destroy(SEObj.gameObject, 10f);
        }
    }
    [PunRPC]
    void RPC_MessageAdd(string Message)
    {
        Messages.Add(Message);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Messages.Add(otherPlayer.NickName + "\\が退室しました");
    }
    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Stage);
            stream.SendNext(Time);
            stream.SendNext(DeathCount);
            stream.SendNext(Star);
            stream.SendNext(DeathStar);
            stream.SendNext(Win);
            stream.SendNext(End);
            stream.SendNext(EStaMults);
        }
        else
        {
            Stage = (int)stream.ReceiveNext();
            Time = (int)stream.ReceiveNext();
            DeathCount = (int)stream.ReceiveNext();
            Star = (int)stream.ReceiveNext();
            DeathStar = (int)stream.ReceiveNext();
            Win = (bool)stream.ReceiveNext();
            End = (bool)stream.ReceiveNext();
            EStaMults = (float)stream.ReceiveNext();
        }
    }
}
