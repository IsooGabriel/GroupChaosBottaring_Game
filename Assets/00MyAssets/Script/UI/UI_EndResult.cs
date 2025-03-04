﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BattleManager;
public class UI_EndResult : MonoBehaviour
{
    [SerializeField] Image BackImage;
    [SerializeField] TextMeshProUGUI WinsTx;
    [SerializeField] TextMeshProUGUI StartsTx;
    [SerializeField] TextMeshProUGUI TimeTx;
    [SerializeField] TextMeshProUGUI DeathTx;

    [SerializeField] List<Net_JoinPlayerUI> PlayerUIs;
    void Update()
    {
        BackImage.gameObject.SetActive(BTManager.End);
        if (!BTManager.End) return;
        UISet_Base();
        UISet_Player();
    }
    void UISet_Base()
    {
        Color BackCol;
        float Alpha = BackImage.color.a;
        if (BTManager.Win)
        {
            BackCol = Color.gray;
            WinsTx.text = "勝利";
        }
        else
        {
            BackCol = Color.red;
            WinsTx.text = "敗北";
        }
        BackCol.a = Alpha;
        BackImage.color = BackCol;

        int CTime = (BTManager.TimeLimSec * 60) - BTManager.Time;
        StartsTx.text = "";
        for (int i = 0; i < 3; i++) StartsTx.text += i < BTManager.Star ? "★" : "☆";
        TimeTx.text = (BTManager.Time / 3600).ToString("D2") + ":" + (BTManager.Time / 60 % 60).ToString("D2");
        DeathTx.text = "Death:" + BTManager.DeathCount;
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
            AddDamMax = Mathf.Max(AddDamMax, Sta.AddDamTotal);
            AddHitMax = Mathf.Max(AddHitMax, Sta.AddHitTotal);
            HealMax = Mathf.Max(HealMax, Sta.AddHeal);
            BufMax = Mathf.Max(BufMax, Sta.AddBuf);
            DBufMax = Mathf.Max(DBufMax, Sta.AddDBuf);
            E_AtkMax = Mathf.Max(E_AtkMax, Sta.E_AtkCount);
            RecDamMax = Mathf.Max(RecDamMax, Sta.ReceiveDam);
            DeathMax = Mathf.Max(DeathMax, Sta.DeathCount);
        }
        for (int i = 0; i < Mathf.Max(BTManager.PlayerList.Count, PlayerUIs.Count); i++)
        {
            var Sta = BTManager.PlayerList[i];
            if (PlayerUIs.Count <= i) PlayerUIs.Add(Instantiate(PlayerUIs[0], PlayerUIs[0].transform.parent));
            var SinUI = PlayerUIs[i];
            bool NDisp = false;
            if (Sta != null)
            {
                SinUI.UISet(i,Sta.photonView.Owner,true);
                SinUI.Bars[0].fillAmount = Sta.AddDamTotal / Mathf.Max(1f, AddDamMax);
                SinUI.ValTxs[0].text = Sta.AddDamTotal.ToString("F0");
                SinUI.Bars[1].fillAmount = Sta.AddHitTotal / Mathf.Max(1f, AddHitMax);
                SinUI.ValTxs[1].text = Sta.AddHitTotal.ToString("F0");
                SinUI.Bars[2].fillAmount = Sta.AddHeal / Mathf.Max(1f, HealMax);
                SinUI.ValTxs[2].text = Sta.AddHeal.ToString("F0");
                SinUI.Bars[3].fillAmount = Sta.AddBuf / Mathf.Max(1f, BufMax);
                SinUI.ValTxs[3].text = Sta.AddBuf.ToString("F0");
                SinUI.Bars[4].fillAmount = Sta.AddDBuf / Mathf.Max(1f, DBufMax);
                SinUI.ValTxs[4].text = Sta.AddDBuf.ToString("F0");
                SinUI.Bars[5].fillAmount = Sta.E_AtkCount / Mathf.Max(1f, E_AtkMax);
                SinUI.ValTxs[5].text = Sta.E_AtkCount.ToString("F0");
                SinUI.Bars[6].fillAmount = Sta.ReceiveDam / Mathf.Max(1f, RecDamMax);
                SinUI.ValTxs[6].text = Sta.ReceiveDam.ToString("F0");
                SinUI.Bars[7].fillAmount = Sta.DeathCount / Mathf.Max(1f, DeathMax);
                SinUI.ValTxs[7].text = Sta.DeathCount.ToString("F0");
            }
            else
            {
                NDisp = true;
            }
            SinUI.gameObject.SetActive(i < BTManager.PlayerList.Count || !NDisp);

        }
    }
}
