using System;

[Flags]
public enum WorldLayer
{
    Item = 0b_0001,
    Floor = 0b_0010,
    Furniture = 0b_0100,
    StructureLayers = Floor | Furniture
}
