using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteText : MonoBehaviour
{
    public static readonly string[] RESOURCE_SPRITE = {"<sprite=\"resources\" index=0>",
                                                       "<sprite=\"resources\" index=1>",
                                                       "<sprite=\"resources\" index=2>",
                                                       "<sprite=\"resources\" index=3>"};

    public static readonly string[] ZONE_SPRITE = {"ERR: NO SPRITE FOR CITY CENTER",
                                                   "<sprite=\"zones\" index=0>",
                                                   "<sprite=\"zones\" index=1>",
                                                   "<sprite=\"zones\" index=2>",
                                                   "<sprite=\"zones\" index=3>",
                                                   "<sprite=\"zones\" index=4>",
                                                   "<sprite=\"zones\" index=5>",
                                                   "ERR: NO SPRITE FOR ZONE TYPE NONE"};
}
