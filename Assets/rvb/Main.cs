using System;
using System.Collections.Generic;
using rvb.scripts;
using rvb.utils;
using scx.SpriteRenderer;
using UnityEngine;

namespace rvb {
    // 小兵渲染配置
    [Serializable]
    public class PetRenderConfig {
        public Texture2D texture;
        public TextAsset json;
        public SheepCamp camp;
        public int animId;
    }

    // 字段渲染配置
    [Serializable]
    public class BulletRenderConfig {
        public Texture2D texture;
        public TextAsset json;
        public SheepCamp camp;
        public SheepRoleType roleType;
    }

    public class Main : MonoBehaviour {
        // 小兵渲染数据
        public List<PetRenderConfig> petRenderConfigs;

        // 子弹渲染数据
        public List<BulletRenderConfig> bulletRenderConfigs;

        // 主材质
        public Material mainMaterial;

        // 动画帧数解析器
        private SheepAnimFrameCountResolver animFrameCountResolver = new();

        [Header("Logic")] 
        [SerializeField] private float logicFPS = 30f;
        [SerializeField] private int maxLogicStepsPerUnityFrame = 4;
        [SerializeField] private float logicToWorldScale = 0.01f;
        [SerializeField] private float logicHeightToWorldScale = 0.01f;
        [SerializeField] private int fallbackLogicalAnimationFrames = 30;

        // 按照 [阵营][角色类型] 存储
        private Dictionary<int, ScxSpriteRenderer>[] petSpriteRenderers;

        // 按照 [子弹类型] 存储
        private ScxSpriteRenderer[] bulletSpriteRenderers;

        private string[] spriteNames;
        private SheepMgr sheepMgr;
        private SheepCtl sheepCtl = new SheepCtl();

        private float logicAccumulator;
        private LoadRoleResult loadRoleResult;

        private void Start() {
            this.petSpriteRenderers = new[] {
                new Dictionary<int, ScxSpriteRenderer>(),
                new Dictionary<int, ScxSpriteRenderer>()
            };

            this.bulletSpriteRenderers = new ScxSpriteRenderer[0];


            foreach (var petRenderConfig in petRenderConfigs) {
                var loadRoleResult =
                    SheepSpriteAtlasLoader.loadRole(petRenderConfig.texture, petRenderConfig.json.text);
                var scxSpriteRenderer = new ScxSpriteRenderer(
                    loadRoleResult.spriteAtlas,
                    100,
                    mainMaterial,
                    2000
                );

                foreach (var keyValuePair in loadRoleResult.animFrame) {
                    var k = keyValuePair.Key;
                    var v = keyValuePair.Value;
                    animFrameCountResolver.setAnimationFrameCount(petRenderConfig.camp, petRenderConfig.animId,
                        (SheepRoleAnimType)k, v);
                }

                scxSpriteRenderer.setParent(gameObject);
                this.petSpriteRenderers[(int)petRenderConfig.camp][(int)petRenderConfig.animId] = scxSpriteRenderer;
            }


            sheepMgr = new SheepMgr(SheepConfigs.sheepConfig, animFrameCountResolver, sheepCtl);

            logicAccumulator = 0f;

            sheepMgr.gameIndex++;
            sheepMgr.setState(SheepRoomState.Start);
            sheepMgr.onGameStart();
            sheepMgr.game_clear();
        }


        public void StartBattle() {
            // 这里先跳过倒计时，直接进入战斗状态。
            sheepMgr.setState(SheepRoomState.Run);
            sheepMgr.onGameRun();
        }

        private void Update() {
            UpdateHotkeys();

            if (sheepMgr.state == SheepRoomState.Start || sheepMgr.state == SheepRoomState.Run) {
                RunLogicFrames();
            }

            // Unity 每个显示帧提交一次渲染。
            foreach (var p1 in petSpriteRenderers) {
                foreach (var scxSpriteRenderer in p1) {
                    scxSpriteRenderer.Value.update();
                }
            }
        }

