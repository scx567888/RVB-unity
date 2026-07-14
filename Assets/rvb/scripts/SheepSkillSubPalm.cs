using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubPalm {
        public int id;
        public string name = string.Empty;
        public int endFrame;
        public int healHealthPercent;
        public int[] healFrames = Array.Empty<int>();
        public int parryPercent;
        public int[] atkFrames = Array.Empty<int>();
        public float spiltRadiusBet;
        public int atkFindR;
        public float atkBet;
        public int[] hitBackFrames = Array.Empty<int>();
        public int[] hitBackDistances = Array.Empty<int>();

        public static IReadOnlyList<SheepSkillSubPalm> List => SheepSkillSubPalms.All;

        public static SheepSkillSubPalm getById(int id) {
            return SheepSkillSubPalms.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubPalm config) {
            return SheepSkillSubPalms.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubPalms {
        // 羊狼神大招
        public static readonly SheepSkillSubPalm config_70001 = new() {
            id = 70001,
            name = "羊狼神大招",
            endFrame = 87,
            healHealthPercent = 0,
            healFrames = new[] { 31, 32, 33, 34, 35 },
            parryPercent = 90,
            atkFrames = new[] { 63 },
            spiltRadiusBet = 3f,
            atkFindR = 24,
            atkBet = 2.5f,
            hitBackFrames = new[] { 15, 63 },
            hitBackDistances = new[] { -1500, 1200 }
        };

        // 大虎王强力大招如来神掌
        public static readonly SheepSkillSubPalm config_70002 = new() {
            id = 70002,
            name = "大虎王强力大招如来神掌",
            endFrame = 87,
            healHealthPercent = 0,
            healFrames = new[] { 31, 32, 33, 34, 35 },
            parryPercent = 90,
            atkFrames = new[] { 63 },
            spiltRadiusBet = 3f,
            atkFindR = 60,
            atkBet = 1.3f,
            hitBackFrames = new[] { 15, 63 },
            hitBackDistances = new[] { -2500, 2200 }
        };

        public static readonly SheepSkillSubPalm[] All = {
            config_70001,
            config_70002,
        };

        private static readonly Dictionary<int, SheepSkillSubPalm> Map = BuildMap();

        public static SheepSkillSubPalm GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubPalm config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubPalm 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubPalm config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubPalm> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubPalm>(All.Length);

            foreach (SheepSkillSubPalm config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubPalm 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}