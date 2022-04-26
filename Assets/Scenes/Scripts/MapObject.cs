using System.Collections;
using System.Collections.Generic;

abstract public class MapObject {

    public string name = "No name";

    public Hex hex { get; protected set; }

    public delegate void objectMovedDelegate(Hex oldHex, Hex newHex);
    public event objectMovedDelegate onObjectMoved;

    virtual public void SetHex(Hex newHex) {
        Hex oldHex = this.hex;
        this.hex = newHex;

        if (onObjectMoved != null) {
            onObjectMoved(oldHex, newHex);
        }
    }
}
