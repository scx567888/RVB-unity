namespace rvb.scripts {
    public class BullteCreate {
        public PetView view_pet;
        public int bulletId;
        public PetView view_tar_pet;
        public Info info;

        public class Info {
            public float startX;
            public float startY;
            public float startZ;
            public float endX;
            public float endY;
            public float endZ;
            public float dirX;
            public float dirY;
            public float dirZ;
            public float angle;
            public SheepCamp camp;
            public float atk;
            public bool hasStart;
            public bool hasEnd;
        }
    }
}