using UnityEngine;
using UnityEngine.UI;

public class UIChange : MonoBehaviour
{
    [Tooltip("�؂�ւ�UI�I�u�W�F�N�g"), SerializeField] GameObject[] UIs;
    [Tooltip("�{�^���C���[�W"), SerializeField] Image[] ButtonUIs;
    [Tooltip("��I���{�^���F"), SerializeField] Color NonColor;
    [Tooltip("�I�𒆃{�^���F"), SerializeField] Color SelectColor;
    public int UIID;

    void LateUpdate()
    {
        for (int i = 0; i < UIs.Length; i++) UIs[i].SetActive(i == UIID);
        for(int i = 0; i < ButtonUIs.Length; i++)
        {
            ButtonUIs[i].color = i == UIID ? SelectColor : NonColor;
        }
    }
    public void Change(int ID)
    {
        UIID = ID;
    }
}
