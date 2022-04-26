using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPathfinder {

    /*
     * Get a path from start to end using A* algorithm
     */
    public static Hex[] GetPath(Hex start, Hex end) {

        //discovered nodes
        HexPriorityQueue openSet = new HexPriorityQueue();
        openSet.Add(start, Heuristic(start, end));

        //cameFrom[h] is the hex preceding h in the final path
        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();

        //g[h] = cost of best path from start to h so far -- default values should be infinity
        Dictionary<Hex, float> g = new Dictionary<Hex, float>();
        g[start] = 0;

        Dictionary<Hex, float> f = new Dictionary<Hex, float>();
        f[start] = Heuristic(start, end);

        HashSet<Hex> done = new HashSet<Hex>();

        while(openSet.Count() > 0) {
            Hex currHex = openSet.Dequeue();

            if(f[currHex] >= Mathf.Infinity) { // no route is possible
                return ReconstructPath(null, null, Mathf.Infinity);
            } else if (currHex == end) { // found the best route
                return ReconstructPath(cameFrom, currHex, g[currHex]);
            }

            Hex[] neighbors = currHex.GetNeighbors();
            foreach(Hex n in neighbors) {
                float tentative_gScore = g[currHex] + n.GetMovementCost();
                if(!g.ContainsKey(n) || tentative_gScore < g[n]) {
                    cameFrom[n] = currHex;
                    g[n] = tentative_gScore;
                    f[n] = g[n] + Heuristic(n, end);
                    openSet.Add(n, f[n]); //may create copies of hexes with different f values
                }
            }
        }

        //didnt find a way to reach end
        return null;
    }

    private static Hex[] ReconstructPath(Dictionary<Hex, Hex> cameFrom, Hex endHex, float totalDist) {
        if (totalDist >= Mathf.Infinity) {
            return null;
        }

        if (!cameFrom.ContainsKey(endHex)) {
            Hex[] path = new Hex[1];
            path[0] = endHex;
            return path;
        }

        List<Hex> finalPath = new List<Hex>();
        finalPath.Add(endHex);

        Hex currHex = endHex;
        while (cameFrom.ContainsKey(currHex)) {
            currHex = cameFrom[currHex];
            finalPath.Add(currHex);
        }

        finalPath.RemoveAt(finalPath.Count - 1);
        finalPath.Reverse();
        return finalPath.ToArray();
    }

    /*
     *  Gives an estimate of the cost from start to end
     */
    private static float Heuristic(Hex start, Hex end) {
        float dist = LinearHeuristic(start, end);
        return dist;
    }

    /*
     * 
     */
    private static float LinearHeuristic(Hex start, Hex end) {
        return Hex.Distance(start, end);
    }
}