        private void RunLogicFrames() {
            float safeFps = Mathf.Max(1f, logicFPS);
            float logicStepSeconds = 1f / safeFps;
            float logicStepMilliseconds = logicStepSeconds * 1000f;

            logicAccumulator += Time.deltaTime;

            var list = new List<(HashSet<PetView> del_pets, HashSet<BulletView> del_bullets)>();

            int stepCount = 0;
            while (logicAccumulator >= logicStepSeconds && stepCount < maxLogicStepsPerUnityFrame) {
                var gameUpdate = sheepMgr.game_update(sheepCtl, logicStepMilliseconds);
                list.Add(gameUpdate);
                SyncBossMarker(0);
                SyncBossMarker(1);

                logicAccumulator -= logicStepSeconds;
                stepCount++;
            }

            // 这一显示帧没有产生逻辑步时，不清理，否则画面会闪烁。
            if (stepCount == 0) {
                return;
            }

            foreach (var valueTuple in list) {
                foreach (var valueTupleDelPet in valueTuple.del_pets) {
                    valueTupleDelPet.renderUnit?.destroy();
                }

                foreach (var valueTupleDelBullet in valueTuple.del_bullets) {
                    // valueTupleDelBullet.renderUnit?.destroy();
                }
            }

            foreach (var sheepMgrPet in sheepMgr.pets) {
                SyncRoleView(sheepMgrPet);
            }

            RecycleMissingRoleRenderers();
            RecycleMissingBulletRenderers();
        }

        private void SyncBossMarker(int poolIndex) {
            PetView bossView = sheepMgr.boss[poolIndex];
            if (bossView == null || !bossView.isActive) {
                return;
            }

            // SyncRoleView(bossView);
        }

        private void SyncRoleView(PetView view) {
            var renderUnit = view.renderUnit;
            if (renderUnit == null) {
                renderUnit = petSpriteRenderers[(int)view.camp][(int)view.conf.animId].createUnit();
                view.renderUnit = renderUnit;
                renderUnit.setScale(view.conf.scale, view.conf.scale, 1f);
                renderUnit.setRotationFromEuler(45, 0, 0);

                var initialFrame = ResolveRoleSpriteFrame(view);
                renderUnit.setFrame(initialFrame);
                renderUnit.setVisible(true);
            }

            float worldX = view.animX * logicToWorldScale;
            float worldY = view.animZ * logicHeightToWorldScale;
            float worldZ = view.animY * logicToWorldScale;

            renderUnit.setVisible(true);
            renderUnit.setPosition(worldX, worldY, worldZ);

            var frameIndex = ResolveRoleSpriteFrame(view);
            renderUnit.setFrame(frameIndex);
        }

