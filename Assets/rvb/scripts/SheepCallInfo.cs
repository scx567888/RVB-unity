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

        // 逆向源码中存在这两个可选字段，普通 produce_pets 不会设置。
        public Stack<bool> booms;
        public SheepCamp player;
    }
}