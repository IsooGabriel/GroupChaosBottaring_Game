using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static Statics;
public class UI_AddInfos : MonoBehaviour
{
    [SerializeField] State_Base Sta;
    [SerializeField] TextMeshProUGUI DamTx;
    [SerializeField] List<Image> DamValIm;
    [SerializeField] TextMeshProUGUI HitTx;
    [SerializeField] List<Image> HitValIm;
    float DamMax;
    float HitMax;
    private void Update()
    {
        int DamT = 0;
        int DamM = 0;
        int HitT = 0;
        int HitM = 0;

        for (int i = 0; i < Sta.PLValues.AddDams.Length; i++)
        {
            DamT += Sta.PLValues.AddDams[i];
            DamM = Mathf.Max(DamM, Sta.PLValues.AddDams[i]);
            HitT += Sta.PLValues.AddHits[i];
            HitM = Mathf.Max(HitM, Sta.PLValues.AddHits[i]);
        }
        var DPS = DamT / (float)Sta.PLValues.AddDams.Length;
        DamMax = Mathf.Max(DamMax,DPS);
        var HPS = HitT / (float)Sta.PLValues.AddDams.Length;
        HitMax = Mathf.Max(HitMax, HPS);

        var DamStr = "TDam" + Sta.PLValues.AddDamTotal.ToString("F0");
        DamStr += "\nDPS" + DPS.ToString("F0");
        DamStr += "\nMax" + DamMax.ToString("F0");
        if (DamTx.text != DamStr) DamTx.text = DamStr;
        for (int i = 0; i < Sta.PLValues.AddDams.Length; i++)
        {
            if (DamValIm.Count <= i) DamValIm.Add(Instantiate(DamValIm[0], DamValIm[0].transform.parent));
            var DamPer = Float_Cuts(Sta.PLValues.AddDams[i] / Mathf.Max(DamM, 1f), 20);
            if(DamValIm[i].fillAmount != DamPer) DamValIm[i].fillAmount = DamPer;
        }
        var HitStr = "THit" + Sta.PLValues.AddHitTotal;
        HitStr += "\nHPS" + HPS.ToString("F1");
        HitStr += "\nMax" + HitMax.ToString("F1");
        if(HitTx.text != HitStr) HitTx.text = HitStr;
        for (int i = 0; i < Sta.PLValues.AddDams.Length; i++)
        {
            if (HitValIm.Count <= i) HitValIm.Add(Instantiate(HitValIm[0], HitValIm[0].transform.parent));
            var HitPer = Float_Cuts(Sta.PLValues.AddHits[i] / Mathf.Max(HitM, 1f), 20);
            if(HitValIm[i].fillAmount != HitPer) HitValIm[i].fillAmount = HitPer;
        }
    }
    public void AddReset()
    {
        DamMax = 0;
        HitMax = 0;
        Sta.AddInfoReset();
    }
}
