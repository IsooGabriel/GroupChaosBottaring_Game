using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Statics : MonoBehaviour
{
    static public int V2Int_Rand(Vector2Int V2,bool YMind=false)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        return Random.Range(V2.x, V2.y+1);
    }
    static public string V2Int_Rand_Str(Vector2Int V2, bool YMind = false)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        if (V2.x != V2.y) return V2.x + "～" + V2.y;
        else return V2.x.ToString();
    }
    static public float V2_Rand_Float(Vector2 V2, bool YMind = false)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        return Random.Range(V2.x, V2.y);
    }
    static public string V2_Rand_Str(Vector2 V2, bool YMind = false,int Keta = 1)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        if (V2.x != V2.y) return V2.x.ToString("F"+Keta) + "～" + V2.y.ToString("F" + Keta);
        else return V2.x.ToString("F" + Keta);
    }
    static public string V2Int_Rand_Str(Vector2 V2, bool YMind = false)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        if (V2.x != V2.y) return V2.x + "～" + V2.y;
        else return V2.x.ToString();
    }
    static public int V2_Rand_IntPer(Vector2 V2, bool YMind = false)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        float FCount = Random.Range(V2.x, V2.y);
        int IConut = Mathf.FloorToInt(FCount);
        float Countsd = FCount - IConut;
        if(Countsd>=0.01) IConut += Countsd >= Random.value ? 1 : 0;
        return IConut;
    }
    static public string V2_Rand_IntPerStr(Vector2 V2, bool YMind = false)
    {
        if (YMind && V2.x > V2.y) V2.x = V2.y;
        float XFConut = V2.x;
        int XICount = Mathf.FloorToInt(V2.x);
        float XCountsd = XFConut - XICount;
        string XStr = XICount.ToString();
        if(XCountsd>=0.01f) XStr +="+"+ (XCountsd * 100f).ToString("F0") + "%";
        if (V2.x != V2.y)
        {
            float YFConut = V2.y;
            int YICount = Mathf.FloorToInt(V2.y);
            float YCountsd = YFConut - YICount;
            string YStr = YICount.ToString();
            if (YCountsd >= 0.01f) YStr += "+" + (YCountsd * 100f).ToString("F0") + "%";
            return XStr + "～" + YStr;
        }
        else return XStr;

    }
    static public int Int_NegRand(int I)
    {
        return Random.Range(-I, I+1);
    }
    static public float Float_NegRand(float F)
    {
        return Random.Range(-F, F);
    }
    static public float HoriDistance(Vector3 V1,Vector3 V2)
    {
        return Vector2.Distance(new Vector2(V1.x, V1.z), new Vector2(V2.x, V2.z));
    }
    static public bool TeamCheck(State_Base S1,State_Base S2,bool EHit=true,bool FHit=false,bool MHit=false)
    {
        if (S1 == S2) return MHit;
        else if (S1.Team == S2.Team) return FHit;
        else return EHit;
    }
    static public string Float_KetaString(float F)
    {
        float FBase = F;
        int Tani = 0;
        for(int i = 0; i < 6; i++)
        {
            if (Mathf.Abs(F) >= 100000000)
            {
                F /= 100000000;
                Tani += 2;
            }
            else if (Mathf.Abs(F) >= 10000)
            {
                F /= 10000;
                Tani++;
            }
            else break;
        }
        string KetaStr = "";
        switch (Tani)
        {
            case 0: KetaStr = "";break;
            case 1: KetaStr = "万"; break;
            case 2: KetaStr = "億"; break;
            case 3: KetaStr = "兆"; break;
            case 4: KetaStr = "京"; break;
            case 5: KetaStr = "亥"; break;
            default:KetaStr = "Over"; break;
        }
        int SKeta = 0;
        if (Tani > 0) {
            if (Mathf.Abs(F) >= 1000) SKeta = 0;
            else if (Mathf.Abs(F) >= 100) SKeta = 1;
            else if (Mathf.Abs(F) >= 10) SKeta = 2;
            else SKeta = 3;
        }
        if (KetaStr != "Over") return F.ToString("F" + SKeta) + KetaStr;
        else return FBase.ToString("E3");

    }
    static public float String_KetaFloat(string S)
    {
        int Tani = 0;
        switch (S.Substring(S.Length-1,1))
        {
            case "万": Tani = 1; break;
            case "億": Tani = 2; break;
            case "兆": Tani = 3; break;
            case "京": Tani = 4; break;
            case "亥": Tani = 5; break;
        }
        string sd = S;
        if(Tani>0)sd = S.Substring(0,S.Length-1);
        float Val = float.Parse(sd, CultureInfo.InvariantCulture);
        Val *= Mathf.Pow(10000, Tani);
        return Val;
    }
    static public bool V3IntTimeCheck(int Times,Vector3Int TimeIf)
    {
        return (TimeIf.x <= Times && Times <= TimeIf.y && (Times - TimeIf.x) % Mathf.Max(1, TimeIf.z) == 0);
    }
    /// <summary>floatをn倍して四捨五入しnで割り値を調整する</summary>
    static public float Float_Cuts(float F,int Cuts)
    {
        return Mathf.RoundToInt(F * Cuts) / (float)Cuts;
    }
    static List<GameObject> Strages = new List<GameObject>();
    static public void ObjStrageParent(GameObject Target, string StrageName)
    {
        GameObject StrageObj=null;
        for(int i = Strages.Count - 1; i >= 0; i--)
        {
            if (Strages[i] == null) Strages.RemoveAt(i);
            else if (Strages[i].name == StrageName)
            {
                StrageObj = Strages[i];
                break;
            }
        }
        if (!StrageObj)
        {
            StrageObj = new GameObject(StrageName);
            Strages.Add(StrageObj);
        }
        Target.transform.parent = StrageObj.transform;

    }
    static public Color Rainbow(float OffSet=0f,float S=1f,float V=1f)
    {
        return Color.HSVToRGB(Mathf.Repeat(Time.fixedTime+OffSet, 1f), S, V);
    }
    static public Color YellowNu { get { return new Color(1, 1, 0); } }
    static public Color ColChange(Color Base,float R,float G,float B,float A=-1)
    {
        return new Color(Base.r + R, Base.g + G, Base.b + B, A < 0 ? Base.a : A);
    }
}
