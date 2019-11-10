using Unity.Entities;
using Unity.Collections;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.UILayout;
using Unity.Tiny.UIControls;
using Unity.Mathematics;
using Shogi;

[UpdateAfter(typeof(BoardMan))]
public class PlayerInput : ComponentSystem
{
    EntityQueryDesc GirdEntityDesc;
    EntityQuery GridEntity;

    EntityQueryDesc CanvasDesc;
    EntityQuery CanvasEntity;

    protected override void OnCreate()
    {
        /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

        GirdEntityDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(RectTransform), typeof(Sprite2DRenderer), typeof(PointerInteraction), typeof(Button), typeof(GridComp) },
        };

        /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
        //�쐬�����N�G���̌��ʂ��擾���܂��B
        GridEntity = GetEntityQuery(GirdEntityDesc);
    }

    protected override void OnUpdate()
    {
        //���������V���O���g������������Ă��邩�ǂ����`�F�b�N
        if (HasSingleton<GameState>() == false)
        {
            return;
        }

        var G_State = GetSingleton<GameState>();

        if (G_State.IsActive == false)
        {
            return;
        }


        if (G_State.GameEnd == true)
        {
            return;
        }

        bool TapConfirm = false;

        //�v���C���[���̓��͎�t
        Entities.With(GridEntity).ForEach((Entity EntityData, ref PointerInteraction GridClickData, ref GridComp GridData) =>
        {
            if (TapConfirm == false && GridClickData.clicked == true)
            {
                if (GridData.GridState != Pieces.NONE && GridData.Owner==G_State.NowTurn)
                {
                    //�^�b�v�t���O�𗧂Ă܂�
                    GridData.TapFlag = true;
                    TapConfirm = true;
                }
            }
        });

    }
}
