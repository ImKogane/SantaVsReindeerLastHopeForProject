using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClienNetworkTransform : NetworkTransform
{
    private override bool IsServerAuthoritative(){
        return false;
    }
}
