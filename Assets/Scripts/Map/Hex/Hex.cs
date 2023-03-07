using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Base;

public class Hex
{
    private HexBehaviour hexBehaviour;

    private HexType hexType;
    public HexType type { get { return hexType; } set { hexType = value; } }

    public Hex(HexType hexType)
    {
        type = hexType;
    }

    public Hex InstantiateHex(GameObject hexPrefab, Transform container)
    {
        GameObject hex = Object.Instantiate(hexPrefab, new Vector3(0,0,0) , Quaternion.identity);
        
        hex.transform.parent = container;
        hexBehaviour = hex.GetComponent<HexBehaviour>();

        return this;
    }

    public void Move(float x, float y)
    {
        float yPos = y * Constant.YOFFSET;

        if (x % 2 == 1)
            yPos += Constant.YOFFSET / 2;

        hexBehaviour.transform.position = new Vector3(x * Constant.XOFFSET, yPos, 0) + Constant.TO_CENTER_OFFSET;
    }

    public void SetObjectName(int x, int y)
    {
        hexBehaviour .name = "Hex" + x + "_" + y;
    }
}
