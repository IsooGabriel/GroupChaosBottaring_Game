using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BattleManager;
public class UI_EndResult : MonoBehaviour
{
    [SerializeField] GameObject[] EndUIs;
    [SerializeField] Image BackImage;
    [SerializeField] TextMeshProUGUI WinsTx;
    [SerializeField] TextMeshProUGUI StarsTx;
    [SerializeField] TextMeshProUGUI TimeTx;
    [SerializeField] TextMeshProUGUI DeathTx;

    [SerializeField] List<Net_JoinPlayerUI> PlayerUIs;
    void Update()
    {
        for (int i = 0; i < EndUIs.Length; i++)
        {
            if (EndUIs[i].activeSelf != BTManager.End) EndUIs[i].SetActive(BTManager.End);
        }
        if (!BTManager.End) return;
        UISet_Base();
        UISet_Player();
    }
    void UISet_Base()
    {
        Color BackCol;
        float Alpha = BackImage.color.a;
        var WinStr = "";
        if (BTManager.Win)
        {
            BackCol = Color.gray;
            WinStr = "勝利";
        }
        else
        {
            BackCol = Color.red;
            WinStr = "敗北";
        }
        BackCol.a = Alpha;
        if(BackImage.color != BackCol) BackImage.color = BackCol;
        if(WinsTx.text != WinStr) WinsTx.text = WinStr;

        //int CTime = (BTManager.TimeLimSec * 60) - BTManager.Time;
        var StarStr = "";
        StarsTx.text = "";
        for (int i = 0; i < 3; i++) StarStr += i < BTManager.Star ? "★" : "☆";
        if (StarsTx.text != StarStr) StarsTx.text = StarStr;
        var TimeStr = (BTManager.Time / 3600).ToString("D2") + ":" + (BTManager.Time / 60 % 60).ToString("D2");
        if(TimeTx.text != TimeStr) TimeTx.text = TimeStr;
        var DeathStr = "Death:" + BTManager.DeathCount;
        if (DeathTx.text != DeathStr) DeathTx.text = DeathStr;
    }
    void UISet_Player()
    {
        float AddDamMax = 0;
        int AddHitMax = 0;
        float HealMax = 0;
        int BufMax = 0;
        int DBufMax = 0;
        int E_AtkMax = 0;
        float RecDamMax = 0;
        int DeathMax = 0;
        for (int i = 0; i < BTManager.PlayerList.Count; i++)
        {
            var Sta = BTManager.PlayerList[i];
            if (Sta == null) continue;
            AddDamMax = Mathf.Max(AddDamMax, Sta.PLValues.AddDamTotal);
            AddHitMax = Mathf.Max(AddHitMax, Sta.PLValues.AddHitTotal);
            HealMax = Mathf.Max(HealMax, Sta.PLValues.AddHeal);
            BufMax = Mathf.Max(BufMax, Sta.PLValues.AddBuf);
            DBufMax = Mathf.Max(DBufMax, Sta.PLValues.AddDBuf);
            E_AtkMax = Mathf.Max(E_AtkMax, Sta.PLValues.E_AtkCount);
            RecDamMax = Mathf.Max(RecDamMax, Sta.PLValues.ReceiveDam);
            DeathMax = Mathf.Max(DeathMax, Sta.PLValues.DeathCount);
        }
        for (int i = 0; i < Mathf.Max(BTManager.PlayerList.Count, PlayerUIs.Count); i++)
        {
            var Sta = BTManager.PlayerList[i];
            if (PlayerUIs.Count <= i) PlayerUIs.Add(Instantiate(PlayerUIs[0], PlayerUIs[0].transform.parent));
            var SinUI = PlayerUIs[i];
            bool NDisp = false;
            if (Sta != null)
            {
                SinUI.UISet(i + 1, Sta.photonView.Owner, true);
                float[] BarPer = new float[8];
                string[] ValStr = new string[8];
                BarPer[0] = Sta.PLValues.AddDamTotal / Mathf.Max(1f, AddDamMax);
                ValStr[0] = Sta.PLValues.AddDamTotal.ToString("F0");
                BarPer[1] = Sta.PLValues.AddHitTotal / Mathf.Max(1f, AddDamMax);
                ValStr[1] = Sta.PLValues.AddHitTotal.ToString("F0");
                BarPer[2] = Sta.PLValues.AddHeal / Mathf.Max(1f, AddDamMax);
                ValStr[2] = Sta.PLValues.AddHeal.ToString("F0");
                BarPer[3] = Sta.PLValues.AddBuf / Mathf.Max(1f, AddDamMax);
                ValStr[3] = Sta.PLValues.AddBuf.ToString("F0");
                BarPer[4] = Sta.PLValues.AddDBuf / Mathf.Max(1f, AddDamMax);
                ValStr[4] = Sta.PLValues.AddDBuf.ToString("F0");
                BarPer[5] = Sta.PLValues.E_AtkCount / Mathf.Max(1f, AddDamMax);
                ValStr[5] = Sta.PLValues.E_AtkCount.ToString("F0");
                BarPer[6] = Sta.PLValues.ReceiveDam / Mathf.Max(1f, AddDamMax);
                ValStr[6] = Sta.PLValues.ReceiveDam.ToString("F0");
                BarPer[7] = Sta.PLValues.DeathCount / Mathf.Max(1f, AddDamMax);
                ValStr[7] = Sta.PLValues.DeathCount.ToString("F0");
                for (int j = 0; j < 8; j++)
                {
                    if(SinUI.Bars[j].fillAmount != BarPer[j]) SinUI.Bars[0].fillAmount = BarPer[j];
                    if(SinUI.ValTxs[j].text != ValStr[j]) SinUI.ValTxs[0].text = ValStr[j];
                }
            }
            else
            {
                NDisp = true;
            }
            var DispIf = i < BTManager.PlayerList.Count || !NDisp;
            if(SinUI.gameObject.activeSelf != DispIf)SinUI.gameObject.SetActive(DispIf);

        }
    }
}
