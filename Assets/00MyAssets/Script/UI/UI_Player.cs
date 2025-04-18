using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
using static PlayerValue;
using static Manifesto;
using UnityEngine.SceneManagement;
public class UI_Player : UI_State
{
    [SerializeField] Player_Cont PCont;
    [SerializeField] Player_Atk PAtk;
    [SerializeField] Player_Target PTarget;

    [SerializeField] Image DeathPanel;
    [SerializeField] float DeathAlphaSpeed;
    [SerializeField] float DeathAlphaMax;

    [SerializeField] Image MPBar;
    [SerializeField] Image MPFill;

    [SerializeField] Image AtkTimeBar;
    [SerializeField] TextMeshProUGUI AtkNameTx;
    [SerializeField] TextMeshProUGUI AtkBranchTx;
    [SerializeField] RectTransform BranchTrans;
    [SerializeField] List<Slider> BranchSliders;

    [SerializeField] Image TargetImage;

    [SerializeField] TextMeshProUGUI AtkFBTx;
    [SerializeField] UI_Sin_Atk[] AtkUIs;
    [SerializeField] TextMeshProUGUI StateInfoTx;
    [SerializeField] BinaryUIAnimationo binaryUIAnimationo;
    [SerializeField] GameObject DebugUI;

    #region 関数

    /// <summary>
    /// ステータスUIを更新するメソッド
    /// </summary>
    /// <param name="index"></param>
    /// <param name="AtkD"></param>
    /// <param name="AtkCTs"></param>
    private void UpdateStatus(int index, Data_Atk AtkD, Class_Sta_AtkCT AtkCTs)
    {
        // AtkUIs[index] の UI 更新処理
        for (int j = 0; j < AtkUIs[j].ChengedImages.Length; j++)
        {
            // すべての変更画像を非表示にする
            AtkUIs[index].ChengedImages[j].SetActive(false);

            // 名前の表示を更新（存在する場合）
            if (AtkUIs[index].Name[j])
            {
                AtkUIs[index].Name[j].text = AtkD.Name;
            }

            // アイコンの表示を更新（存在する場合）
            if (AtkUIs[index].Icon[j])
            {
                AtkUIs[index].Icon[j].texture = AtkD.Icon;
            }

            // クールタイムの画像更新（存在する場合）
            if (AtkUIs[index].CTImage[j])
            {
                if (AtkCTs != null)
                {
                    // クールタイムの進行状況を計算して更新
                    AtkUIs[index].CTImage[j].fillAmount = ((float)AtkCTs.CT / Mathf.Max(1, AtkCTs.CTMax));
                }
                else AtkUIs[index].CTImage[j].fillAmount = 0;
            }

            // チャージ量の画像更新（存在する場合）
            if (AtkUIs[index].ChargeImage[j])
            {
                if (AtkD.SPUse > 0)
                {
                    // スタミナ（SP）の使用量に対する割合を計算して更新
                    AtkUIs[index].ChargeImage[j].fillAmount = (float)Sta.SP / AtkD.SPUse;

                    // スタミナ不足の場合、背景を灰色に変更
                    //if (Sta.SP < AtkD.SPUse && AtkUIs[index].BackImage[j])
                    //{
                    //    AtkUIs[index].BackImage[j].color = Color.gray;
                    //}
                }
                else AtkUIs[index].ChargeImage[j].fillAmount = 0;
            }
            AtkUIs[index].FullImage.SetActive(AtkD.SPUse > 0 && Sta.SP >= AtkD.SPUse);
        }

        // 変更を反映するための画像を表示
        AtkUIs[index].ChengedImages[!PAtk.Backs ? 0 : 1].SetActive(true);
    }

