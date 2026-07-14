using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepRoleFormation {
        public int id;
        public string name;
        public SheepRoleFormationType formationType;
        public int preStartX;
        public int preItemX;
        public int preItemY;
        public int preItemNumY;
        public int startR;
        public int minAngle;
        public int startAngle;
        public int maxAngle;
        public int angleDensity;
        public int startStepR;
        public int startStepAngle;
        public int minScope;
        public int startScope;
        public int maxScope;
        public int density;
        public int startTimeStep;
        public int frameMaxCount;
        public int startX;
        public int itemY;
        public int frameItemX;
        public int itemNumY;
        public int itemYGapNum;
        public int itemYGap;
        public int itemNumX;
        public int itemYGapFrame;
        public int baseTimes;

        public static IReadOnlyList<SheepRoleFormation> List => SheepRoleFormations.All;

        public static SheepRoleFormation getById(int id) {
            return SheepRoleFormations.GetById(id);
        }

        public static bool TryGetById(int id, out SheepRoleFormation formation) {
            return SheepRoleFormations.TryGetById(id, out formation);
        }
    }

    public static class SheepRoleFormations {
        // 阵型 0
        public static readonly SheepRoleFormation formation_0 = new() {
            id = 0,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 300,
            preItemX = 100,
            preItemY = 150,
            preItemNumY = 50,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 800,
            itemY = 100,
            frameItemX = 8,
            itemNumY = 1,
            itemYGapNum = 0,
            itemYGap = 0,
            itemNumX = 0,
            itemYGapFrame = 12,
            baseTimes = 1
        };

        // 阵型 1
        public static readonly SheepRoleFormation formation_1 = new() {
            id = 1,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 500,
            preItemX = 300,
            preItemY = 225,
            preItemNumY = 50,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 350,
            frameItemX = 4,
            itemNumY = 36,
            itemYGapNum = 12,
            itemYGap = 120,
            itemNumX = 6,
            itemYGapFrame = 3,
            baseTimes = 1
        };

        // 阵型 2
        public static readonly SheepRoleFormation formation_2 = new() {
            id = 2,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 400,
            preItemX = 300,
            preItemY = 300,
            preItemNumY = 35,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 800,
            frameItemX = 6,
            itemNumY = 15,
            itemYGapNum = 15,
            itemYGap = 150,
            itemNumX = 6,
            itemYGapFrame = 9,
            baseTimes = 1
        };

        // 阵型 3
        public static readonly SheepRoleFormation formation_3 = new() {
            id = 3,
            name = "",
            formationType = SheepRoleFormationType.AngleTidy,
            preStartX = 600,
            preItemX = 300,
            preItemY = 280,
            preItemNumY = 35,
            startR = 10000,
            minAngle = 5,
            startAngle = 5,
            maxAngle = 25,
            angleDensity = 30,
            startStepR = 150,
            startStepAngle = 1,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 400,
            frameItemX = 9,
            itemNumY = 24,
            itemYGapNum = 10,
            itemYGap = 200,
            itemNumX = 4,
            itemYGapFrame = 14,
            baseTimes = 1
        };

        // 阵型 4
        public static readonly SheepRoleFormation formation_4 = new() {
            id = 4,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 300,
            preItemX = 1000,
            preItemY = 500,
            preItemNumY = 20,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 550,
            frameItemX = 12,
            itemNumY = 20,
            itemYGapNum = 50,
            itemYGap = 0,
            itemNumX = 2,
            itemYGapFrame = 14,
            baseTimes = 1
        };

        // 阵型 5
        public static readonly SheepRoleFormation formation_5 = new() {
            id = 5,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 1800,
            preItemX = 1200,
            preItemY = 2000,
            preItemNumY = 4,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 4000,
            itemY = 1600,
            frameItemX = 40,
            itemNumY = 8,
            itemYGapNum = 4,
            itemYGap = 400,
            itemNumX = 2,
            itemYGapFrame = 60,
            baseTimes = 1
        };

        // 阵型 6
        public static readonly SheepRoleFormation formation_6 = new() {
            id = 6,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 600,
            preItemX = 800,
            preItemY = 2200,
            preItemNumY = 3,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 1600,
            frameItemX = 40,
            itemNumY = 3,
            itemYGapNum = 4,
            itemYGap = 400,
            itemNumX = 2,
            itemYGapFrame = 60,
            baseTimes = 1
        };

        // 阵型 7
        public static readonly SheepRoleFormation formation_7 = new() {
            id = 7,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 300,
            preItemX = 800,
            preItemY = 1200,
            preItemNumY = 8,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 2000,
            itemY = 1600,
            frameItemX = 40,
            itemNumY = 5,
            itemYGapNum = 4,
            itemYGap = 400,
            itemNumX = 2,
            itemYGapFrame = 60,
            baseTimes = 1
        };

        // 阵型 8
        public static readonly SheepRoleFormation formation_8 = new() {
            id = 8,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 300,
            preItemX = 800,
            preItemY = 1200,
            preItemNumY = 8,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 1600,
            frameItemX = 40,
            itemNumY = 5,
            itemYGapNum = 4,
            itemYGap = 400,
            itemNumX = 2,
            itemYGapFrame = 60,
            baseTimes = 1
        };

        // 阵型 9
        public static readonly SheepRoleFormation formation_9 = new() {
            id = 9,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 300,
            preItemX = 1000,
            preItemY = 1800,
            preItemNumY = 5,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 700,
            frameItemX = 12,
            itemNumY = 10,
            itemYGapNum = 50,
            itemYGap = 0,
            itemNumX = 2,
            itemYGapFrame = 14,
            baseTimes = 1
        };

        // 阵型 10
        public static readonly SheepRoleFormation formation_10 = new() {
            id = 10,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 500,
            preItemX = 300,
            preItemY = 500,
            preItemNumY = 20,
            startR = 2500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 5000,
            itemY = 500,
            frameItemX = 4,
            itemNumY = 20,
            itemYGapNum = 12,
            itemYGap = 120,
            itemNumX = 6,
            itemYGapFrame = 3,
            baseTimes = 1
        };

        // 阵型 11
        public static readonly SheepRoleFormation formation_11 = new() {
            id = 11,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 2000,
            preItemX = 300,
            preItemY = 500,
            preItemNumY = 20,
            startR = 3000,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 6600,
            itemY = 500,
            frameItemX = 4,
            itemNumY = 20,
            itemYGapNum = 12,
            itemYGap = 120,
            itemNumX = 6,
            itemYGapFrame = 3,
            baseTimes = 1
        };

        // 阵型 12
        public static readonly SheepRoleFormation formation_12 = new() {
            id = 12,
            name = "",
            formationType = SheepRoleFormationType.RectangleTidy,
            preStartX = 1200,
            preItemX = 300,
            preItemY = 500,
            preItemNumY = 20,
            startR = 3500,
            minAngle = 0,
            startAngle = 5,
            maxAngle = 30,
            angleDensity = 30,
            startStepR = 0,
            startStepAngle = 2,
            minScope = 0,
            startScope = 200,
            maxScope = 1600,
            density = 30,
            startTimeStep = 33,
            frameMaxCount = 200,
            startX = 6000,
            itemY = 500,
            frameItemX = 4,
            itemNumY = 20,
            itemYGapNum = 12,
            itemYGap = 120,
            itemNumX = 6,
            itemYGapFrame = 3,
            baseTimes = 1
        };

        public static readonly SheepRoleFormation[] All = {
            formation_0,
            formation_1,
            formation_2,
            formation_3,
            formation_4,
            formation_5,
            formation_6,
            formation_7,
            formation_8,
            formation_9,
            formation_10,
            formation_11,
            formation_12,
        };

        private static readonly Dictionary<int, SheepRoleFormation> Map = BuildMap();

        public static SheepRoleFormation GetById(int id) {
            if (!Map.TryGetValue(id, out SheepRoleFormation formation)) {
                throw new KeyNotFoundException($"不存在 SheepRoleFormation 配置，ID: {id}");
            }

            return formation;
        }

        public static bool TryGetById(int id, out SheepRoleFormation formation) {
            return Map.TryGetValue(id, out formation);
        }

        private static Dictionary<int, SheepRoleFormation> BuildMap() {
            var map = new Dictionary<int, SheepRoleFormation>(All.Length);

            foreach (SheepRoleFormation formation in All) {
                if (!map.TryAdd(formation.id, formation)) {
                    throw new InvalidOperationException($"SheepRoleFormation 存在重复 ID: {formation.id}");
                }
            }

            return map;
        }
    }
}