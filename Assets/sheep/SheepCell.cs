using System;
using System.Collections.Generic;
using scx.GridMap;

namespace sheep {
    public class SheepCell : GridCell {
        // 当前格子的角色列表
        private readonly List<Pet> pets;

        public SheepCell(
            int gridX, int gridY,
            float worldStartX, float worldStartY,
            float worldEndX, float worldEndY
        ) :
            base(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) {
            this.pets = new List<Pet>();
        }

        public void addPet(Pet pet) {
            this.pets.Add(pet);
        }

        // callback 返回 false: 继续
        // callback 返回 true: 停止
        // 返回值表示 是否 调用过 callback 并且 callback 提前终止
        public bool forEachPet(Func<Pet, bool> callback) {
            foreach (var pet in this.pets) {
                var stop = callback(pet);
                if (stop) {
                    return true;
                }
            }

            return false;
        }

        public void clearPets() {
            this.pets.Clear();
        }
    }
}