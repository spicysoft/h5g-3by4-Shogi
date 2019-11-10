using Unity.Entities;
using Unity.Tiny.Core2D;
using Shogi;

public struct GameState : IComponentData
{

    //�Q�[�����i�s���Ȃ̂��A�I�����Ă���̂��i�[���܂�
    public bool IsActive;

    //  �ǂ���̃^�[���Ȃ̂����i�[���܂�
    public TurnEnum NowTurn;

    // �������������ǂ����i�[���܂�
    public bool GameEnd;

    //���҂��i�[���܂�
    public TurnEnum WinnetNum;
}