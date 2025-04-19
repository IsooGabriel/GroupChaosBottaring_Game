using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static BattleManager;
public class UI_BattleBase : MonoBehaviour
{
    [SerializeField] GameObject InfoUI;
    [SerializeField] TextMeshProUGUI TimeTx;
    [SerializeField] TextMeshProUGUI DeathTx;
    [SerializeField] List<UI_Sin_BossUI> BossUIs;

    [SerializeField] TextMeshProUGUI TimeStarTx;
    [SerializeField] TextMeshProUGUI DeathStarTx;

    void LateUpdate()
    {
        if (BTManager.TimeLimSec > 0)
        {
            if (InfoUI.activeSelf != true) InfoUI.SetActive(true);

            var TimeStr = (BTManager.Time / 3600).ToString("D2") + ":" + (BTManager.Time / 60 % 60).ToString("D2");
            if (TimeTx.text != TimeStr) TimeTx.text = TimeStr;

            var DeathStr = "Death:" + BTManager.DeathCount;
            if (DeathTx.text != DeathStr) DeathTx.text = DeathStr;

            var TStarStr = (BTManager.Time >= BTManager.TimeStar * 60 ? "★" : "☆");
            TStarStr += (BTManager.TimeStar / 60).ToString("D2") + ":" + (BTManager.TimeStar % 60).ToString("D2");
            if (TimeStarTx.text != TStarStr) TimeStarTx.text = TStarStr;

            var DStarStr = (BTManager.DeathCount <= BTManager.DeathStar ? "★" : "☆");
            DStarStr += "Death:" + BTManager.DeathStar;
            if (DeathStarTx.text != DStarStr) DeathStarTx.text = DStarStr;
        }
        else if (InfoUI.activeSelf != false) InfoUI.SetActive(false);
        for (int i = 0; i < Mathf.Max(BossUIs.Count, BTManager.BossList.Count); i++)
        {
            bool NDisp = false;
            if (i < BTManager.BossList.Count)
            {
                if (BossUIs.Count <= i) BossUIs.Add(Instantiate(BossUIs[0], BossUIs[0].transform.parent));
                var Sta = BTManager.BossList[i];
                var BUI = BossUIs[i];
                if (Sta == null)
                {
                    NDisp = true;
                }
                else
                {
                    BUI.Sta = Sta;
                    BUI.BaseSet();
                }
            }
            var BossDispIf = i < BTManager.BossList.Count && !NDisp;
            if(BossUIs[i].gameObject.activeSelf != BossDispIf) BossUIs[i].gameObject.SetActive(BossDispIf);
        }

    }
}
