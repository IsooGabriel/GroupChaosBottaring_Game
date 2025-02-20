﻿using Photon.Pun;

using UnityEngine;
using System.Linq;
using static Statics;
using static DataBase;
using static BattleManager;
using static Manifesto;
using static Calculation;
public class State_Atk
{
    static public void Branch(State_Base USta, bool Enter, bool Stay)
    {
        var AtkD = USta.AtkD;
        if (AtkD.Branchs == null) return;
        for (int i = 0; i < AtkD.Branchs.Length; i++)
        {
            var BranchD = AtkD.Branchs[i];
            bool NumCheck = false;
            for (int j = 0; j < BranchD.BranchNums.Length; j++)
            {
                if (USta.AtkBranch == BranchD.BranchNums[j]) NumCheck = true;
            }
            if (!NumCheck) continue;
            if (!V3IntTimeCheck(USta.AtkTime, (Vector3Int)BranchD.Times)) continue;
            bool Check = true;
            for (int j = 0; j < BranchD.Ifs.Length; j++)
            {
                switch (BranchD.Ifs[j])
                {
                    case Enum_AtkIf.攻撃単入力:
                        if (!Enter) Check = false;
                        break;
                    case Enum_AtkIf.攻撃長入力:
                        if (!Stay) Check = false;
                        break;
                    case Enum_AtkIf.攻撃未入力:
                        if (Enter || Stay) Check = false;
                        break;
                    case Enum_AtkIf.攻撃未長入力:
                        if (Stay) Check = false;
                        break;
                    case Enum_AtkIf.地上:
                        if(!USta.Ground) Check = false;
                        break;
                    case Enum_AtkIf.空中:
                        if (USta.Ground) Check = false;
                        break;
                    case Enum_AtkIf.MP有り:
                        if (USta.LowMP) Check = false;
                        break;
                    case Enum_AtkIf.MP無し:
                        if(!USta.LowMP)Check = false;
                        break;
                }
                if (!Check) break;
            }
            if (!Check) continue;
            if (BranchD.UseMP > 0 && USta.LowMP) continue;
            USta.MP -= BranchD.UseMP;
            USta.AtkBranch = BranchD.FutureNum;
            USta.AtkTime = BranchD.FutureTime;
            return;

        }
    }
    static public void Fixed(State_Base USta)
    {
        var AtkD = USta.AtkD;
        if (AtkD.Fixeds == null) return;
        for(int i = 0; i < AtkD.Fixeds.Length; i++)
        {
            var MFixed = AtkD.Fixeds[i];
            if (MFixed.BranchNum >= 0 && USta.AtkBranch != MFixed.BranchNum) continue;
            if (V3IntTimeCheck(USta.AtkTime, (Vector3Int)MFixed.Times))
            {
                USta.SpeedRem = MFixed.SpeedRem;
                USta.NoDash = MFixed.NoDash;
                USta.NoJump = MFixed.NoJump;
                USta.Aiming = MFixed.Aiming;
                USta.NGravity = MFixed.NGravity;
            }
        }
    }

    static public void Shot(State_Base USta, Vector3 BasePos, Vector3 BaseRot,Vector3 CamRot)
    {
        var AtkD = USta.AtkD;
        if (AtkD.Shots == null) return;
        for (int i = 0; i < AtkD.Shots.Length; i++)
        {
            var AtShot = AtkD.Shots[i];
            for (int j = 0; j < AtShot.Fires.Length; j++)
            {
                var AtFire = AtShot.Fires[j];
                if (AtFire.BranchNum >= 0 && USta.AtkBranch != AtFire.BranchNum) continue;
                if (!V3IntTimeCheck(USta.AtkTime, AtFire.Times)) continue;

                for (int k = 0; k < AtFire.Count; k++)
                {
                    float WaySet = k - ((AtFire.Count - 1) / 2f);
                    var Pos = PosGet(USta, AtFire, BasePos, BaseRot, WaySet,USta.AtkTime);
                    var Rot = RotGet(USta, AtFire, Pos, BaseRot, CamRot, WaySet, USta.AtkTime);
                    Shots(USta, AtShot,USta.AtkBranch, AtFire.Speed, Pos, Rot);
                }
            }
        }
    }
    static public void ShotAdd(State_Base USta,int BranchID,Data_AddShot AddShotD,int Times, Vector3 BasePos, Vector3 BaseRot)
    {
        for (int i = 0; i < AddShotD.Shots.Length; i++)
        {
            var AtShot = AddShotD.Shots[i];
            for (int j = 0; j < AtShot.Fires.Length; j++)
            {
                var AtFire = AtShot.Fires[j];
                if (AtFire.BranchNum >= 0 && BranchID != AtFire.BranchNum) continue;
                if (!V3IntTimeCheck(Times, AtFire.Times)) continue;
                for (int k = 0; k < AtFire.Count; k++)
                {
                    float WaySet = k - ((AtFire.Count - 1) / 2f);
                    var Pos = PosGet(USta, AtFire, BasePos, BaseRot, WaySet,Times);
                    var Rot = RotGet(USta, AtFire, Pos, BaseRot,BaseRot, WaySet, Times);
                    Shots(USta, AtShot,BranchID, AtFire.Speed, Pos, Rot);
                }

            }
        }
    }

