using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DataBase;
public class Net_JoinPlayerUI : MonoBehaviour
{
    Photon.Realtime.Player PlayerD;
    [Tooltip("番号テキスト"), SerializeField] TextMeshProUGUI IndexTx;
    [Tooltip("プレイヤー名テキスト"), SerializeField] TextMeshProUGUI NameTx;
    [Tooltip("プレイヤー名テキスト"), SerializeField] TextMeshProUGUI OKsTx;
    [Tooltip("マスター用管理UI"), SerializeField] GameObject MasterOnlyUI;
    [Tooltip("マスター表示"), SerializeField] GameObject IsMasterUI;

    [SerializeField] UI_Sin_Set Chara_UI;
    [SerializeField] List<UI_Sin_Set> FAtk_UIs;
    [SerializeField] List<UI_Sin_Set> BAtk_UIs;
    [SerializeField] List<UI_Sin_Set> Passive_UIs;

    public Image[] Bars;
    public TextMeshProUGUI[] ValTxs;
    public void UISet(int Index,Photon.Realtime.Player Player,bool Plays)
    {
        PlayerD = Player;
        var IndexStr = "[" + Index + "]";
        if(IndexTx.text != IndexStr) IndexTx.text = IndexStr;
        if(NameTx.text != Player.NickName) NameTx.text = Player.NickName;
        if (!Plays)
        {
            var MasterDisp = PhotonNetwork.IsMasterClient && PhotonNetwork.LocalPlayer != Player;
            if(MasterOnlyUI.activeSelf!=MasterDisp)MasterOnlyUI.SetActive(MasterDisp);
            if(IsMasterUI.activeSelf != Player.IsMasterClient) IsMasterUI.SetActive(Player.IsMasterClient);
        }

        var CPro = Player.CustomProperties;
        var OKs = CPro.TryGetValue("OK", out var oOK) ? (bool)oOK : false;
        var OKStr = OKs ? "OK" : "準備中";
        if(OKsTx.text != OKStr) OKsTx.text = OKStr;
        var OKCol = OKs ? new Color(1, 0.5f, 0) : Color.white;
        if(OKsTx.color != OKCol) OKsTx.color = OKCol;
        var CID = CPro.TryGetValue("Chara", out var oCID) ? (int)oCID : 0;
        var CharaData = DB.Charas[CID];
        if(Chara_UI.Name.text != CharaData.Name) Chara_UI.Name.text = CharaData.Name;
        if(Chara_UI.Icon.texture != CharaData.Icon) Chara_UI.Icon.texture = CharaData.Icon;
        for (int i = 0; i < 4; i++)
        {
            #region 表攻撃
            if (FAtk_UIs.Count <= i) FAtk_UIs.Add(Instantiate(FAtk_UIs[0], FAtk_UIs[0].transform.parent));
            var FSinUI = FAtk_UIs[i];
            var FAtkID = CPro.TryGetValue("FAtk_" + i, out var oFAtk) ? (int)oFAtk : 0;
            Data_Atk FAtkData = null;
            switch (i)
            {
                case 0:
                    FAtkData = DB.N_Atks[FAtkID];
                    break;
                case 1:
                case 2:
                    FAtkData = DB.S_Atks[FAtkID];
                    break;
                case 3:
                    FAtkData = DB.E_Atks[FAtkID];
                    break;
            }
            if(FSinUI.Name.text != FAtkData.Name) FSinUI.Name.text = FAtkData.Name;
            if(FSinUI.Icon.texture != FAtkData.Icon) FSinUI.Icon.texture = FAtkData.Icon;
            #endregion
            #region 裏攻撃
            if (BAtk_UIs.Count <= i) BAtk_UIs.Add(Instantiate(BAtk_UIs[0], BAtk_UIs[0].transform.parent));
            var BSinUI = BAtk_UIs[i];
            var BAtkID = CPro.TryGetValue("BAtk_" + i, out var oBAtk) ? (int)oBAtk : 0;
            Data_Atk BAtkData = null;
            switch (i)
            {
                case 0:
                    BAtkData = DB.N_Atks[BAtkID];
                    break;
                case 1:
                case 2:
                    BAtkData = DB.S_Atks[BAtkID];
                    break;
                case 3:
                    BAtkData = DB.E_Atks[BAtkID];
                    break;
            }
            if(BSinUI.Name.text != BAtkData.Name) BSinUI.Name.text = BAtkData.Name;
            if(BSinUI.Icon.texture != BAtkData.Icon) BSinUI.Icon.texture = BAtkData.Icon;
            #endregion
            #region パッシブ
            if (Passive_UIs.Count <= i) Passive_UIs.Add(Instantiate(Passive_UIs[0], Passive_UIs[0].transform.parent));
            var PSinUI = Passive_UIs[i];
            var PID = CPro.TryGetValue("Passive_" + i, out var oPassive) ? (int)oPassive : 0;
            var PassiveData = DB.Passives[PID];
            if(PSinUI.Name.text != PassiveData.Name) PSinUI.Name.text = PassiveData.Name;
            if(PSinUI.Icon.texture != PassiveData.Icon) PSinUI.Icon.texture = PassiveData.Icon;
            #endregion
        }
    }
    public void MasterChange()
    {
        PhotonNetwork.SetMasterClient(PlayerD);
    }
    public void KickPlayer()
    {
        PhotonNetwork.CloseConnection(PlayerD);
    }
}
