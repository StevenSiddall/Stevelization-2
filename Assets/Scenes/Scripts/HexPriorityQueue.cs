using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPriorityQueue {

    private List<AStarTuple> data;

    public HexPriorityQueue() {
        this.data = new List<AStarTuple>();
    }

    public void Add(Hex hex, float fscore) {
        data.Add(new AStarTuple(hex, fscore));
        int ci = data.Count - 1;

        while(ci > 0) {
            int pi = (ci - 1) / 2;

            //TODO: need to consider turn cost not movement cost
            if(data[ci].CompareTo(data[pi]) >= 0) {
                break;
            }
            AStarTuple temp = data[ci];
            data[ci] = data[pi];
            data[pi] = temp;
            ci = pi;
        }
    }

    public Hex Dequeue() {
        if(data.Count == 0) {
            Debug.LogError("Tried to dequeue from and empty priority queue");
            return null;
        }

        int li = data.Count - 1;
        AStarTuple frontHex = data[0];
        data[0] = data[li];
        data.RemoveAt(li);

        --li;
        int pi = 0;
        while (true) {
            int ci = pi * 2 + 1;
            if(ci > li) {
                break;
            }

            int rc = ci + 1;
            if(rc <= li && data[rc].CompareTo(data[ci]) < 0) {
                ci = rc;
            }

            if(data[pi].CompareTo(data[ci]) <= 0) {
                break;
            }

            AStarTuple temp = data[pi];
            data[pi] = data[ci];
            data[ci] = temp;
            pi = ci;
        }

        return frontHex.hex;
    }

    public int Count() {
        return data.Count;
    }

    public bool Contains(Hex hex) {
        for(int i = 0; i < data.Count; i++) {
            if(data[i].hex == hex) {
                return true;
            }
        }
        return false;
    }
}