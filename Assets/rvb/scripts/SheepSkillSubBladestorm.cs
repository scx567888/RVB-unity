using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepSkillSubBladestorm {
        public int id;
        public string name = string.Empty;
        public int endFrame;
        public float spiltRadiusBet;
        public int atkFindR;
        public float atkBet;
        public int findMoveFrame;
        public int[] atkFrames = Array.Empty<int>();
        public int parryPercent;
        public int speed;
        public int findR;

        public static IReadOnlyList<SheepSkillSubBladestorm> List => SheepSkillSubBladestorms.All;

        public static SheepSkillSubBladestorm getById(int id) {
            return SheepSkillSubBladestorms.GetById(id);
        }

        public static bool TryGetById(int id, out SheepSkillSubBladestorm config) {
            return SheepSkillSubBladestorms.TryGetById(id, out config);
        }
    }

    public static class SheepSkillSubBladestorms {
        public static readonly SheepSkillSubBladestorm skill_40001 = new() {
            id = 40001,
            name = "大刺客剑刃风暴",
            endFrame = 109,
            spiltRadiusBet = 1f,
            atkFindR = 10,
            atkBet = 1f,
            findMoveFrame = 1,
            parryPercent = 50,
            speed = 200,
            findR = 1,
            atkFrames = new[] { 5, 10, 17, 24, 30, 36, 42, 48, 54, 87, 88 }
        };

        public static readonly SheepSkillSubBladestorm[] All = {
            skill_40001
        };

        private static readonly Dictionary<int, SheepSkillSubBladestorm> Map = BuildMap();

        public static SheepSkillSubBladestorm GetById(int id) {
            if (!Map.TryGetValue(id, out SheepSkillSubBladestorm config)) {
                throw new KeyNotFoundException($"不存在 SheepSkillSubBladestorm 配置，ID: {id}");
            }

            return config;
        }

        public static bool TryGetById(int id, out SheepSkillSubBladestorm config) {
            return Map.TryGetValue(id, out config);
        }

        private static Dictionary<int, SheepSkillSubBladestorm> BuildMap() {
            var map = new Dictionary<int, SheepSkillSubBladestorm>(All.Length);

            foreach (SheepSkillSubBladestorm config in All) {
                if (!map.TryAdd(config.id, config)) {
                    throw new InvalidOperationException($"SheepSkillSubBladestorm 存在重复 ID: {config.id}");
                }
            }

            return map;
        }
    }
}