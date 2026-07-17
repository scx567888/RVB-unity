namespace rvb.scripts {
    public class Boss : PetView{
        public long backStateTime;
        public float curHp;
        public ComProgress comProgress=new ComProgress();

        public Boss(int t) : base(t) {
            
        }

        public bool subShield() {
            return true;
        }

        public void updateState(SheepCtl sheepCtl, SheepMgr manager, int visualState) {
            
        }

        public void updateStateJJL(SheepCtl sheepCtl, SheepMgr manager, int visualState) {
            
        }

        public void hitAnim() {
            
            
        }
        
       
    }

    public class ComProgress {
        public float _vue;

        public void setVue(float f) {
            _vue = f;
        }
    }
}