namespace sheep {
    // 测试
    public class PetLogic {
        public static PetLogic INSTANCE = new PetLogic();

        public void tick(Pet pet, SheepWorld sheepWorld) {
            if (pet.id % 2 == 0) {
                pet.moveMode = PetMoveMode.TARGET;
                pet.targetX = sheepWorld.bossX;
                pet.targetY = sheepWorld.bossY;
            }
            else {
                pet.moveMode = PetMoveMode.TARGET;
                pet.targetX = sheepWorld.boss1X;
                pet.targetY = sheepWorld.boss1Y;
            }
        }
    }
}