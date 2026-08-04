namespace sheep {
    // 测试
    public class PetLogic {
        public static PetLogic INSTANCE = new PetLogic();

        public void tick(Pet pet, SheepWorld sheepWorld) {
            // 简化的 索敌逻辑 
            if (pet.id % 2 == 0) {
                pet.moveIntent.moveMode = PetMoveMode.TELEPORT;
                pet.moveIntent.targetX = sheepWorld.bossX;
                pet.moveIntent.targetY = sheepWorld.bossY;
            }
            else {
                pet.moveIntent.moveMode = PetMoveMode.TARGET;
                pet.moveIntent.targetX = sheepWorld.boss1X;
                pet.moveIntent.targetY = sheepWorld.boss1Y;
            }
        }
    }
}