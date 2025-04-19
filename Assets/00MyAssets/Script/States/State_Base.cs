using Photon.Pun;
using UnityEngine;
using static DataBase;
using static Statics;
using static BattleManager;
using System.Collections.Generic;
using System.Linq;
using static PlayerValue;
using static Manifesto;
using static Calculation;
using NaughtyAttributes;
using System.IO;

public class State_Base : MonoBehaviourPun
{
    #region インスペクター変数
    [Foldout("設定")]public string Name;
    [Foldout("設定")] public Rigidbody Rig;
    [Foldout("設定")] public Transform CamTrans;
    [Foldout("設定")] public float Hight;
    [Foldout("設定")] public float SizeAdd;
    [Foldout("設定"),Tooltip("チーム")] public int Team;
    [Foldout("設定"), Tooltip("プレイヤー")] public bool Player;
    [Foldout("設定"), Tooltip("ボス")] public bool Boss;
    [Foldout("設定"), Tooltip("不死")] public bool Undet;
    [Foldout("設定")] public Class_Base_SEPlay DamageSE;
    [Foldout("設定")] public Class_Base_SEPlay DeathSE;
    [Foldout("設定"), Tooltip("死亡エフェクト")] public GameObject DeathEffect;
    [Foldout("設定"), Tooltip("数値同期間隔(秒)")] public float StreamTime;
    [Foldout("設定"), Tooltip("プレイヤー用変数")] public Player_BattleValues PLValues;
    [Foldout("設定"), Tooltip("武器表示用変数")] public State_WeponValues WepValues;
    //
    [Foldout("ステータス"), Tooltip("最大HP")]public int MHP;
    [Foldout("ステータス"), Tooltip("秒間HP回復速度")] public float HPRegene;
    [Foldout("ステータス"), Tooltip("最大MP(移動力)")] public int MMP;
    [Foldout("ステータス"), Tooltip("秒間MP回復速度")] public float MPRegene;
    [Foldout("ステータス"), Tooltip("秒間SP回復速度")] public float SPRegene;
    [Foldout("ステータス"), Tooltip("最大ブレイク値")] public int MBreak;
    [Foldout("ステータス"), Tooltip("ブレイク時間")] public int BreakTime;
    [Foldout("ステータス"), Tooltip("攻撃力")] public int Atk;
    [Foldout("ステータス"), Tooltip("防御力")] public int Def;
    //
    [Foldout("数値")]public float HP;
    [Foldout("数値")] public float MP;
    [Foldout("数値")] public float SP;
    [Foldout("数値")] public float BreakV;
    [Foldout("数値")] public int BreakT;
    [Foldout("数値")] public bool Ground;
    [Foldout("数値")] public int DeathTime;
    [Foldout("数値")] public State_Base Target;
    [Foldout("数値")] public State_Hit TargetHit;
    [Foldout("数値")] public List<Class_Sta_BufInfo> Bufs = new List<Class_Sta_BufInfo>();
    //

    [Foldout("変数")] public bool LowMP;
    [Foldout("変数")] public int DashTime;
    [Foldout("変数")] public bool GroundB;

    [Foldout("変数")] public Data_Atk AtkD;
    [Foldout("変数")] public int AtkSlot;
    [Foldout("変数")] public int AtkTime;
    [Foldout("変数")] public int AtkBranch;

    [Foldout("変数")] public float SpeedRem;
    [Foldout("変数")] public bool NoJump;
    [Foldout("変数")] public bool NoDash;
    [Foldout("変数")] public bool Aiming;
    [Foldout("変数")] public bool NGravity;
    [Foldout("変数")] public bool NoDamage;

    [Foldout("変数")] public int Anim_MoveID;
    [Foldout("変数")] public int Anim_AtkID;
    [Foldout("変数")] public float Anim_AtkSpeed;
    [Foldout("変数")] public int Anim_OtherID;

    #endregion
    #region 内部変数

