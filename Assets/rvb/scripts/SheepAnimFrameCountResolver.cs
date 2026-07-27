using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepAnimFrameCountResolver {
        public Dictionary<int, int[]>[] data = new[] {
            new Dictionary<int, int[]>(),
            new Dictionary<int, int[]>(),
        };

        public SheepAnimFrameCountResolver() {
        }

        public void setAnimationFrameCount(SheepCamp camp, int animId, SheepRoleAnimType animType, int count) {
            if (!data[(int)camp].TryGetValue(animId, out int[] frameCounts)) {
                frameCounts = new int[(int)SheepRoleAnimType.Count];
                data[(int)camp][animId] = frameCounts;
            }

            frameCounts[(int)animType] = count;
        }

        public int resolve(SheepCamp camp, int animId, SheepRoleAnimType animType) {
            return data[(int)camp][animId][(int)animType];
        }
    }
}