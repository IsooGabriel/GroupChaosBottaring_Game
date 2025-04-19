using Photon.Pun;
using UnityEngine;
using static Statics;
using static Manifesto;
public class Enemy_Atk : MonoBehaviourPun
{
    [SerializeField] State_Base Sta;
    [SerializeField] Data_EnemyAI EnemyAI;
    [SerializeField] int timer;


    private void Start()
    {
        timer = 0;
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        if (Sta.Target == null)
        {
            if(EnemyAI.NoTargetResetTime) timer = 0;
            return;
        }
        timer++;
        if (timer > EnemyAI.TimerLim) timer = 0;
        for(int i = 0; i < EnemyAI.AtkAIs.Length; i++)
        {
            var AtkAI = EnemyAI.AtkAIs[i];
            if (!V3IntTimeCheck(timer, (Vector3Int)AtkAI.TimeIf)) continue;
            bool OIf = true;
            for(int j = 0; j < AtkAI.OtherIfs.Length; j++)
            {
                var OtherIf = AtkAI.OtherIfs[j];
                float Vals;
                switch (OtherIf.Ifs)
                {
                    case Enum_OtherIfs.HP割合_x以下:
                        if ((float)Sta.HP/ Sta.MHP > OtherIf.Val.x * 0.01f) OIf = false;
                        break;
                    case Enum_OtherIfs.HP割合_x以上:
                        if ((float)Sta.HP / Sta.MHP > OtherIf.Val.x * 0.01f) OIf = false;
                        break;
                    case Enum_OtherIfs.ターゲット距離_x以上:
                        Vals = Vector3.Distance(Sta.PosGet(), Sta.Target.PosGet());
                        if (Vals < OtherIf.Val.x) OIf = false;
                        break;
                    case Enum_OtherIfs.ターゲット距離_x以下:
                        Vals = Vector3.Distance(Sta.PosGet(), Sta.Target.PosGet());
                        if (Vals > OtherIf.Val.x) OIf = false;
                        break;
                }
                if (!OIf) break;
            }
            if(!OIf) continue;
            Sta.AtkInput(AtkAI.AtkSlot, AtkAI.AtkD, true, true,0,true);
        }

    }
}