    static public Vector3 PosGet(State_Base USta, Class_Atk_Shot_Fire Fire, Vector3 BasePos, Vector3 BaseRot, float Way,int Times)
    {
        var Pos = BasePos;
        switch (Fire.Trans.PosBase)
        {
            case Enum_PosBase.ターゲット位置:
                if (USta.Target != null) Pos = USta.Target.PosGet();
                break;
        }
        Pos += Quaternion.Euler(BaseRot) * Fire.Trans.PosChange;
        Pos += Quaternion.Euler(BaseRot) * Fire.Trans.PosWay * Way;
        Pos += Quaternion.Euler(BaseRot) * Fire.Trans.PosTime * Times;
        Vector3 RPos;
        RPos.x = Float_NegRand(Fire.Trans.PosRand.x);
        RPos.y = Float_NegRand(Fire.Trans.PosRand.y);
        RPos.z = Float_NegRand(Fire.Trans.PosRand.z);
        Pos += Quaternion.Euler(BaseRot) * RPos;
        return Pos;
    }
    static public Vector3 RotGet(State_Base USta, Class_Atk_Shot_Fire Fire, Vector3 BasePos, Vector3 BaseRot, Vector3 CamRot, float Way, int Times)
    {
        var Rot = RotBaseGet(USta, Fire.Trans.RotBase, BasePos, BaseRot, CamRot);
        Rot += Fire.Trans.RotChange;
        Rot += Fire.Trans.RotWay * Way;
        Rot += Fire.Trans.RotTime * Times;
        Rot.x += Float_NegRand(Fire.Trans.RotRand.x);
        Rot.y += Float_NegRand(Fire.Trans.RotRand.y);
        Rot.z += Float_NegRand(Fire.Trans.RotRand.z);
        return Rot;
    }
    static public Vector3 RotBaseGet(State_Base USta, Enum_RotBase RotBase,Vector3 Pos,Vector3 Rot,Vector3 CamRot)
    {
        switch (RotBase)
        {
            case Enum_RotBase.固定:return Vector3.zero;
            case Enum_RotBase.ターゲット方向:
                if (USta.Target != null || USta.TargetHit != null)
                {
                    var TVect = (USta.Target != null ? USta.Target.PosGet() : USta.TargetHit.PosGet()) - Pos;
                    return Quaternion.LookRotation(TVect, Vector3.forward).eulerAngles;
                }
                break;
            case Enum_RotBase.使用者カメラ方向: return CamRot;
        }
        return Rot;
    }
    static public void Shots(State_Base USta, Class_Atk_Shot_Base Shot,int BranchID,Vector2 Speed,Vector3 Pos,Vector3 Rot)
    {
        var ShotIns = PhotonNetwork.Instantiate(Shot.Obj.name, Pos, Quaternion.Euler(Rot));
        var ShotRig = ShotIns.GetComponent<Rigidbody>();
        ShotRig.linearVelocity += ShotIns.transform.forward * V2_Rand_Float(Speed) * 0.01f;
        var SObj = ShotIns.GetComponent<Shot_Obj>();
        if (SObj != null)
        {
            SObj.USta = USta;
            SObj.ShotD = Shot;
            SObj.BranchNum = BranchID;
        }
        var Sta = ShotIns.GetComponent<State_Base>();
        if (Sta != null)
        {
            Sta.Team = USta.Team;
        }
    }

