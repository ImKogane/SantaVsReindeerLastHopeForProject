using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientNetworkAnimator : NetworkAnimator
{
    private override bool IsServerAuthoritative(){
        return false;
    }
}
