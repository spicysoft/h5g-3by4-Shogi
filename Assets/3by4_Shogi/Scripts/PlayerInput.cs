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
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

        GirdEntityDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(RectTransform), typeof(Sprite2DRenderer), typeof(PointerInteraction), typeof(Button), typeof(GridComp) },
        };

        /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
        //作成したクエリの結果を取得します。
        GridEntity = GetEntityQuery(GirdEntityDesc);
    }

    protected override void OnUpdate()
    {
        //そもそもシングルトンが生成されているかどうかチェック
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

        //プレイヤー側の入力受付
        Entities.With(GridEntity).ForEach((Entity EntityData, ref PointerInteraction GridClickData, ref GridComp GridData) =>
        {
            if (TapConfirm == false && GridClickData.clicked == true)
            {
                if (GridData.GridState != Pieces.NONE && GridData.Owner==G_State.NowTurn)
                {
                    //タップフラグを立てます
                    GridData.TapFlag = true;
                    TapConfirm = true;
                }
            }
        });

    }
}
