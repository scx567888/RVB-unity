using System.Collections.Generic;
using UnityEngine;

namespace sheep {
    public class SheepWorld {
        // 当前在场上的角色
        public List<Pet> pets = new();

        // 准备添加到下一帧的 角色
        public List<Pet> pre_pets = new();

        // 准备删除的 角色, 这里用 Set 保证查询速度
        public HashSet<Pet> del_pets = new();

        // 角色自增 id 
        public int petId = 0;

        // *********************** 单位添加删除相关 ***************************

        // 获取 petId
        public int getNextPetId() {
            return ++petId;
        }

        // 添加单位, 不要在逻辑帧循环中调用
        public void addPet(Pet pet) {
            pets.Add(pet);
        }

        // 删除单位, 不要在逻辑帧循环中调用
        public void delPet(Pet pet) {
            pets.Remove(pet);
        }

        // 添加单位 下一帧才会使用
        public void addPrePet(Pet pet) {
            pre_pets.Add(pet);
        }

        // 添加 删除单位.
        public void addDelPet(Pet pet) {
            del_pets.Add(pet);
        }

        // 将 pre_pets 应用到 pets 中
        public void applyPrePets() {
            foreach (var pet in pre_pets) {
                addPet(pet);
            }

            pre_pets.Clear();
        }

        // 从 pets 中 删除 del_pets 中的 单位
        public HashSet<Pet> applyDelPets() {
            // 复制一份方便 渲染层处理
            var copy = new HashSet<Pet>(del_pets);

            // 应用移除
            foreach (var pet in del_pets) {
                delPet(pet);
            }

            // 清空
            del_pets.Clear();

            return copy;
        }

        // **************************** 逻辑相关 ***************************

        public void petsAction() {
            foreach (var pet in pets) {
                pet.action(this);
            }
        }

        // Tick
        public HashSet<Pet> tick() {
            // 应用 预添加数据
            applyPrePets();

            this.petsAction();

            var del_pets1 = applyDelPets();

            return del_pets1;
        }
    }
}