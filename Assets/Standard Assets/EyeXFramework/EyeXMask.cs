//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

public enum EyeXMaskType
{
    None = 0,
    LowRes = 5,
    MediumRes = 10,
    HighRes = 20
}

/// <summary>
/// A mask describing the approximate shape of a non-rectangular interactor.
/// </summary>
public class EyeXMask
{
    public EyeXMask(EyeXMaskType type)
    {
        Type = type;
        MaskData = new byte[Size * Size];
    }

    public EyeXMaskType Type { get; private set; }

    public int Size
    {
        get { return (int)Type; }
    }

    public byte[] MaskData { get; private set; }

    public byte this[int row, int col]
    {
        get
        {
            return MaskData[row * Size + col];
        }

        set
        {
            MaskData[row * Size + col] = value;
        }
    }
}
