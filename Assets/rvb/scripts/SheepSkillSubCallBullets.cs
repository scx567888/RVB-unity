using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepSkillSubCallBullets :Skill{
        public int id;
        public string name = string.Empty;
        public int isAnim;
        public int endFrame;
        public int bullet;
        public int frameStep;
        public int frameCnt;
        public int[] callFrames = Array.Empty<int>();
        public int[] callCnts = Array.Empty<int>();
        public int type;
        public int[] startOffsetPos = Array.Empty<int>();
        public int startRadius;
        public int endRadius;
        public int len;



        public void tick(SheepMgr sheepMgr,PetView e) {
            var fff = sheepMgr.findTar(e);
            var l = fff.atkTar;
            var n = fff.moveTar;
            var r = fff.moveBoss;

            if (l != null || r != null) {
                e.state = SheepRoleState.CallBullets;
                e.subState = SheepRoleSubState.CallBullets;
                if (this.isAnim != 0) {
                    e.animType = SheepRoleAnimType.CallBullets;
                }
                else {
                    e.animType = SheepRoleAnimType.Idle;
                }

                e.readySkillId = this.id;
                return;
            }

            sheepMgr.moveTar(e, null);
        }
        
        

        public static IReadOnlyList<SheepSkillSubCallBullets> List => SheepSkillSubCallBulletsConfigs.All;

        public static SheepSkillSubCallBullets getById(int id) {
            return SheepSkillSubCallBulletsConfigs.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubCallBullets config) {
            return SheepSkillSubCallBulletsConfigs.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubCallBulletsConfigs {
        // 召唤冰雹
        public static readonly SheepSkillSubCallBullets config_80001 = new() {
            id = 80001,
            name = "召唤冰雹",
            isAnim = 0,
            endFrame = 300,
            bullet = 101,
            frameStep = 1,
            frameCnt = 10,
            callFrames = Array.Empty<int>(),
            callCnts = Array.Empty<int>(),
            type = 1,
            startOffsetPos = new[] { 0, 0, 1000 },
            startRadius = 0,
            endRadius = 500,
            len = 500
        };

        // 召唤剑雨
        public static readonly SheepSkillSubCallBullets config_80002 = new() {
            id = 80002,
            name = "召唤剑雨",
            isAnim = 0,
            endFrame = 300,
            bullet = 102,
            frameStep = 1,
            frameCnt = 10,
            callFrames = Array.Empty<int>(),
            callCnts = Array.Empty<int>(),
            type = 2,
            startOffsetPos = new[] { 0, 0, 1000 },
            startRadius = 500,
            endRadius = 500,
            len = 500
        };

        // 召唤鬼火
        public static readonly SheepSkillSubCallBullets config_80003 = new() {
            id = 80003,
            name = "召唤鬼火",
            isAnim = 0,
            endFrame = 3,
            bullet = 103,
            frameStep = 0,
            frameCnt = 0,
            callFrames = new[] { 1 },
            callCnts = new[] { 100 },
            type = 3,
            startOffsetPos = new[] { 0, 0, 200 },
            startRadius = 0,
            endRadius = 0,
            len = 500
        };

        // 召唤导弹
        public static readonly SheepSkillSubCallBullets config_80004 = new() {
            id = 80004,
            name = "召唤导弹",
            isAnim = 0,
            endFrame = 300,
            bullet = 104,
            frameStep = 1,
            frameCnt = 10,
            callFrames = Array.Empty<int>(),
            callCnts = Array.Empty<int>(),
            type = 4,
            startOffsetPos = new[] { 0, 0, 1000 },
            startRadius = 0,
            endRadius = 0,
            len = 500
        };

        // 音波
        public static readonly SheepSkillSubCallBullets config_80005 = new() {
            id = 80005,
            name = "音波",
            isAnim = 1,
            endFrame = 30,
            bullet = 10,
            frameStep = 0,
            frameCnt = 0,
            callFrames = new[] { 10 },
            callCnts = new[] { 1 },
            type = 0,
            startOffsetPos = Array.Empty<int>(),
            startRadius = 0,
            endRadius = 0,
            len = 0
        };

        public static readonly SheepSkillSubCallBullets[] All = {
            config_80001,
            config_80002,
            config_80003,
            config_80004,
            config_80005,
        };

        private static readonly Dictionary<int, SheepSkillSubCallBullets> Map = BuildMap();

        public static SheepSkillSubCallBullets GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubCallBullets config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubCallBullets 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubCallBullets config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubCallBullets> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubCallBullets>(All.Length);

            foreach (SheepSkillSubCallBullets config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubCallBullets 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}