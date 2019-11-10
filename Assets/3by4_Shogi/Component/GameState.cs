using Unity.Entities;
using Unity.Tiny.Core2D;
using Shogi;

public struct GameState : IComponentData
{

    //ƒQ[ƒ€‚ªis’†‚È‚Ì‚©AI—¹‚µ‚Ä‚¢‚é‚Ì‚©Ši”[‚µ‚Ü‚·
    public bool IsActive;

    //  ‚Ç‚¿‚ç‚Ìƒ^[ƒ“‚È‚Ì‚©‚ğŠi”[‚µ‚Ü‚·
    public TurnEnum NowTurn;

    // Œˆ’…‚ª‚Â‚¢‚½‚©‚Ç‚¤‚©Ši”[‚µ‚Ü‚·
    public bool GameEnd;

    //ŸÒ‚ğŠi”[‚µ‚Ü‚·
    public TurnEnum WinnetNum;
}