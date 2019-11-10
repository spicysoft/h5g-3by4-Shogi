using Unity.Entities;
using Unity.Mathematics;
using Shogi;

public struct GridComp : IComponentData
{
    public int2 GridNum;

    public Pieces GridState;

    public TurnEnum Owner;

    public bool TapFlag;
}
