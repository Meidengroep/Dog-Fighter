using UnityEngine;
using System.Collections;

public class MatchRPC : RPCHolder
{
    public static void EndMatch(MatchResult result)
    {
        if (StopSend())
            return;

        Channel.GetComponent<NetworkView>().RPC("EndMatchRPC", RPCMode.Others, (int)result);
    }

    public static void EndMatchDefinite()
    {
        if (StopSend())
            return;

        Channel.GetComponent<NetworkView>().RPC("EndMatchDefiniteRPC", RPCMode.Others);
    }

    [RPC]
    private void EndMatchRPC(int result, NetworkMessageInfo info)
    {
        MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();

        matchControl.EndMatch((MatchResult)result);
    }

    [RPC]
    private void EndMatchDefiniteRPC(NetworkMessageInfo info)
    {
        MatchControl matchControl = GameObject.Find(GlobalSettings.MatchControlName).GetComponent<MatchControl>();

        matchControl.MatchFinished();
    }
}
