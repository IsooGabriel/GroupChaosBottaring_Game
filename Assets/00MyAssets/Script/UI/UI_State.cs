using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Manifesto;
using static DataBase;
using static Statics;
public class UI_State : MonoBehaviour
{
    public State_Base Sta;
    [SerializeField] TextMeshProUGUI NameTx;
    [SerializeField] Image HPBackBar;
    [SerializeField] Image HPBackFill;
    [SerializeField] Image HPMiddleBar;
    [SerializeField] Image HPMiddleFill;
    [SerializeField] Image HPFrontBar;
    [SerializeField] Image HPFrontFill;
    [SerializeField] Image BreakBar;
    [SerializeField] Image BreakFill;
    [SerializeField] TextMeshProUGUI BreakText;
    [SerializeField] List<UI_Sin_Buf> BufUIs;
    float CHPPer;
    private void Start()
    {
        CHPPer = 1;
    }
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        BaseSet();
    }
    public void BaseSet()
    {
        if(NameTx!=null && NameTx.text != Sta.Name) NameTx.text = Sta.Name;
        float HPPer = Float_Cuts( Sta.HP / Mathf.Max(1, Sta.MHP),50);
        Color HPCol = Color.red;
        switch (Sta.Team)
        {
            case 0:
                HPCol = Color.green;
                break;
        }
        float BufChanges = -Sta.BufTPMultGet(Enum_Bufs.毒) / 60f;
        float BufPer = Float_Cuts(BufChanges / Mathf.Max(1, Sta.FMHP),50);
        float ChangeSpeed = 0.01f;
        if (HPPer < CHPPer)
        {
            if(HPBackBar.fillAmount != CHPPer) HPBackBar.fillAmount = CHPPer;
            if(HPBackFill.color != new Color(1f, 0.5f, 0f)) HPBackFill.color = new Color(1f, 0.5f, 0f);
            if(HPMiddleBar.fillAmount != HPPer) HPMiddleBar.fillAmount = HPPer;
            if(HPMiddleFill.color != Color.magenta) HPMiddleFill.color = Color.magenta;
            if(HPFrontBar.fillAmount != HPPer + BufPer) HPFrontBar.fillAmount = HPPer + BufPer;
            if(HPFrontFill.color != HPCol) HPFrontFill.color = HPCol;
            CHPPer = Mathf.Max(CHPPer - ChangeSpeed, HPPer);
        }
        else
        {
            if(HPBackBar.fillAmount != HPPer) HPBackBar.fillAmount = HPPer;
            if(HPBackFill.color != new Color(0.5f, 1f, 0.5f)) HPBackFill.color = new Color(0.5f, 1f, 0.5f);
            if(HPMiddleBar.fillAmount != CHPPer) HPMiddleBar.fillAmount = CHPPer;
            if(HPMiddleFill.color != Color.magenta) HPMiddleFill.color = Color.magenta; 
            if(HPFrontBar.fillAmount != CHPPer + BufPer) HPFrontBar.fillAmount = CHPPer + BufPer;
            if(HPFrontFill.color != HPCol) HPFrontFill.color = HPCol;
            CHPPer = Mathf.Min(CHPPer + ChangeSpeed, HPPer);
        }
        CHPPer = Mathf.Clamp01(CHPPer);

        if (BreakBar != null)
        {
            if (Sta.BreakT <= 0)
            {
                var BreakVPer = Float_Cuts(Sta.BreakV / Mathf.Max(1f, Sta.MBreak),50);
                if(BreakBar.fillAmount != BreakVPer) BreakBar.fillAmount = BreakVPer;
                if(BreakFill.color != Color.cyan) BreakFill.color = Color.cyan;
                if(BreakText.text != "") BreakText.text = "";
            }
            else
            {
                var BreakTPer = Float_Cuts(Sta.BreakT / Mathf.Max(1f, Sta.BreakTime),50);
                if(BreakBar.fillAmount != BreakTPer) BreakBar.fillAmount = BreakTPer;
                var BreakCol = Color.HSVToRGB(Mathf.Repeat(Time.time * 1f, 1f), 1, 1);
                if(BreakFill.color != BreakCol)BreakFill.color = BreakCol;
                if(BreakText.text != "Break!!!") BreakText.text = "Break!!!";
            }
        }

        for (int i = 0; i < Mathf.Max(BufUIs.Count, Sta.Bufs.Count); i++)
        {
            if (i < Sta.Bufs.Count)
            {
                if (BufUIs.Count <= i) BufUIs.Add(Instantiate(BufUIs[0], BufUIs[0].transform.parent));
                var Bufi = Sta.Bufs[i];
                var BufD = DB.Bufs.Find(x => (int)x.Buf == Bufi.ID);
                if (BufD != null)
                {
                    if(BufUIs[i].BackImage.color != BufD.Col) BufUIs[i].BackImage.color = BufD.Col;
                    if(BufUIs[i].Icon.texture != BufD.Icon) BufUIs[i].Icon.texture = BufD.Icon;
                    var IconCol = BufUIs[i].Icon.texture != null ? Color.white : Color.clear;
                    if(BufUIs[i].Icon.color != IconCol) BufUIs[i].Icon.color = IconCol;
                }
                else
                {
                    if(BufUIs[i].BackImage.color != Color.white) BufUIs[i].BackImage.color = Color.white;
                    if(BufUIs[i].Icon.color != Color.clear) BufUIs[i].Icon.color = Color.clear;
                }
                if(BufUIs[i].NameTx.text != ((Enum_Bufs)Bufi.ID).ToString()) BufUIs[i].NameTx.text = ((Enum_Bufs)Bufi.ID).ToString();
                var PowStr = Bufi.Pow > 0 ? Bufi.Pow.ToString() : "";
                if(BufUIs[i].PowTx.text != PowStr)BufUIs[i].PowTx.text = PowStr;
                if (Bufi.TimeMax > 0)
                {
                    var BufTimePer = Float_Cuts(1f - ((float)Bufi.Time / Bufi.TimeMax),30);
                    if(BufUIs[i].TimeImage.fillAmount != BufTimePer) BufUIs[i].TimeImage.fillAmount = BufTimePer;
                }
                else if(BufUIs[i].TimeImage.fillAmount != 0) BufUIs[i].TimeImage.fillAmount = 0;
            }
            if(BufUIs[i].gameObject.activeSelf != i < Sta.Bufs.Count) BufUIs[i].gameObject.SetActive(i < Sta.Bufs.Count);
        }
    }
}
