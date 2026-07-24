using System;
using System.Collections.Generic;
using scx.GridMap;

namespace rvb.scripts {
    public class SheepCell : GridCell {
        // 当前格子的角色列表, 按照 [阵营][碰撞ID] 存储
        private readonly List<PetView>[][] pets;

        // 当前格子的角色数量, 按照 [阵营] 存储
        public int[] petCounts = { 0, 0 };

        public SheepCell(int gridX, int gridY, float worldStartX, float worldStartY, float worldEndX, float worldEndY) :
            base(gridX, gridY, worldStartX, worldStartY, worldEndX, worldEndY) {
            this.pets = new[] {
                new List<PetView>[SheepConfig.MaxGroupCount],
                new List<PetView>[SheepConfig.MaxGroupCount]
            };
        }

        public void addPet(PetView pet) {
            var p1 = this.pets[(int)pet.camp][pet.conf.collideId];
            if (p1 == null) {
                p1 = new List<PetView>();
                this.pets[(int)pet.camp][pet.conf.collideId] = p1;
            }

            p1.Add(pet);
            petCounts[(int)pet.camp] += 1;
        }

        // callback 返回 false: 继续
        // callback 返回 true: 停止
        // 返回值表示 是否 调用过 callback 并且 callback 提前终止
        public bool forEachPet(Func<PetView, bool> callback) {
            foreach (var p1 in this.pets) {
                foreach (var p2 in p1) {
                    if (p2 != null) {
                        foreach (var pet in p2) {
                            var stop = callback(pet);
                            if (stop == true) {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        // callback 返回 false: 继续
        // callback 返回 true: 停止
        // 返回值表示 是否 调用过 callback 并且 callback 提前终止
        public bool forEachPet(SheepCamp camp, Func<PetView, bool> callback) {
            var p1 = pets[(int)camp];

            foreach (var p2 in p1) {
                if (p2 != null) {
                    foreach (var pet in p2) {
                        var stop = callback(pet);
                        if (stop == true) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // callback 返回 false: 继续
        // callback 返回 true: 停止
        // 返回值表示 是否 调用过 callback 并且 callback 提前终止
        public bool forEachPet(SheepCamp camp, int collideId, Func<PetView, bool> callback) {
            var p1 = pets[(int)camp];
            var p2 = p1[collideId];

            if (p2 != null) {
                foreach (var pet in p2) {
                    var stop = callback(pet);
                    if (stop == true) {
                        return true;
                    }
                }
            }

            return false;
        }

        public void clearPets() {
            foreach (var p1 in this.pets) {
                foreach (var p2 in p1) {
                    if (p2 != null) {
                        p2.Clear();
                    }
                }
            }

            petCounts = new[] { 0, 0 };
        }
    }
}