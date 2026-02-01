using System.Collections.Generic;

public static class MaskDatabase
{
    private static readonly Dictionary<MaskType, MaskData> Data = new()
    {
        { MaskType.None, new MaskData("No Mask", 0, 0f) },
        { MaskType.Gold, new MaskData("Gold Mask", 20, 0.03f) },
        { MaskType.Blue, new MaskData("Azure Mask", 5, 0.80f) },
        { MaskType.Silver, new MaskData("Silver Mask", 15, 0.07f) },
        { MaskType.Bronze, new MaskData("Bronze Mask", 10, 0.10f) }
    };

    public static MaskData Get(MaskType type) => Data[type];
}