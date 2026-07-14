using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepBulletAnimInfo {
        public int id;
        public int itemMeshCnt;
        public string name = string.Empty;
        public int rotType;

        public static IReadOnlyList<SheepBulletAnimInfo> List => SheepBulletAnimInfos.All;

        public static SheepBulletAnimInfo getById(int id) {
            return SheepBulletAnimInfos.GetById(id);
        }

        public static bool TryGetById(int id, out SheepBulletAnimInfo info) {
            return SheepBulletAnimInfos.TryGetById(id, out info);
        }
    }

    public static class SheepBulletAnimInfos {
        // 弓箭
        public static readonly SheepBulletAnimInfo anim_1 = new() {
            id = 1,
            itemMeshCnt = 6000,
            name = "弓箭",
            rotType = 3
        };

        // 技能弓箭
        public static readonly SheepBulletAnimInfo anim_2 = new() {
            id = 2,
            itemMeshCnt = 1000,
            name = "技能弓箭",
            rotType = 5
        };

        // 投石
        public static readonly SheepBulletAnimInfo anim_3 = new() {
            id = 3,
            itemMeshCnt = 1000,
            name = "投石",
            rotType = 3
        };

        // 铜钱
        public static readonly SheepBulletAnimInfo anim_4 = new() {
            id = 4,
            itemMeshCnt = 1000,
            name = "铜钱",
            rotType = 3
        };

        // 小投石
        public static readonly SheepBulletAnimInfo anim_5 = new() {
            id = 5,
            itemMeshCnt = 1000,
            name = "小投石",
            rotType = 3
        };

        // 飞棍
        public static readonly SheepBulletAnimInfo anim_6 = new() {
            id = 6,
            itemMeshCnt = 1000,
            name = "飞棍",
            rotType = 3
        };

        // 金箍棒
        public static readonly SheepBulletAnimInfo anim_7 = new() {
            id = 7,
            itemMeshCnt = 1000,
            name = "金箍棒",
            rotType = 0
        };

        // 投石爆炸
        public static readonly SheepBulletAnimInfo anim_8 = new() {
            id = 8,
            itemMeshCnt = 1000,
            name = "投石爆炸",
            rotType = 0
        };

        // 手掌
        public static readonly SheepBulletAnimInfo anim_9 = new() {
            id = 9,
            itemMeshCnt = 1000,
            name = "手掌",
            rotType = 0
        };

        // 弓箭爆炸
        public static readonly SheepBulletAnimInfo anim_10 = new() {
            id = 10,
            itemMeshCnt = 1000,
            name = "弓箭爆炸",
            rotType = 0
        };

        // 小兵自爆
        public static readonly SheepBulletAnimInfo anim_11 = new() {
            id = 11,
            itemMeshCnt = 1000,
            name = "小兵自爆",
            rotType = 0
        };

        // 小金箍棒
        public static readonly SheepBulletAnimInfo anim_12 = new() {
            id = 12,
            itemMeshCnt = 1000,
            name = "小金箍棒",
            rotType = 0
        };

        // 小手掌
        public static readonly SheepBulletAnimInfo anim_13 = new() {
            id = 13,
            itemMeshCnt = 1000,
            name = "小手掌",
            rotType = 0
        };

        // 火球
        public static readonly SheepBulletAnimInfo anim_14 = new() {
            id = 14,
            itemMeshCnt = 1000,
            name = "火球",
            rotType = 3
        };

        // 火球二
        public static readonly SheepBulletAnimInfo anim_15 = new() {
            id = 15,
            itemMeshCnt = 1000,
            name = "火球二",
            rotType = 3
        };

        // 火球爆炸
        public static readonly SheepBulletAnimInfo anim_16 = new() {
            id = 16,
            itemMeshCnt = 1000,
            name = "火球爆炸",
            rotType = 0
        };

        // 火球爆炸二
        public static readonly SheepBulletAnimInfo anim_17 = new() {
            id = 17,
            itemMeshCnt = 1000,
            name = "火球爆炸二",
            rotType = 0
        };

        // 投石左
        public static readonly SheepBulletAnimInfo anim_18 = new() {
            id = 18,
            itemMeshCnt = 1000,
            name = "投石左",
            rotType = 3
        };

        // 投石爆炸左
        public static readonly SheepBulletAnimInfo anim_19 = new() {
            id = 19,
            itemMeshCnt = 1000,
            name = "投石爆炸左",
            rotType = 0
        };

        public static readonly SheepBulletAnimInfo[] All = {
            anim_1,
            anim_2,
            anim_3,
            anim_4,
            anim_5,
            anim_6,
            anim_7,
            anim_8,
            anim_9,
            anim_10,
            anim_11,
            anim_12,
            anim_13,
            anim_14,
            anim_15,
            anim_16,
            anim_17,
            anim_18,
            anim_19,
        };

        private static readonly Dictionary<int, SheepBulletAnimInfo> Map = BuildMap();

        public static SheepBulletAnimInfo GetById(int id) {
            if (!Map.TryGetValue(id, out SheepBulletAnimInfo info)) {
                throw new KeyNotFoundException($"不存在 SheepBulletAnimInfo 配置，ID: {id}");
            }

            return info;
        }

        public static bool TryGetById(int id, out SheepBulletAnimInfo info) {
            return Map.TryGetValue(id, out info);
        }

        private static Dictionary<int, SheepBulletAnimInfo> BuildMap() {
            var map = new Dictionary<int, SheepBulletAnimInfo>(All.Length);

            foreach (SheepBulletAnimInfo info in All) {
                if (!map.TryAdd(info.id, info)) {
                    throw new InvalidOperationException($"SheepBulletAnimInfo 存在重复 ID: {info.id}");
                }
            }

            return map;
        }
    }
}