        // 直接替换 Main/RVB 中原来的 HandleBulletRender。
// 前提：
// 1. loadBullet 生成的帧名是 "animId-frame"，例如 "3-12"。
// 2. ScxSpriteRenderer 构造时 pixelsPerUnit = 100。
// 3. logicToWorldScale 与 logicHeightToWorldScale 最好相同；相同时与原版完全等价。
        private void HandleBulletRender(BulletView bullet) {
            // if (
            //     bullet == null ||
            //     bullet.id == 0 ||
            //     bullet.isDie ||
            //     bullet.conf == null) {
            //     return;
            // }
            //
            // int id = bullet.id;
            // seenBulletIds.Add(id);
            //
            // if (!bulletRenderers.TryGetValue(id, out ScxSpriteRenderUnit renderPet)) {
            //     renderPet = scxSpriteRenderer2.createUnit();
            //     renderPet.setVisible(true);
            //     bulletRenderers.Add(id, renderPet);
            // }
            //
            // int animId = bullet.conf.animId;
            //
            // // 原 bullets_framess 中各动画的真实帧数。
            // // 未列出的动画退回到 endFrame + 1。
            // int frameCount;
            // switch (animId) {
            //     case 1:
            //         frameCount = 1;
            //         break;
            //     case 2:
            //         frameCount = 9;
            //         break;
            //     case 3:
            //     case 18:
            //         frameCount = 38;
            //         break;
            //     case 7:
            //     case 9:
            //     case 12:
            //     case 13:
            //         frameCount = 61;
            //         break;
            //     case 8:
            //     case 19:
            //         frameCount = 24;
            //         break;
            //     case 10:
            //         frameCount = 8;
            //         break;
            //     case 11:
            //         frameCount = 16;
            //         break;
            //     default:
            //         frameCount = Mathf.Max(1, bullet.conf.endFrame + 1);
            //         break;
            // }
            //
            // int localFrame = PositiveModulo(bullet.frame, frameCount);
            //
            // // loadBullet 的标准命名规则。
            // renderPet.setFrame(localFrame);
            //
            // int rotType = 0;
            // if (SheepBulletAnimInfo.TryGetById(animId, out SheepBulletAnimInfo animInfo)) {
            //     rotType = animInfo.rotType;
            // }
            //
            // const float spritePixelsPerUnit = 100f;
            // const float sin45 = 0.7071067811865475f;
            // const float epsilon = 0.000001f;
            //
            // // Cocos 逻辑坐标 (x, y, z) -> Unity 世界坐标 (x, z, y)。
            // Vector3 ToWorldPoint(float x, float y, float z) {
            //     return new Vector3(
            //         x * logicToWorldScale,
            //         z * logicHeightToWorldScale,
            //         y * logicToWorldScale
            //     );
            // }
            //
            // Vector3 ToWorldVector(Vector3 value) {
            //     return new Vector3(
            //         value.x * logicToWorldScale,
            //         value.z * logicHeightToWorldScale,
            //         value.y * logicToWorldScale
            //     );
            // }
            //
            // // rotType 0 的 Sprite 原点在原版中相当于“底边中心”，
            // // 标准 Sprite 通常以图片中心为 Transform 原点，因此需要抬高半个图片高度。
            // float GetFixedFrameHeight(int currentAnimId, int currentFrame) {
            //     switch (currentAnimId) {
            //         case 7:
            //         case 9:
            //         case 12:
            //         case 13:
            //             return 452f;
            //
            //         case 8:
            //         case 19:
            //             // 原图集中这些帧高度在 42/43 像素之间变化。
            //             if ((currentFrame >= 4 && currentFrame <= 7) ||
            //                 (currentFrame >= 16 && currentFrame <= 19)) {
            //                 return 42f;
            //             }
            //
            //             return 43f;
            //
            //         case 10:
            //         case 11:
            //         case 16:
            //         case 17:
            //             return 128f;
            //
            //         default:
            //             // 未知固定特效的保守值。
            //             return 128f;
            //     }
            // }
            //
            // Vector3 centerOriginal;
            // Vector3 rightOriginal;
            // Vector3 upOriginal;
            //
            // if (rotType == 3) {
            //     // 原版：right = normalize(dirX, dirY, dirZ)
            //     //       up    = normalize(cross((0,-1,0), right))
            //     rightOriginal = new Vector3(
            //         bullet.dirX,
            //         bullet.dirY,
            //         bullet.dirZ
            //     );
            //
            //     if (rightOriginal.sqrMagnitude <= epsilon) {
            //         rightOriginal = bullet.camp == SheepCamp.Red
            //             ? Vector3.right
            //             : Vector3.left;
            //     }
            //
            //     rightOriginal.Normalize();
            //
            //     upOriginal = Vector3.Cross(Vector3.down, rightOriginal);
            //     if (upOriginal.sqrMagnitude <= epsilon) {
            //         upOriginal = Vector3.forward;
            //     }
            //
            //     upOriginal.Normalize();
            //
            //     centerOriginal = new Vector3(
            //         bullet.x,
            //         bullet.y,
            //         bullet.z
            //     );
            // }
            // else if (rotType == 5) {
            //     // 原版：right = normalize(dirX, dirY, 0)
            //     //       up    = normalize(cross((0,0,1), right))
            //     rightOriginal = new Vector3(
            //         bullet.dirX,
            //         bullet.dirY,
            //         0f
            //     );
            //
            //     if (rightOriginal.sqrMagnitude <= epsilon) {
            //         rightOriginal = bullet.camp == SheepCamp.Red
            //             ? Vector3.right
            //             : Vector3.left;
            //     }
            //
            //     rightOriginal.Normalize();
            //
            //     upOriginal = Vector3.Cross(Vector3.forward, rightOriginal);
            //     if (upOriginal.sqrMagnitude <= epsilon) {
            //         upOriginal = Vector3.up;
            //     }
            //
            //     upOriginal.Normalize();
            //
            //     centerOriginal = new Vector3(
            //         bullet.x,
            //         bullet.y,
            //         bullet.z
            //     );
            // }
            // else {
            //     // rotType == 0：爆炸、手掌、金箍棒等固定特效。
            //     rightOriginal = Vector3.right;
            //     upOriginal = new Vector3(0f, sin45, sin45);
            //
            //     centerOriginal = new Vector3(
            //         bullet.x,
            //         bullet.y + bullet.startY,
            //         bullet.z
            //     );
            // }
            //
            // float logicalScale = bullet.conf.scale;
            //
            // // 每个 Sprite 像素在世界中的局部 X/Y 方向。
            // Vector3 rightPerPixelWorld =
            //     ToWorldVector(rightOriginal * logicalScale);
            //
            // Vector3 upPerPixelWorld =
            //     ToWorldVector(upOriginal * logicalScale);
            //
            // Vector3 worldCenter = ToWorldPoint(
            //     centerOriginal.x,
            //     centerOriginal.y,
            //     centerOriginal.z
            // );
            //
            // if (rotType == 0) {
            //     float frameHeight = GetFixedFrameHeight(animId, localFrame);
            //
            //     // 从底边中心换算到标准 Sprite 的图片中心。
            //     worldCenter += upPerPixelWorld * (frameHeight * 0.5f);
            // }
            //
            // float scaleX = rightPerPixelWorld.magnitude * spritePixelsPerUnit;
            //
            // if (scaleX <= epsilon) {
            //     renderPet.setVisible(false);
            //     return;
            // }
            //
            // Vector3 rightAxis = rightPerPixelWorld / rightPerPixelWorld.magnitude;
            //
            // // 当 logicToWorldScale != logicHeightToWorldScale 时可能产生轻微剪切。
            // // 标准 Transform 没有 shear，这里用 Gram-Schmidt 去掉剪切分量。
            // Vector3 upOrthogonal = upPerPixelWorld -
            //                        rightAxis * Vector3.Dot(upPerPixelWorld, rightAxis);
            //
            // float upLength = upOrthogonal.magnitude;
            // if (upLength <= epsilon) {
            //     renderPet.setVisible(false);
            //     return;
            // }
            //
            // Vector3 upAxis = upOrthogonal / upLength;
            // Vector3 forwardAxis = Vector3.Cross(rightAxis, upAxis).normalized;
            //
            // Quaternion rotation = Quaternion.LookRotation(forwardAxis, upAxis);
            // Vector3 euler = rotation.eulerAngles;
            //
            // float scaleY = upLength * spritePixelsPerUnit;
            //
            // renderPet.setVisible(true);
            // renderPet.setPosition(worldCenter.x, worldCenter.y, worldCenter.z);
            // renderPet.setRotationFromEuler(euler.x, euler.y, euler.z);
            // renderPet.setScale(scaleX, scaleY, 1f);
        }

