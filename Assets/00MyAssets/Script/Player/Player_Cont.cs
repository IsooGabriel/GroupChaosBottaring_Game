﻿using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using static BattleManager;
using static Statics;
public class Player_Cont : MonoBehaviourPun
{
    public int AIID = 0;
    public bool IsMoblie = false;
    [SerializeField] PlayerInput PI;
    [SerializeField] Player_Moblie Moblie;
    [SerializeField] GameObject MoblieUI;
    public Vector2 Move;
    public Vector2 Look;
    public bool Jump_Enter;
    public bool Dash_Enter;
    public bool Change_Enter;
    public bool Target_Stay;
    public bool NAtk_Enter;
    public bool NAtk_Stay;
    public bool S1Atk_Enter;
    public bool S1Atk_Stay;
    public bool S2Atk_Enter;
    public bool S2Atk_Stay;
    public bool EAtk_Enter;
    public bool EAtk_Stay;

    private void Start()
    {
        if (!photonView.IsMine) return;
        IsMoblie = false;
        if (Application.platform == RuntimePlatform.Android) IsMoblie = true;
        if (Application.platform == RuntimePlatform.IPhonePlayer) IsMoblie = true;
        MoblieUI.SetActive(IsMoblie);
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        MoblieUI.SetActive(IsMoblie);
        ResetInput();
        switch (AIID)
        {
            case 0:PICont(); PIMoblie(); break;
            case 1: AILV0(); break;

        }

    }
    void ResetInput()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
        Jump_Enter = false;
        Dash_Enter = false;
        Change_Enter = false;
        NAtk_Enter = false;
        NAtk_Stay= false;
        S1Atk_Enter= false;
        S1Atk_Stay= false;
        S2Atk_Enter = false;
        S2Atk_Stay = false;
        EAtk_Enter = false;
        EAtk_Stay= false;
    }
    void PICont()
    {
        Move = PI.actions["Move"].ReadValue<Vector2>();
        Look = PI.actions["Look"].ReadValue<Vector2>();
        Jump_Enter = PI.actions["Jump"].triggered;
        Dash_Enter = PI.actions["Dash"].triggered;
        Change_Enter = PI.actions["Change"].triggered;
        Target_Stay = PI.actions["Target"].IsPressed();
        NAtk_Enter = PI.actions["N_Atk"].triggered;
        NAtk_Stay = PI.actions["N_Atk"].IsPressed();
        S1Atk_Enter = PI.actions["S1_Atk"].triggered;
        S1Atk_Stay = PI.actions["S1_Atk"].IsPressed();
        S2Atk_Enter = PI.actions["S2_Atk"].triggered;
        S2Atk_Stay = PI.actions["S2_Atk"].IsPressed();
        EAtk_Enter = PI.actions["E_Atk"].triggered;
        EAtk_Stay = PI.actions["E_Atk"].IsPressed();
    }
    void PIMoblie()
    {
        if (!IsMoblie) return;
        if (Moblie.MoveInputs.magnitude >= 0.1f) Move = Moblie.MoveInputs;
        if (Moblie.LookInputs.magnitude >= 0.1f) Look = Moblie.LookInputs;
        if (Moblie.Jump_Enter) Jump_Enter = true;
        if (Moblie.Dash_Enter) Dash_Enter = true;
        if (Moblie.Change_Enter) Change_Enter = true;
        if (Moblie.Target_Stay) Target_Stay = true;
        if (Moblie.Atk_Enters[0]) NAtk_Enter = true;
        if (Moblie.Atk_Stays[0]) NAtk_Stay = true;
        if (Moblie.Atk_Enters[1]) S1Atk_Enter = true;
        if (Moblie.Atk_Stays[1]) S1Atk_Stay = true;
        if (Moblie.Atk_Enters[2]) S2Atk_Enter = true;
        if (Moblie.Atk_Stays[2]) S2Atk_Stay = true;
        if (Moblie.Atk_Enters[3]) EAtk_Enter = true;
        if (Moblie.Atk_Stays[3]) EAtk_Stay = true;

    }
    void AILV0()
    {
        Move = new Vector2(Random.value - 0.5f, Random.value - 0.5f);
        Change_Enter = Random.value <= 0.02f;
        NAtk_Enter = Random.value <= 0.7f;
        S1Atk_Enter = Random.value <= 0.1f;
        S2Atk_Enter = Random.value <= 0.1f;
        EAtk_Enter = Random.value <= 0.05f;
    }

}
