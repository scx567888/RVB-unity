using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public sealed class SheepSkillSubInvincible {
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
        public int hitBackDistance;

        public static IReadOnlyList<SheepSkillSubInvincible> List => SheepSkillSubInvincibles.All;

        public static SheepSkillSubInvincible getById(int id) {
            return SheepSkillSubInvincibles.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubInvincible config) {
            return SheepSkillSubInvincibles.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubInvincibles {
        // 大河马大招格挡
        public static readonly SheepSkillSubInvincible config_50001 = new() {
            id = 50001,
            name = "大河马大招格挡",
            endFrame = 130,
            healHealthPercent = 2,
            healFrames = new[] { 30, 40, 50, 60, 70 },
            parryPercent = 90,
            atkFrames = new[] { 25 },
            spiltRadiusBet = 1.5f,
            atkFindR = 20,
            atkBet = 1f,
            hitBackDistance = 100
        };

        public static readonly SheepSkillSubInvincible[] All = {
            config_50001,
        };

        private static readonly Dictionary<int, SheepSkillSubInvincible> Map = BuildMap();

        public static SheepSkillSubInvincible GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubInvincible config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubInvincible 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubInvincible config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubInvincible> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubInvincible>(All.Length);

            foreach (SheepSkillSubInvincible config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubInvincible 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}