    static public void Move(State_Base USta, Vector3 CamRot)
    {
        var AtkD = USta.AtkD;
        if (AtkD.Moves == null) return;
        for (int i = 0; i < AtkD.Moves.Length; i++)
        {
            var Move = AtkD.Moves[i];
            if (Move.BranchNum >= 0 && USta.AtkBranch != Move.BranchNum) continue;
            if (!V3IntTimeCheck(USta.AtkTime, Move.Times)) continue;
            var RigVect = USta.Rig.linearVelocity;
            var Rot = RotBaseGet(USta, Move.Base,USta.PosGet(), USta.RotGet(), CamRot);
            if(Move.SetSpeed) RigVect = Quaternion.Euler(Rot) * Move.Vect * 0.01f;
            else RigVect += Quaternion.Euler(Rot) * Move.Vect * 0.01f;
            USta.Rig.linearVelocity = RigVect;
        }
    }
    static public void State(State_Base USta)
    {
        var AtkD = USta.AtkD;
        if (AtkD.States == null) return;
        for (int i = 0; i < AtkD.States.Length; i++)
        {
            var State = AtkD.States[i];
            if (State.BranchNum >= 0 && USta.AtkBranch != State.BranchNum) continue;
            if (!V3IntTimeCheck(USta.AtkTime, State.Times)) continue;
            var Val = (float)Cal(State.Adds,USta,USta);
            switch (State.State)
            {
                case Enum_State.回復:USta.Damage(USta.PosGet(), -Mathf.RoundToInt(Val)); break;
                case Enum_State.ダメージ: USta.Damage(USta.PosGet(), Mathf.RoundToInt(Val)); break;
                case Enum_State.MP増加: USta.MP += Val;break;
                case Enum_State.MP減少: USta.MP -= Val; break;
                case Enum_State.SP増加: USta.SP += Val;break;
                case Enum_State.SP減少: USta.SP -= Val; break;
            }
        }
    }
    static public void Buf(State_Base USta)
    {
        var AtkD = USta.AtkD;
        if (AtkD.Bufs == null) return;
        for (int i = 0; i < AtkD.Bufs.Length; i++)
        {
            var Buf = AtkD.Bufs[i];
            if (Buf.BranchNum >= 0 && USta.AtkBranch != Buf.BranchNum) continue;
            if (!V3IntTimeCheck(USta.AtkTime, Buf.Times)) continue;
            for (int j = 0; j < Buf.BufSets.Length; j++) USta.BufSets(Buf.BufSets[j], USta);
        }
    }
    static public void WeponSet(State_Base USta)
    {
        var AtkD = USta.AtkD;
        if (AtkD.WeponSets == null) return;
        for (int i = 0; i < AtkD.WeponSets.Length; i++)
        {
            var AtWep = AtkD.WeponSets[i];
            if (AtWep.BranchNum >= 0 && USta.AtkBranch != AtWep.BranchNum) continue;
            if (V3IntTimeCheck(USta.AtkTime, (Vector3Int)AtWep.Times))
            {
                USta.WeponSets.TryAdd((int)AtWep.Set, -1);
                USta.WeponPoss.TryAdd((int)AtWep.Set, Vector3.zero);
                USta.WeponRots.TryAdd((int)AtWep.Set, Vector3.zero);

                USta.WeponSets[(int)AtWep.Set] = DB.Wepons.IndexOf(AtWep.Obj);
                USta.WeponPoss[(int)AtWep.Set] = AtWep.PosChange;
                USta.WeponRots[(int)AtWep.Set] = AtWep.RotChange;
            }
        }
    }
    static public void Anim(State_Base USta)
    {
        var AtkD = USta.AtkD;
        if (AtkD.Anims == null) return;
        for (int i = 0; i < AtkD.Anims.Length; i++)
        {
            var AtAnim = AtkD.Anims[i];
            if (AtAnim.BranchNum >= 0 && USta.AtkBranch != AtAnim.BranchNum) continue;
            if (V3IntTimeCheck(USta.AtkTime, (Vector3Int)AtAnim.Times))
            {
                if (AtAnim.ID != 0)
                {
                    USta.Anim_AtkID = AtAnim.ID;
                    USta.Anim_AtkSpeed = 1f + AtAnim.Speed*0.01f;
                }
            }
        }
    }
    static public void SEPlay(State_Base USta)
    {
        var AtkD = USta.AtkD;
        if (AtkD.SEPlays == null) return;
        for(int i = 0; i < AtkD.SEPlays.Length; i++)
        {
            var SEPlay = AtkD.SEPlays[i];
            if (SEPlay.BranchNum >= 0 && USta.AtkBranch != SEPlay.BranchNum) continue;
            if (!V3IntTimeCheck(USta.AtkTime, SEPlay.Times)) continue;
            BTManager.SEPlay(SEPlay.Clip, USta.PosGet(), SEPlay.Volume, SEPlay.Pitch);
        }
    }
    static public void SEPlayAdd(Data_AddShot AddShotD, Vector3 Pos)
    {
        if (AddShotD.SEPlays == null) return;
        for (int i = 0; i < AddShotD.SEPlays.Length; i++)
        {
            var SEPlay = AddShotD.SEPlays[i];
            BTManager.SEPlay(SEPlay.Clip, Pos, SEPlay.Volume, SEPlay.Pitch);
        }
    }
}
