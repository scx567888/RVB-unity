using System.Collections.Generic;
using scx.GridMap;

namespace rvb.scripts {
    public class SheepCell : GridCell {
        public readonly List<PetView> pets;

        public SheepCell(int gridX, int gridY, float worldStartX, float worldStartY, float worldEndX, float worldEndY) : base(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) {
            this.pets = new List<PetView>();
        }
    }
}