    public Dictionary<int,Class_Sta_AtkCT> AtkCTs = new Dictionary<int,Class_Sta_AtkCT>();
    public Dictionary<int,GameObject> BufEffects = new Dictionary<int,GameObject>();
    public Dictionary<int, int> LocalCTs = new Dictionary<int, int>();
    [System.NonSerialized] public GameObject BreakEffect = null;
    [System.NonSerialized] public bool LimitFlag = false;
    float STimer = 0;

    public int FMHP
    {
        get
        {
            float FVal = MHP;
            FVal *= 1f + BufPowGet(Enum_Bufs.HP増加) * 0.01f;
            return Mathf.RoundToInt(FVal);
        }
    }
    public int FMMP
    {
        get
        {
            float FVal = MMP;
            return Mathf.RoundToInt(FVal);
        }
    }
    public int FAtk
    {
        get
        {
            float FVal = Atk;
            FVal *= 1f + (BufPowGet(Enum_Bufs.攻撃増加) * 0.01f) - (BufPowGet(Enum_Bufs.攻撃低下) * 0.01f);
            return Mathf.RoundToInt(FVal);
        }
    }
    public int FDef
    {
        get
        {
            float FVal = Def;
            FVal *= 1f + (BufPowGet(Enum_Bufs.防御増加) * 0.01f) - (BufPowGet(Enum_Bufs.防御低下) * 0.01f);
            return Mathf.RoundToInt(FVal);
        }
    }
    #endregion
    private void Awake()
    {
        HP = MHP;
    }
    private void Start()
    {
        BTManager.StateList.Add(this);
        if (Player) BTManager.PlayerList.Add(this);
        if (Boss) BTManager.BossList.Add(this);
        if (CamTrans == null) CamTrans = transform;
        if (Team != 0)
        {
            MHP = Mathf.RoundToInt(MHP * BTManager.EStaMults);
            HPRegene = Mathf.RoundToInt(HPRegene * BTManager.EStaMults);
            MBreak = Mathf.RoundToInt(MBreak * BTManager.EStaMults);
        }

        if (photonView.IsMine && Player)
        {
            MHP = Mathf.RoundToInt(MHP * (1f + PriSetGet.PassiveLVGet(Enum_Passive.HP増加) * 0.2f));
            HPRegene = Mathf.RoundToInt(HPRegene * (1f + PriSetGet.PassiveLVGet(Enum_Passive.自然再生) * 0.5f));
            MMP = Mathf.RoundToInt(MMP * (1f + PriSetGet.PassiveLVGet(Enum_Passive.MP増加) * 0.1f));
            MPRegene = Mathf.RoundToInt(MPRegene * (1f + PriSetGet.PassiveLVGet(Enum_Passive.気力増幅) * 0.05f));
            SPRegene = Mathf.RoundToInt(SPRegene * (1f + PriSetGet.PassiveLVGet(Enum_Passive.SPブースト) * 0.2f));
            Atk = Mathf.RoundToInt(Atk * (1f + PriSetGet.PassiveLVGet(Enum_Passive.攻撃力増加) * 0.1f));
            Def = Mathf.RoundToInt(Def * (1f + PriSetGet.PassiveLVGet(Enum_Passive.防御力増加) * 0.25f));
            if (!PhotonNetwork.OfflineMode) Name = photonView.Owner.NickName;
        }
        HP = FMHP;
        MP = FMMP;
    }
    void Update()
    {
        //return;
        if (PhotonNetwork.OfflineMode) return;
        if (!photonView.IsMine) return;
        STimer -= Time.unscaledDeltaTime;
        if (STimer <= 0)
        {
            STimer = Mathf.Max(0.05f, StreamTime);
            photonView.RPC(nameof(RPC_Stream_Base), RpcTarget.Others, MHP,MMP,Atk,Def,MBreak,Name,Team,NoDamage,LimitFlag);
            photonView.RPC(nameof(RPC_Stream_Value), RpcTarget.Others, HP,BreakV,BreakT);
            photonView.RPC(nameof(RPC_Stream_Anim), RpcTarget.Others, Anim_MoveID,Anim_AtkID,Anim_AtkSpeed,Anim_OtherID);
            var Buf_ID = new List<int>();
            var Buf_Index = new List<int>();
            var Buf_Time = new List<int>();
            var Buf_Pow = new List<int>();
            var Buf_TimeMax = new List<int>();
            for (int i = 0; i < Bufs.Count; i++)
            {
                Buf_ID.Add(Bufs[i].ID);
                Buf_Index.Add(Bufs[i].Index);
                Buf_Time.Add(Bufs[i].Time);
                Buf_Pow.Add(Bufs[i].Pow);
                Buf_TimeMax.Add(Bufs[i].TimeMax);
            }
            photonView.RPC(nameof(RPC_Stream_Buf), RpcTarget.Others,
                Buf_ID.ToArray(),Buf_Index.ToArray(),Buf_Time.ToArray(),Buf_Pow.ToArray(),Buf_TimeMax.ToArray());
        }
    }
    private void FixedUpdate()
    {
        Anim_OtherID = 0;
        if (HP <= 0) Anim_OtherID = 2;
        else if (BreakT > 0) Anim_OtherID = 3;
        BufEffectSet();
        if (!photonView.IsMine) return;

        Ground = GroundB;
        GroundB = false;
        if (MP <= 0)
        {
            MP = 0;
            LowMP = true;
        }
        if(Ground)MP += MPRegene / 60f;
        MP = Mathf.Min(MP, FMMP);
        if (LowMP && MP >= FMMP * 0.2f) LowMP = false;
        if (HP <= 0)
        {
            StateDeaths();
        }
        if (HP > 0)
        {
            LivesTimes();
        }
        if (!Player && Team != 0 && BTManager.End) Deletes();
        DashTime--;
        BufTimeRems();
        LocalCTRems();
        AtkPlays(CamTrans.eulerAngles);
        if (Rig) Rig.useGravity = !NGravity;
        AddInfoChange();
    }
    #region 内部メソッド
    void Deletes()
    {
        photonView.RPC(nameof(RPC_DeathEffect), RpcTarget.All);
        PhotonNetwork.Destroy(gameObject);
    }
    void AtkPlays(Vector3 CamRot)
    {
        if (!PhotonNetwork.InRoom) return;
        #region CT減少
        var CTKeys = AtkCTs.Keys.ToArray();
        for (int i = 0; i < CTKeys.Length; i++)
        {
            AtkCTs[CTKeys[i]].CT--;
            if (AtkCTs[CTKeys[i]].CT <= 0) AtkCTs.Remove(CTKeys[i]);
        }
        #endregion
        if (WepValues != null)
        {
            var WeponKeys = WepValues.WeponSets.Keys.ToArray();
            for (int i = 0; i < WeponKeys.Length; i++)
            {
                WepValues.WeponSets[WeponKeys[i]] = -1;
            }
        }
        SpeedRem = 0;
        NoJump = false;
        NoDash = false;
        Aiming = false;
        NGravity = false;
        NoDamage = false;
        #region スキル処理
        if (HP <= 0) AtkD = null;
        if (BreakT > 0) AtkD = null;
        Anim_AtkID = 0;
        Anim_AtkSpeed = 1;
        if (AtkD == null)
        {
            AtkTime = 0;
            AtkSlot = -1;
            return;
        }
        State_Atk.Fixed(this);
        State_Atk.Shot(this, PosGet(), RotGet(), CamRot);
        State_Atk.Move(this, CamRot);
        State_Atk.State(this);
        State_Atk.Buf(this);
        State_Atk.WeponSet(this);
        State_Atk.Anim(this);
        State_Atk.SEPlay(this);
        AtkTime++;
        if (AtkTime > AtkD.EndTime) AtkD = null;
        #endregion
    }
    void StateDeaths()
    {
        var GutsLV = BufPowGet(Enum_Bufs.根性);
        if (GutsLV <= 0)
        {
            if (Player)
            {
                if (DeathTime == 0)
                {
                    PLValues.DeathCount++;
                    BTManager.SEPlay(DeathSE, PosGet());
                    BTManager.DeathAdd();
                    BTManager.MessageAdd("<color=#FF0000>" + Name + "</color>\\<color=#FF0000>は倒れた!!!</color>");
                    var DeathPowLV = PriSetGet.PassiveLVGet(Enum_Passive.死に力);
                    if (DeathPowLV > 0)
                    {
                        BufSets(Enum_Bufs.攻撃増加, -1000, Enum_BufSet.付与増加, 0, DeathPowLV * 5);
                    }
                }
            }
            else if (!Boss && !Undet)
            {
                if (DeathTime >= 60)
                {
                    BTManager.SEPlay(DeathSE, PosGet());
                    Deletes();
                }
            }
            else
            {
                if (DeathTime == 0) BTManager.SEPlay(DeathSE, PosGet());
            }
        }
        else
        {
            BufPowRem(Enum_Bufs.根性, 1);
            HP = 1;
        }
        DeathTime++;
        DashTime = 0;
    }
    void LivesTimes()
    {
        HP += HPRegene / 60f;
        HP -= BufPowGet(Enum_Bufs.毒) / 60f;
        HP = Mathf.Clamp(HP, 1, FMHP);
        SP += SPRegene / 60f;
        DeathTime = 0;
        if (Player)
        {
            var GutLV = PriSetGet.PassiveLVGet(Enum_Passive.根性);
            if (GutLV > 0 && !BufCheck(Enum_Bufs.根性CT))
            {
                BufSets(Enum_Bufs.根性, -1000, Enum_BufSet.付与, 0, GutLV);
                BufSets(Enum_Bufs.根性CT, -1000, Enum_BufSet.付与, 60 * 15, 0);
            }
        }
        if (MBreak >= 0)
        {
            if (BreakT <= 0)
            {
                if (BreakV >= MBreak) BreakT = BreakTime;
            }
            else
            {
                BreakT--;
                BreakV = 0;
            }
        }
        if (BreakT > 0)
        {
            if (BreakEffect == null)
            {
                BreakEffect = Instantiate(DB.BreakEffect, PosGet(), Quaternion.identity);
                BreakEffect.transform.localScale = Vector3.one * (1f + SizeAdd * 0.01f);
                BreakEffect.transform.parent = Rig != null ? Rig.transform : transform;
            }
        }
        else if (BreakEffect != null) Destroy(BreakEffect);
        if (LimitFlag && !BufCheck(Enum_Bufs.時間制限))
        {
            Deletes();
        }
    }
    void BufTimeRems()
    {
        for (int i = Bufs.Count - 1; i >= 0; i--)
        {
            var Bufi = Bufs[i];
            if (Bufi.TimeMax <= 0) continue;
            Bufi.Time--;
            if (Bufi.Time <= 0) Bufs.RemoveAt(i);
        }
    }
    void LocalCTRems()
    {
        var LocalKeys = LocalCTs.Keys.ToArray();
        for (int i = 0; i < LocalKeys.Length; i++)
        {
            LocalCTs[LocalKeys[i]]--;
            if (LocalCTs[LocalKeys[i]] <= 0) LocalCTs.Remove(LocalKeys[i]);
        }
    }
    void BufEffectSet()
    {
        var BufKeys = BufEffects.Keys.ToArray();
        for (int i = 0; i < BufKeys.Length; i++)
        {
            var BufGet = Bufs.Find(x => x.ID == BufKeys[i]);
            if (BufGet == null)
            {
                Destroy(BufEffects[BufKeys[i]]);
                BufEffects.Remove(BufKeys[i]);
            }
        }
        for (int i = 0; i < Bufs.Count; i++)
        {
            var Bufi = Bufs[i];
            var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
            if (BufD != null && BufD.EffectObj!=null && !BufEffects.ContainsKey(Bufi.ID))
            {
                var EffectIns = Instantiate(BufD.EffectObj, PosGet(), Quaternion.identity);
                EffectIns.transform.localScale = Vector3.one * (1f + SizeAdd * 0.01f);
                EffectIns.transform.parent = Rig != null ? Rig.transform : transform;
                BufEffects.Add(Bufi.ID, EffectIns);
            }
        }
    }
    void BufPowRem(Enum_Bufs BufID, int Val)
    {
        for (int i = Bufs.Count - 1; i >= 0; i--)
        {
            var Bufi = Bufs[i];
            if (Bufi.ID == (int)BufID)
            {
                var Vald = Val;
                Val -= Bufi.Pow;
                Bufi.Pow -= Vald;
                if (Bufi.Pow <= 0)
                {
                    var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
                    if (BufD != null) BTManager.SEPlay(BufD.RemSE, PosGet());
                    Bufs.Remove(Bufi);
                }
            }
            if (Val <= 0) return;
        }
    }
    void AddInfoChange()
    {
        if (PLValues == null) return;
        PLValues.AddTimer++;
        if(PLValues.AddTimer >= 60)
        {
            PLValues.AddTimer = 0;
            for (int i = PLValues.AddDams.Length - 1; i > 0; i--)
            {
                PLValues.AddDams[i] = PLValues.AddDams[i - 1];
                PLValues.AddHits[i] = PLValues.AddHits[i - 1];
            }
            PLValues.AddDams[0] = 0;
            PLValues.AddHits[0] = 0;
        }
    }
    #endregion
    #region 呼び出しメソッド
    public Vector3 PosGet()
    {
        if (Rig != null) return Rig.position + Vector3.up * Hight;
        else return transform.position + Vector3.up * Hight;
    }
    public Vector3 RotGet()
    {
        if (Rig != null) return Rig.transform.eulerAngles;
        else return transform.eulerAngles;
    }
    public void Damage(Vector3 HitPos, int Val,float Break = 0)
    {
        photonView.RPC(nameof(RPC_Damage), RpcTarget.All, HitPos, Val,Break);
    }
    public void TargetSet()
    {
        float NearDis = -1;
        Target = null;
        foreach (var Sta in BTManager.StateList)
        {
            if (Sta == null) continue;
            if (!TeamCheck(this, Sta)) continue;
            if (Sta.HP <= 0) continue;
            float Dis = Vector3.Distance(PosGet(), Sta.PosGet());
            if (NearDis < 0 || NearDis > Dis)
            {
                NearDis = Dis;
                Target = Sta;
            }
        }
    }
    public void AtkInput(int UseAtkSlot, Data_Atk UseAtkD, bool Enter, bool Stay,int StayFl,bool Exit)
    {
        if (HP <= 0) return;
        if (BreakT > 0) return;
        if (AtkD != UseAtkD)
        {
            if (!Enter) return;
            if (AtkCTs.ContainsKey(UseAtkSlot)) return;
            if (SP < UseAtkD.SPUse) return;
            if (Player)
            {
                switch (UseAtkSlot)
                {
                    default: if (AtkSlot > 0) return; break;
                    case 10: if (AtkSlot >= 10) return; break;
                }
            }
            else if (AtkD != null) return;
            if (UseAtkD.SPUse > 0) SP = 0;
            int CTs = Mathf.RoundToInt(UseAtkD.CT * 60);
            if (Player)
            {
                CTs = Mathf.RoundToInt(CTs * (1f - PriSetGet.PassiveLVGet(Enum_Passive.CTカット) * 0.10f));
                if (UseAtkD.AtkType == Enum_AtkType.必殺)
                {
                    PLValues.E_AtkCount++;
                    var SpHealLV = PriSetGet.PassiveLVGet(Enum_Passive.必殺再生);
                    if (SpHealLV > 0) Damage(PosGet(), Mathf.RoundToInt(MHP * SpHealLV * 0.15f));
                    var SpReturnLV = PriSetGet.PassiveLVGet(Enum_Passive.必殺返還);
                    if (SpReturnLV > 0) SP = SpReturnLV * 25;
                }
            }
            if (UseAtkD.AtkType == Enum_AtkType.必殺) BTManager.MessageAdd(Name + "\\の必殺「" + UseAtkD.Name + "」");
            AtkCTs.Add(UseAtkSlot, new Class_Sta_AtkCT { CT = CTs, CTMax = CTs });

            AtkD = UseAtkD;
            AtkSlot = UseAtkSlot;
            AtkTime = 0;
            AtkBranch = 0;
            return;
        }
        else
        {
            if (UseAtkD != AtkD)
            {
                Enter = false;
                Stay = false;
            }
            State_Atk.Branch(this, Enter, Stay,StayFl,Exit);
        }

    }
    public void BufSets(Class_Base_BufSet BufSet,State_Base AddSta)
    {
        var PowVal = Mathf.RoundToInt((float)Cal(BufSet.PowVal, AddSta, this));
        var PowMax = Mathf.RoundToInt((float)Cal(BufSet.PowMax, AddSta, this));
        if (AddSta != null && AddSta.PLValues!=null)
        {
            var BufD = DB.Bufs.Find(x => x.Buf == BufSet.Buf);
            if (BufD != null && BufD.Type == Enum_BufType.バフ) AddSta.PLValues.AddBuf++;
            if (BufD != null && BufD.Type == Enum_BufType.デバフ) AddSta.PLValues.AddDBuf++;
        }
        BufSets(BufSet.Buf, BufSet.Index,BufSet.Set, BufSet.TimeVal, PowVal, BufSet.TimeMax, PowMax);
    }
    public void BufSets(Enum_Bufs BufID, int Index, Enum_BufSet Sets, int Time, int Pow, int TMax=0, int PMax=0)
    {
        photonView.RPC(nameof(RPC_BufSet), RpcTarget.All, (int)BufID, Index, (int)Sets, Time, Pow, TMax, PMax);
    }
    public int BufPowGet(Enum_Bufs BufID)
    {
        int Pow = 0;
        for(int i = 0; i < Bufs.Count; i++)
        {
            if (Bufs[i].ID == (int)BufID) Pow += Bufs[i].Pow;
        }
        return Pow;
    }
    public bool BufCheck(Enum_Bufs BufID)
    {
        for (int i = 0; i < Bufs.Count; i++)
        {
            if (Bufs[i].ID == (int)BufID) return true;
        }
        return false;
    }
    public float BufTPMultGet(Enum_Bufs BufID)
    {
        float Val = 0;
        for (int i = 0; i < Bufs.Count; i++)
        {
            if (Bufs[i].ID == (int)BufID) Val += Bufs[i].Time * Bufs[i].Pow;
        }
        return Val;
    }
    public void HitEvents(State_Base HitSta,Vector3 Pos, Enum_DamageType DamType, bool ShortAtk)
    {
        if (Player)
        {
            var TaruLV = PriSetGet.PassiveLVGet(Enum_Passive.タルタル);
            if (TaruLV > 0 && !HitSta.LocalCTs.ContainsKey((int)Enum_PassiveAtk.タルタル))
            {
                HitSta.LocalCTs.Add((int)Enum_PassiveAtk.タルタル, 60 * 3);
                State_Atk.ShotAdd(this, TaruLV, DB.PassiveAtks[(int)Enum_PassiveAtk.タルタル], 0, Pos, Vector3.zero);
            }
            var AddShashLV = PriSetGet.PassiveLVGet(Enum_Passive.追斬);
            if (AddShashLV > 0 && !LocalCTs.ContainsKey((int)Enum_PassiveAtk.追斬))
            {
                LocalCTs.Add((int)Enum_PassiveAtk.追斬, 60 * 4);
                State_Atk.ShotAdd(this, AddShashLV, DB.PassiveAtks[(int)Enum_PassiveAtk.追斬], 0, Pos, Vector3.zero);
            }
            var WSystemLV = PriSetGet.PassiveLVGet(Enum_Passive.Wシステム);
            if(WSystemLV > 0 && !LocalCTs.ContainsKey((int)Enum_PassiveAtk.Wシステム))
            {
                LocalCTs.Add((int)Enum_PassiveAtk.Wシステム, 60 * 1);
                int Timed = 60 * 6;
                if (ShortAtk) BufSets(Enum_Bufs.遠距離強化, -1000, Enum_BufSet.付与増加, Timed, 2 * WSystemLV, Timed, 40);
                else BufSets(Enum_Bufs.近距離強化, -1000, Enum_BufSet.付与増加, Timed, 3 * WSystemLV, Timed, 60);
            }
        }
    }
    public void AddInfoAdd(int Dam)
    {
        if (PLValues == null) return;
        PLValues.AddDamTotal += Dam;
        PLValues.AddDams[0]+=Dam;
        PLValues.AddHitTotal++;
        PLValues.AddHits[0]++;
    }
    public void AddInfoReset()
    {
        if (PLValues == null) return;
        PLValues.AddTimer = 0;
        for(int i = 0; i < PLValues.AddDams.Length; i++)
        {
            PLValues.AddDams[i] = 0;
            PLValues.AddHits[i] = 0;
        }
        PLValues.AddDamTotal = 0;
        PLValues.AddHitTotal = 0;
    }
    #endregion
    #region RPCメソッド
    [PunRPC]
    void RPC_Damage(Vector3 HitPos, int Val,float Break,PhotonMessageInfo PhInfo)
    {
        if (Val > 0 && NoDamage) return;
        if (HP <= 0) return;
        if (Val < 0)
        {
            int Healm = FMHP - (int)HP;
            Val = -Mathf.Min(-Val, Healm);
        }
        else if (Val > 0)
        {
            int Barria = BufPowGet(Enum_Bufs.バリア);
            if (Barria > 0)
            {
                Val /= 10;
                if (photonView.IsMine)
                {
                    BufPowRem(Enum_Bufs.バリア, 1);
                    BTManager.SEPlay(DB.BarriaHitSE, HitPos, true);
                }
            }
            int Shilds = BufPowGet(Enum_Bufs.シールド);
            if (Shilds > 0)
            {
                int Vald = Val;
                int ShValue = Mathf.Min(Val, Shilds);
                Val = Mathf.Max(0, Val - Shilds);
                if (photonView.IsMine)
                {
                    Color ShildCol = Color.blue;
                    switch (Team)
                    {
                        case 0:
                            ShildCol = Color.cyan;
                            break;
                    }
                    DamageObj.DamageSet(HitPos, Mathf.Abs(ShValue), ShildCol);
                    BufPowRem(Enum_Bufs.シールド, Vald);
                    BTManager.SEPlay(DB.ShildHitSE, HitPos, true);
                }
            }
        }
        if (Val == 0) return;
        Color DamCol = Val >= 0 ? Color.white : Color.magenta;
        GameObject HitEffect = Val >= 0 ? DB.HitEffects[1] : DB.HealEffects[1];
        switch (Team)
        {
            case 0:
                DamCol = Val >= 0 ? Color.red : Color.green;
                HitEffect = Val >= 0 ? DB.HitEffects[0] : DB.HealEffects[0];
                break;
        }
        DamageObj.DamageSet(HitPos, Mathf.Abs(Val), DamCol);
        var InsHitEffect = Instantiate(HitEffect, HitPos, Quaternion.identity);
        if (Val >= 0)BTManager.SEPlay(DamageSE, HitPos,true);

        if (!photonView.IsMine && PhInfo.Sender != PhotonNetwork.LocalPlayer) return;
        BreakV+=Break;
        if (Player)
        {
            if (Val > 0)PLValues.ReceiveDam += Val;
            var LifeVibrationLV = PriSetGet.PassiveLVGet(Enum_Passive.生命の振動);
            if (LifeVibrationLV > 0 && !LocalCTs.ContainsKey((int)Enum_PassiveAtk.生命の振動))
            {
                LocalCTs.Add((int)Enum_PassiveAtk.生命の振動, 60 * 1);
                int Timed = 60 * 6;
                BufSets(Enum_Bufs.与ダメージ増加, -1000, Enum_BufSet.付与増加, Timed, 3 * LifeVibrationLV, Timed, 36);
            }
        }
        HP -= Val;
    }
    [PunRPC]
    void RPC_DeathEffect()
    {
        if (DeathEffect != null) Instantiate(DeathEffect, PosGet(), Quaternion.identity);
    }
    [PunRPC]
    void RPC_BufSet(int BufID, int Index, int Sets, int Time, int Pow, int TMax, int PMax, PhotonMessageInfo PhInfo)
    {
        if (!photonView.IsMine && PhInfo.Sender != PhotonNetwork.LocalPlayer) return;
        Class_Sta_BufInfo Bufi = null;
        for (int i = 0; i < Bufs.Count; i++)
        {
            var Bufd = Bufs[i];
            if(Bufd.ID == BufID && Bufd.Index == Index)
            {
                Bufi = Bufd;
                break;
            }
        }
        if (Sets != (int)Enum_BufSet.消去)
        {
            if (Bufi != null && Sets == (int)Enum_BufSet.切り替え)
            {
                var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
                if (BufD != null) BTManager.SEPlay(BufD.RemSE, PosGet());
                Bufs.Remove(Bufi);
            }
            else
            {
                if (Bufi == null && Sets == (int)Enum_BufSet.不付与増加) return;
                if (Bufi == null)
                {
                    Bufi = new Class_Sta_BufInfo { ID = BufID, Index = Index, Time = 0, Pow = 0, TimeMax = 1 };
                    Bufs.Add(Bufi);
                }
                if (Sets == (int)Enum_BufSet.付与 || Sets == (int)Enum_BufSet.切り替え)
                {
                    Bufi.Time = Mathf.Max(Bufi.Time, Time);
                    Bufi.Pow = Mathf.Max(Bufi.Pow, Pow);
                }
                else
                {
                    if (TMax > 0) Bufi.Time = Mathf.Min(Bufi.Time + Time, TMax);
                    else Bufi.Time += Time;
                    if (PMax > 0) Bufi.Pow = Mathf.Min(Bufi.Pow + Pow, PMax);
                    else Bufi.Pow += Pow;
                }
                if (Bufi.Time <= 0) Bufi.TimeMax = 0;
                if (Bufi.TimeMax > 0) Bufi.TimeMax = Mathf.Max(Bufi.Time, Bufi.TimeMax);
            }
        }
        else if (Bufi != null)
        {
            var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
            if (BufD != null) BTManager.SEPlay(BufD.RemSE, PosGet());
            Bufs.Remove(Bufi);
        }
    }
    [PunRPC]
    void RPC_Stream_Base(int Str_MHP,int Str_MMP, int Str_Atk, int Str_Def, int Str_MBreak, string Str_Name, int Str_Team, bool Str_NoDamage, bool Str_LimitFlag)
    {
        MHP = Str_MHP;
        MMP = Str_MMP;
        Atk = Str_Atk;
        Def = Str_Def;
        MBreak = Str_MBreak;
        Name = Str_Name;
        Team = Str_Team;
        NoDamage = Str_NoDamage;
        LimitFlag = Str_LimitFlag;
    }
    [PunRPC]
    void RPC_Stream_Value(float Str_HP, float Str_BreakV, int Str_BreakT)
    {
        HP = Str_HP;
        BreakV = Str_BreakV;
        BreakT = Str_BreakT;
    }
    [PunRPC]
    void RPC_Stream_Anim(int Str_MoveID, int Str_AtkID, float Str_AtkSpeed, int Str_OtherID)
    {
        Anim_MoveID = Str_MoveID;
        Anim_AtkID = Str_AtkID;
        Anim_AtkSpeed = Str_AtkSpeed;
        Anim_OtherID = Str_OtherID;
    }
    [PunRPC]
    void RPC_Stream_Buf(int[] Str_ID, int[] Str_Index, int[] Str_Time, int[] Str_Pow, int[] Str_TimeMax)
    {
        Bufs.Clear();
        for (int i = 0; i < Str_ID.Length; i++)
        {
            Bufs.Add(new Class_Sta_BufInfo
            {
                ID = Str_ID[i],
                Index = Str_Index[i],
                Time = Str_Time[i],
                Pow = Str_Pow[i],
                TimeMax = Str_TimeMax[i],
            });
        }
    }

    #endregion

}
