using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AStarTuple {
    public Hex hex;
    float fscore;

    public AStarTuple(Hex h, float f) {
        hex = h;
        fscore = f;
    }

    public int CompareTo(object otherTuple) {
        AStarTuple other = (AStarTuple)otherTuple;
        return this.fscore < other.fscore ? -1 : 1;
    }
}