    #endregion
    private void Start()
    {
        var DeathColor = DeathPanel.color;
        DeathColor.a = 0;
        DeathPanel.color = DeathColor;
    }
    void LateUpdate()
    {
        BaseSet();

        var DeathColor = DeathPanel.color;
        if (Sta.HP <= 0) DeathColor.a += 0.01f * DeathAlphaSpeed;
        else DeathColor.a -= 0.05f * DeathAlphaSpeed;
        DeathColor.a = Mathf.Clamp(DeathColor.a, 0f, DeathAlphaMax);
        DeathPanel.color = DeathColor;

        MPBar.fillAmount = Sta.MP / Mathf.Max(1, Sta.FMMP);
        MPFill.color = Sta.LowMP ? Color.red : Color.white;

        var TargetColor = PTarget.TargetOff ? Color.white : new Color(1, 0.5f, 0);
        TargetColor.a = TargetImage.color.a;
        TargetImage.color = TargetColor;

        if (Sta.AtkD != null)
        {
            var AtkD = Sta.AtkD;
            AtkTimeBar.fillAmount = Sta.AtkTime / Mathf.Max(1f, AtkD.EndTime);
            AtkNameTx.text = AtkD.Name;
            AtkBranchTx.text = "";
            if (AtkD.BranchInfos.Count > 0)
            {
                var BranchGet = AtkD.BranchInfos.Find(x => x.BID == Sta.AtkBranch);
                if(BranchGet!=null)AtkBranchTx.text = BranchGet.Name;
            }
            var BranchIndexs = new List<int>();
            if(AtkD.Branchs!=null)
                for(int i = 0; i < AtkD.Branchs.Length; i++)
                {
                    var BranchD = AtkD.Branchs[i];
                    for(int j = 0; j < BranchD.BranchNums.Length; j++)
                    {
                        if(BranchD.BranchNums[j] < 0 || Sta.AtkBranch == BranchD.BranchNums[j])
                        {
                            BranchIndexs.Add(i);
                            break;
                        }
                    }
                }
            for (int i = 0; i < Mathf.Max(BranchIndexs.Count, BranchSliders.Count); i++)
            {
                if (BranchSliders.Count <= i) BranchSliders.Add(Instantiate(BranchSliders[0], BranchSliders[0].transform.parent));

                var SinUI = BranchSliders[i];
                if (i < BranchIndexs.Count)
                {
                    var BranchInfo = AtkD.Branchs[BranchIndexs[i]];
                    if (BranchInfo.HideUI)
                    {
                        SinUI.gameObject.SetActive(false);
                        continue;
                    }
                    SinUI.value = ((BranchInfo.Times.x + BranchInfo.Times.y) / 2f) / Mathf.Max(1f, AtkD.EndTime);
                    var TRectSize = SinUI.targetGraphic.rectTransform.sizeDelta;
                    TRectSize.x = BranchTrans.sizeDelta.x * ((BranchInfo.Times.y - BranchInfo.Times.x + 1f) / Mathf.Max(1f, AtkD.EndTime));
                    SinUI.targetGraphic.rectTransform.sizeDelta = TRectSize;
                    Color SliderCol = Color.black;
                    if (BranchInfo.BranchColor.a > 0) SliderCol = BranchInfo.BranchColor;
                    else if (BranchInfo.Ifs.Length > 0)
                    {
                        SliderCol = Color.green;
                        for (int j = 0; j < BranchInfo.Ifs.Length; j++)
                        {
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.攻撃単入力)
                            {
                                SliderCol = Color.yellow;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.攻撃長入力)
                            {
                                SliderCol = Color.magenta;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.空中)
                            {
                                SliderCol = Color.cyan;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.攻撃未入力 || BranchInfo.Ifs[j] == Enum_AtkIf.攻撃未長入力)
                            {
                                SliderCol = Color.white;
                                break;
                            }
                            if (BranchInfo.Ifs[j] == Enum_AtkIf.MP無し)
                            {
                                SliderCol = new Color(1f,0.8f,0.8f);
                                break;
                            }
                        }

                    }

                    SliderCol.a = SinUI.targetGraphic.color.a;
                    SinUI.targetGraphic.color = SliderCol;


                }
                SinUI.gameObject.SetActive(i < BranchIndexs.Count);
            }
        }
        else
        {
            AtkTimeBar.fillAmount = 0;
            AtkNameTx.text = "";
            AtkBranchTx.text = "";
            for(int i = 0; i < BranchSliders.Count; i++)
            {
                BranchSliders[i].gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < AtkUIs.Length; i++)
        {
            Data_Atk AtkD = null;
            bool Input = false;
            int Slot = i;
            AtkFBTx.text = !PAtk.Backs ? "表" : "裏";
            binaryUIAnimationo.UpdateAnimation(PAtk.Backs);

            var Atks = PriSetGet.AtkGet(PAtk.Backs);
            switch (i)
            {
                case 0:
                    AtkD = DB.N_Atks[Atks.N_AtkID];
                    Input = PCont.NAtk_Stay;
                    break;
                case 1:
                    AtkD = DB.S_Atks[Atks.S1_AtkID];
                    Input = PCont.S1Atk_Stay;
                    break;
                case 2:
                    AtkD = DB.S_Atks[Atks.S2_AtkID];
                    Input = PCont.S2Atk_Stay;
                    break;
                case 3:
                    AtkD = DB.E_Atks[Atks.E_AtkID];
                    Input = PCont.EAtk_Stay;
                    Slot = 10;
                    break;
            }

            foreach (var animation in AtkUIs[i].imageAnimation)
            {
                // 入力があった場合にアニメーションをPressedに変更
                animation.ChangeStatu(Input ? UISystem_Gabu.AnimatorStatu.Pressed : UISystem_Gabu.AnimatorStatu.Normal);
            }


            Sta.AtkCTs.TryGetValue(Slot, out var AtkCTs);
            UpdateStatus(i, AtkD, AtkCTs);
        }
        StateInfoTx.text = "MHP:" + Sta.FMHP;
        StateInfoTx.text += "\nMMP:" + Sta.FMMP;
        StateInfoTx.text += "\nAtk:" + Sta.FAtk;
        StateInfoTx.text += "\nDef:" + Sta.FDef;
        DebugUI.SetActive(SceneManager.GetActiveScene().buildIndex == 1);
    }
}
