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
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        GirdEntityDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(RectTransform), typeof(Sprite2DRenderer),typeof(PointerInteraction) ,typeof(Button),typeof(GridComp)},
        };

        /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
        //作成したクエリの結果を取得します。
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

    //Entityを各座標に対応させた順番に格納しなおします
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

    //指定座標のEntityを取得します
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

    //指定したグリッドのデータを取得します
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

    //送られてきた座標にデータを書き換えます。
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



    //盤面のデータを読み取り、色を再描画します。
    public bool RefreshBoardColor()
    {
        //Board自体の色
        Color TableColor_1 = new Color(0.4366531f, 0.853f, 0.423941f);


        var G_State = GetSingleton<GameState>();

        Entities.With(GridEntity).ForEach((Entity EntityData, ref Sprite2DRenderer Sprite2D, ref GridComp GridData) =>
        {

            //盤面に駒が何もなければそのグリッドに適したBoardの色を設定
            if (GridData.GridState==0)
            {
                int ColBaseNum = GridData.GridNum.y % 2 == 0 ? 0 : 1;

                Sprite2D.color = TableColor_1 ;
                return;
            }

            //そうでない場合、駒の色を描画

            Sprite2D.color = TableColor_1;
        });

        return true;
    }
}
