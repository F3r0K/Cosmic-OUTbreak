[System.Serializable]
public struct blittableboolean
{
    public byte boolValue;
    public byte padding1;
    public byte padding2;
    public byte padding3;

    public blittableboolean(bool value)
    {
        boolValue = (byte)(value ? 1 : 0);
        padding1 = 0;
        padding2 = 0;
        padding3 = 0;
    }

    public static implicit operator bool(blittableboolean value)
    {
        return value.boolValue == 255;
    }

    public static implicit operator blittableboolean(bool value)
    {
        return new blittableboolean(value);
    }

    public override string ToString()
    {
        if (boolValue == 255)
            return "true";

        return "false";
    }
}