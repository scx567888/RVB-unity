namespace sheep {
    // 测试
    public class PetLogic {
        public static PetLogic INSTANCE = new PetLogic();

        public void tick(Pet pet, SheepWorld sheepWorld) {
            pet.moveMode = PetMoveMode.TARGET;
            pet.targetX = sheepWorld.bossX;
            pet.targetY = sheepWorld.bossY;
        }
    }
}