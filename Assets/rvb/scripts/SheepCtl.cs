namespace rvb.scripts {
    public class SheepCtl {
        public ComMatch comMatch=new ComMatch();
        public ComUIAnim comUIAnim=new ();
        public CameraCtl cameraCtl=new ();
        public ComImages comImages=new ();
        public static SheepCtl instance=new SheepCtl();
    }

    public class ComMatch {
        public void showDoubleAnim(SheepCamp camp) {
        }

        public void hideDoubleAnim(SheepCamp camp) {
            
        }

        public void updateWinloops() {
            
            
        }
    }

    public class ComUIAnim {
        public void backAnim(SheepCamp camp) {
        }

        public void backSuccessAnim(SheepCamp camp) {
            
        }
    }

    public class CameraCtl {
        public void onShake(int shockBeginNumber) {
            
        }
    }

}