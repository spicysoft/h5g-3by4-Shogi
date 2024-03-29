using Unity.Entities;
using Unity.Tiny.Core2D;
using Shogi;

public struct GameState : IComponentData
{

    //ゲームが進行中なのか、終了しているのか格納します
    public bool IsActive;

    //  どちらのターンなのかを格納します
    public TurnEnum NowTurn;

    // 決着がついたかどうか格納します
    public bool GameEnd;

    //勝者を格納します
    public TurnEnum WinnetNum;
}