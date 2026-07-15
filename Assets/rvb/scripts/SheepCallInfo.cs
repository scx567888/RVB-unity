using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepCallInfo {
        public SheepCamp camp;
        public int type;
        public int count;
        public int frame;
        public int[] items;
        public List<SheepCallInfoPet> pets;
        public int hasName;
        public int count_line;
    }

    public class SheepCallInfoPet {
        public SheepCamp camp;
        public int count;
    }
    
}