namespace rvb.scripts {
    public class BullteCreate {
       public PetView view_pet;
       public int bulletId;
       public PetView view_tar_pet;
       public Info info;
    }

    public class Info {
        public int startX;
        public int startY;
        public int startZ;
        public int endX;
        public int endY;
        public int endZ;
        public int dirX;
        public int dirY;
        public int dirZ;
        public int angle;
        public SheepCamp camp;
        public float atk;
    }
}