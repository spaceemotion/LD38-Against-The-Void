using System;
using UnityEngine;

public static class Utils {

    public static bool HasLayer(this GameObject go, LayerMask layer) {
        return ((1 << go.layer) & layer.value) != 0;
    }

}

