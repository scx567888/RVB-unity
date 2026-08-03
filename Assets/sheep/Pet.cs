using System;
using scx.SpriteRenderer;
using UnityEngine;

namespace sheep {
    public class Pet {
        // 唯一 ID
        public int id;
        
        // 唯一真实位置 X
        public float x;

        // 唯一真实位置 Y
        public float y;


        // ****************** 核心逻辑帧 *********************

        // 核心逻辑帧
        public int frame;

        public PetMoveIntent moveIntent;

        // ********************* 碰撞移动相关 **************************

        // 碰撞半径
        public float collideR;

        // 同时碰撞到多少个单位以后, 停止自主移动
        public int collideNotMoveNum;

        // 发生碰撞时, 自主位移保留比例
        public float collideMoveScale;

        // 碰撞推开位移的比例
        public float collideElasticityScale;

        // ********************* 渲染器挂载相关 **********************

        // 渲染器句柄 (逻辑层不应使用此字段)
        public ScxSpriteRenderUnit scxSpriteRenderUnit;

        // 渲染器 X 用于插值 (逻辑层不应使用此字段)
        public float lastX;

        // 渲染器 Y 用于插值 (逻辑层不应使用此字段)
        public float lastY;

        public void action(SheepWorld sheepWorld) {
            // 1. 执行逻辑, 更新自主移动意图
            PetLogic.INSTANCE.tick(this, sheepWorld);

            // 2, 更新逻辑帧
            frame++;
        }

    }
}