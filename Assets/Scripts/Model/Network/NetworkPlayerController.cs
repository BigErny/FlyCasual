﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerController : NetworkBehaviour {

    private void Start()
    {
        if (isLocalPlayer) Network.CurrentPlayer = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public bool IsServer
    {
        get { return isServer; }
    }

    // TESTS

    [Command]
    public void CmdTest()
    {
        RpcTest();
    }

    [ClientRpc]
    private void RpcTest()
    {
        Messages.ShowInfo("Network test\nLocal: " + isLocalPlayer + "; Client: " + isClient + "; Server: " + isServer);
    }

    [Command]
    public void CmdCallBacksTest()
    {
        Network.ExecuteWithCallBack(CmdRosterTest, CmdShowVariable);
    }

    [Command]
    public void CmdRosterTest()
    {
        Network.AllShipNames = "";
        RpcRosterTest();
    }

    [ClientRpc]
    private void RpcRosterTest()
    {
        string text = (isServer) ? "Hello from server" : "Hello from client";
        text += "\nMy first ship is " + RosterBuilder.TestGetNameOfFirstShipInRoster();
        Network.UpdateAllShipNames(RosterBuilder.TestGetNameOfFirstShipInRoster() + "\n");
        Network.ShowMessage(text);

        Network.FinishTask();
    }

    [Command]
    public void CmdUpdateAllShipNames(string text)
    {
        Network.AllShipNames += text;
    }

    [Command]
    public void CmdShowVariable()
    {
        RpcShowVariable(Network.AllShipNames);
    }

    [ClientRpc]
    private void RpcShowVariable(string text)
    {
        Messages.ShowInfo(text);
    }

    // START OF BATTLE

    [Command]
    public void CmdLoadBattleScene()
    {
        RpcLoadBattleScene();
    }

    [ClientRpc]
    public void RpcLoadBattleScene()
    {
        RosterBuilder.LoadBattleScene();
    }

    // MESSAGES

    [Command]
    public void CmdShowMessage(string text)
    {
        RpcShowMessage(text);
    }

    [ClientRpc]
    private void RpcShowMessage(string text)
    {
        Messages.ShowInfo(text);
    }

    // CALLBACKS

    [Command]
    public void CmdFinishTask()
    {
        Network.ServerFinishTask();
    }
}