        private string ResolveRoleSpriteFrame(PetView view) {
            var i = animFrameCountResolver.resolve(view.camp, view.skinId, view.animType);

            int localFrame = PositiveModulo(view.animFrame, i);
            return ((int)(view.animType)) + "-" + localFrame;
        }

        private int ResolveLogicalAnimationFrameCount(PetView view) {
            // 这是逻辑状态完成所使用的动画长度，不是 Scx 图集总帧数。
            return Mathf.Max(1, fallbackLogicalAnimationFrames);
        }

        private int ClampSpriteIndex(int index) {
            if (spriteNames == null || spriteNames.Length == 0) {
                return 0;
            }

            return PositiveModulo(index, spriteNames.Length);
        }

        private static int PositiveModulo(int value, int modulo) {
            if (modulo <= 0) {
                return 0;
            }

            int result = value % modulo;
            return result < 0 ? result + modulo : result;
        }

        private void RecycleMissingRoleRenderers() {
            // staleRoleSlots.Clear();

            // foreach (KeyValuePair<string, ScxSpriteRenderUnit> pair in roleRenderers) {
            // if (!seenRoleSlots.Contains(pair.Key)) {
            // staleRoleSlots.Add(pair.Key);
            // }
            // }

            // foreach (var slot in staleRoleSlots) {
            // var renderPet = roleRenderers[slot];
            // roleRenderers.Remove(slot);
            // renderPet.destroy();
            // }
        }

        private void RecycleMissingBulletRenderers() {
            // staleBulletIds.Clear();
            //
            // foreach (KeyValuePair<int, ScxSpriteRenderUnit> pair in bulletRenderers) {
            //     if (!seenBulletIds.Contains(pair.Key)) {
            //         staleBulletIds.Add(pair.Key);
            //     }
            // }
            //
            // foreach (int id in staleBulletIds) {
            //     var renderPet = bulletRenderers[id];
            //     bulletRenderers.Remove(id);
            //     renderPet.destroy();
            // }
        }

        private void UpdateHotkeys() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                StartBattle();
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                SpawnArmy(SheepCamp.Red, 22, 10);
            }

            if (Input.GetKeyDown(KeyCode.B)) {
                SpawnArmy(SheepCamp.Blue, 22, 10);
            }

            // if (Input.GetKeyDown(KeyCode.H) && highlightMaterial != null) {
            // scxSpriteRenderer.setMaterialTemplate(highlightMaterial);
            // }

            // if (Input.GetKeyDown(KeyCode.M) && mainMaterial != null) {
            // scxSpriteRenderer.setMaterialTemplate(mainMaterial);
            // }
        }

        private void SpawnArmy(SheepCamp camp, int roleId, int count) {
            if (sheepMgr == null || count <= 0) {
                return;
            }

            sheepMgr.produce_pets(roleId, count, camp);
        }
    }
}