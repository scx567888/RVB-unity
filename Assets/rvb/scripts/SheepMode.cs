namespace rvb.scripts {
    public class SheepMode {

        // boss 坐标
        public int loongX;
        public int startAddR;
        public int startAddX;

        public SheepMode() {
            this.loongX = 4500;
            this.startAddR = 1500;
            this.startAddX = 800;
        }
    }

    public static class SheepModes {
        
        public static SheepMode sheepMode = new SheepMode();
        
    }
}