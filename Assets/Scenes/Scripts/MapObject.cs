using System.Collections;
using System.Collections.Generic;

abstract public class MapObject {

    public string name = "No name";
    public float hitPoints = 100f;

    public Hex hex { get; protected set; }

    public delegate void objectMovedDelegate(Hex oldHex, Hex newHex);
    public event objectMovedDelegate onObjectMoved;

    virtual public void setHex(Hex newHex) {
        Hex oldHex = this.hex;
        this.hex = newHex;

        if (onObjectMoved != null) {
            onObjectMoved(oldHex, newHex);
        }
    }
}
