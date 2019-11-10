using Unity.Entities;
using Unity.Collections;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.UILayout;
using Unity.Tiny.UIControls;
using Unity.Mathematics;
using Shogi;

public class BoardMan : ComponentSystem
{
    const int BoardSize_X = 3;

    const int BoardSize_Y = 4;

    EntityQueryDesc GirdEntityDesc;
    EntityQuery GridEntity;

    EntityQueryDesc CanvasDesc;
    EntityQuery CanvasEntity;

    protected override void OnCreate()
    {
        /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

        GirdEntityDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(RectTransform), typeof(Sprite2DRenderer),typeof(PointerInteraction) ,typeof(Button),typeof(GridComp)},
        };

        /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
        //�쐬�����N�G���̌��ʂ��擾���܂��B
        GridEntity = GetEntityQuery(GirdEntityDesc);
    }

    protected override void OnUpdate()
    {
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

        if (!(GridEntity.CalculateLength() > 0))
        {
            return;
        }

        NativeArray<Entity> GridDatas = new NativeArray<Entity>(0, Allocator.Temp);

        GetGirdArray(ref GridDatas);


        Entities.With(GridEntity).ForEach((Entity EntityData, ref GridComp GridData) =>
        {
            if (GridData.TapFlag == true)
            {

                SetSingleton<GameState>(G_State);
                RefreshBoardColor();
            }
        });

        GridDatas.Dispose();
    }

    //Entity���e���W�ɑΉ����������ԂɊi�[���Ȃ����܂�
    public bool  GetGirdArray(ref NativeArray<Entity> ReturnEntities)
    {
        NativeArray<Entity> EntitiesArray = new NativeArray<Entity>(BoardSize_X * BoardSize_Y, Allocator.Temp);

        for (int i = 0; i < BoardSize_X * BoardSize_Y; ++i)
        {
            EntitiesArray[i] = Entity.Null;
        }


        Entities.With(GridEntity).ForEach((Entity EntityData, ref GridComp GridData) =>
        {
            if (GridData.GridNum.x < BoardSize_X && GridData.GridNum.y < BoardSize_Y)
            {
                EntitiesArray[GridData.GridNum.x + (GridData.GridNum.y * BoardSize_X)] = EntityData;
            }
        });

        ReturnEntities = EntitiesArray;

        EntitiesArray.Dispose();
        return true;
    }

    //�w����W��Entity���擾���܂�
    public Entity GetGridEntity(int2 CheckPos, ref NativeArray<Entity> Entities)
    {
        if (CheckPos.x < 0 && CheckPos.y < 0)
        {
            return Entity.Null;
        }

        if (CheckPos.x < BoardSize_X && CheckPos.y < BoardSize_Y)
        {
            return Entities[CheckPos.x + (CheckPos.y * BoardSize_X)];
        }

        return Entity.Null;
    }

    //�w�肵���O���b�h�̃f�[�^���擾���܂�
    public Pieces GetGridData(int2 CheckPos, ref NativeArray<Entity> Entities)
    {
        if (CheckPos.x < 0 || CheckPos.y < 0)
        {
            return Pieces.NONE;
        }

        if (CheckPos.x < BoardSize_X && CheckPos.y < BoardSize_Y)
        {
            GridComp GridData = EntityManager.GetComponentData<GridComp>(Entities[CheckPos.x + (CheckPos.y * BoardSize_X)]);
            return GridData.GridState;
        }

        return Pieces.NONE;
    }

    //�����Ă������W�Ƀf�[�^�����������܂��B
    public void SetGridData(int2 SetPos,Pieces SetPiece,ref NativeArray<Entity> Entities)
    {
        if (SetPos.x < 0 || SetPos.y < 0)
        {
            return;
        }

        if (SetPos.x < BoardSize_X && SetPos.y < BoardSize_Y)
        {
            GridComp GridData = EntityManager.GetComponentData<GridComp>(Entities[SetPos.x + (SetPos.y * BoardSize_X)]);
            GridData.GridState = SetPiece;
            EntityManager.SetComponentData(Entities[SetPos.x + (SetPos.y * BoardSize_X)], GridData);
        }
    }



    //�Ֆʂ̃f�[�^��ǂݎ��A�F���ĕ`�悵�܂��B
    public bool RefreshBoardColor()
    {
        //Board���̂̐F
        Color TableColor_1 = new Color(0.4366531f, 0.853f, 0.423941f);


        var G_State = GetSingleton<GameState>();

        Entities.With(GridEntity).ForEach((Entity EntityData, ref Sprite2DRenderer Sprite2D, ref GridComp GridData) =>
        {

            //�Ֆʂɋ�����Ȃ���΂��̃O���b�h�ɓK����Board�̐F��ݒ�
            if (GridData.GridState==0)
            {
                int ColBaseNum = GridData.GridNum.y % 2 == 0 ? 0 : 1;

                Sprite2D.color = TableColor_1 ;
                return;
            }

            //�����łȂ��ꍇ�A��̐F��`��

            Sprite2D.color = TableColor_1;
        });

        return true;
    }
}
