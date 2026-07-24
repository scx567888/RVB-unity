using System;
using System.Collections.Generic;
using UnityEngine;

namespace rvb.scripts {
    public static class UtilFind {
        public static SheepMgr system;

        public static FindTarResult findTar(PetView petSkin, int findR = 0) {
            float i = petSkin.posX;
            float o = petSkin.posY;
            (int xn, int yn) block = Util.getXnYn(i, o);
            int xn = block.xn;
            int yn = block.yn;
            PetView r = null;
            PetView a = null;
            float c = 0f;

            if (findR == 0) {
                findR = petSkin.conf.findR;
            }

            forNearBlocksByAckView(petSkin, xn, yn, findR, targetPetView => {
                if (!targetPetView.isDie && targetPetView.camp != petSkin.camp && targetPetView.roleId != 0) {
                    if (Util.isCanAckByRole(petSkin, targetPetView)) {
                        r = targetPetView;
                        return true;
                    }

                    if (petSkin.conf.isFindMoveTar && a == null && Util.isCanMove(petSkin, targetPetView)) {
                        float tx = targetPetView.posX - petSkin.posX;
                        float ty = targetPetView.posY - petSkin.posY;
                        c = tx * tx + ty * ty;
                        a = targetPetView;
                    }
                    else if (petSkin.conf.isFindMoveTar && a != null && Util.isCanMove(petSkin, targetPetView)) {
                        float tx = targetPetView.posX - petSkin.posX;
                        float ty = targetPetView.posY - petSkin.posY;
                        float distance = tx * tx + ty * ty;
                        if (distance < c) {
                            c = distance;
                            a = targetPetView;
                        }
                    }

                    return false;
                }

                return false;
            });

            if (r != null) {
                petSkin.tarPosX = r.posX;
                petSkin.tarPosY = r.posY;
                return new FindTarResult() {  atkTar= r  };
            }

            PetView backBoss = Util.getBackBoss(petSkin.camp);
            if (Util.isCanAckByRole(petSkin, backBoss)) {
                petSkin.tarPosX = backBoss.posX;
                petSkin.tarPosY = backBoss.posY;
                return new FindTarResult() { atkTar= backBoss } ;
            }

            if (a != null) {
                return new FindTarResult() {  moveTar= a };
            }

            if (petSkin.state == SheepRoleState.Spurt && petSkin.conf.skillSpurt == 0) {
                PetView t = null;
                findNearBlocksByCollisionView(petSkin, xn, yn, petSkin.conf.findR, target => {
                    if (target.state == SheepRoleState.Move) {
                        float s = target.posX - petSkin.posX;
                        float targetY = target.posY - petSkin.posY;
                        float distance = s * s + targetY * targetY;
                        float radius = target.conf.collideR + petSkin.conf.collideR;
                        if (distance < radius * radius * 0.25f) {
                            t = target;
                            return true;
                        }
                    }

                    return false;
                });

                if (t != null) {
                    return new FindTarResult() { moveTar= t };
                }
            }

            if (
                petSkin.state != SheepRoleState.Spurt ||
                petSkin.camp == SheepCamp.Red && petSkin.posX > petSkin.conf.runEndX ||
                petSkin.camp == SheepCamp.Blue && petSkin.posX < petSkin.conf.runEndX
            ) {
                return new FindTarResult() { moveBoss= backBoss } ;
            }

            return new FindTarResult();
        }

        public static PetView findNearAck(PetView petSkin) {
            float t = petSkin.posX;
            float i = petSkin.posY;
            (int xn, int yn) block = Util.getXnYn(t, i);
            PetView l = null;

            findNearBlocksByAckView(petSkin, block.xn, block.yn, petSkin.conf.findR, target => {
                if (!target.isDie && target.camp != petSkin.camp && target.roleId != 0 &&
                    Util.isCanAckByRole(petSkin, target)) {
                    l = target;
                    return true;
                }

                return false;
            });

            if (l != null) {
                return l;
            }

            if (l == null) {
                PetView target = Util.getBackBoss(petSkin.camp);
                if (Util.isCanAckByRole(petSkin, target)) {
                    l = target;
                }
            }

            return l;
        }

        public static PetView findFarAck(PetView e, int findR) {
            float posX = e.posX;
            float posY = e.posY;
            (int xn, int yn) block = Util.getXnYn(posX, posY);
            PetView n = null;

            findFarBlocksByAckView(e, block.xn, block.yn, findR, target => {
                n = target;
                return true;
            });

            if (n == null) {
                PetView t = Util.getBackBoss(e.camp);
                if (Util.isCanAckByRole(e, t)) {
                    n = t;
                }
            }

            return n;
        }

        public static PetView findRandomAck(PetView e, int findR) {
            float i = e.posX;
            float s = e.posY;
            (int xn, int yn) block = Util.getXnYn(i, s);
            PetView n = null;

            findRandomBlocksByAckView(e, block.xn, block.yn, findR, target => {
                n = target;
                return true;
            });

            if (n == null) {
                PetView t = Util.getBackBoss(e.camp);
                if (Util.isCanAckByRole(e, t)) {
                    n = t;
                }
            }

            return n;
        }

        public static int getAtkRank(PetView petView, PetView targetPetView) {
            if (petView.conf.findAtkSort != null) {
                for (int i = 0; i < petView.conf.findAtkSort.Length; i++) {
                    if (petView.conf.findAtkSort[i] == (int)targetPetView.conf.roleType) {
                        return i;
                    }
                }
            }

            return 100;
        }

        public static PetView findSortAck(PetView petView, int targetPetView) {
            float posX = petView.posX;
            float posY = petView.posY;
            (int xn, int yn) block = Util.getXnYn(posX, posY);
            PetView n = null;
            int r = 100;
            int a = 0;

            if (petView.conf.findAtkSort != null) {
                a = petView.conf.findAtkSort[0];
            }

            findNearBlocksByAckView(petView, block.xn, block.yn, targetPetView, t => {
                if (!Util.isCanAckByRole(petView, t)) {
                    return false;
                }

                if (n == null) {
                    n = t;
                    r = getAtkRank(petView, t);
                    return false;
                }

                if (t.roleId == (int)a) {
                    n = t;
                    return true;
                }

                PetView target = t;
                int s = getAtkRank(petView, t);
                if (s < r) {
                    n = target;
                    r = s;
                }

                return false;
            });

            if (n == null) {
                PetView t = Util.getBackBoss(petView.camp);
                if (Util.isCanAckByRole(petView, t)) {
                    n = t;
                }
            }

            return n;
        }

        public static PetView findSortAck1(PetView petSkin, int findR) {
            float i = petSkin.posX;
            float s = petSkin.posY;
            (int xn, int yn) block = Util.getXnYn(i, s);
            PetView n = null;
            int r = 100;
            int a = 0;

            if (petSkin.conf.findAtkSort != null) {
                a = petSkin.conf.findAtkSort[0];
            }

            findNearBlocksByAckView(petSkin, block.xn, block.yn, findR, t => {
                if (n == null) {
                    n = t;
                    r = getAtkRank(petSkin, t);
                    return false;
                }

                if (t.roleId == (int)a) {
                    n = t;
                    return true;
                }

                PetView target = t;
                int rank = getAtkRank(petSkin, t);
                if (rank < r) {
                    n = target;
                    r = rank;
                }

                return false;
            });

            if (n == null) {
                PetView backBoss = Util.getBackBoss(petSkin.camp);
                if (Util.isCanAckByRole(petSkin, backBoss)) {
                    n = backBoss;
                }
            }

            return n;
        }

        public static void foreachFront(PetView e, Action<PetView> t, int i = 0, float o = 30f) {
            float l = e.posX;
            float n = e.posY;
            (int xn, int yn) block = Util.getXnYn(l, n);
            float c = e.tarPosX - l;
            float f = e.tarPosY - n;
            float h = Mathf.Sqrt(c * c + f * f);

            if (h > 0f) {
                c /= h;
                f /= h;
                if (i == 0) {
                    i = e.conf.findR;
                }
            }
            else {
                c = e.camp == SheepCamp.Red ? 1f : -1f;
                f = 0f;
                if (i == 0) {
                    i = e.conf.findR;
                }
            }

            float p = Mathf.Cos(o * Mathf.PI / 180f);
            PetView u = null;
            float d = float.PositiveInfinity;

            forNearBlocksByAckView(e, block.xn, block.yn, i, target => {
                if (!target.isDie && target.camp != e.camp && target.roleId != 0 && Util.isCanAckByRole(e, target)) {
                    float targetX = target.posX - l;
                    float targetY = target.posY - n;
                    float distanceSqr = targetX * targetX + targetY * targetY;
                    float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                    if (distance != 0f) {
                        if ((targetX * c + targetY * f) / distance > p && distanceSqr < d) {
                            d = distanceSqr;
                            u = target;
                            t(u);
                        }
                    }

                    return false;
                }

                return false;
            });
        }

        public static void forfeachBlocksByAckView(SheepCamp camp, int xn, int yn, int splitN, Action<PetView> callback) {
            // 寻找敌方阵营
            var enemyCamp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
            
            for (int n = -splitN; n <= splitN; n++) {
                for (int r = -splitN; r <= splitN; r++) {
                    
                    var sheepCell = system.gridMap.getCell(xn + n, yn + r);
                    if (sheepCell==null) {
                        continue;
                    }
                    sheepCell.forEachPet(enemyCamp, (p) => {
                        callback(p);
                        return false;
                    });
                    
                }
            }
            
        }

        public static void forfeachBlocksByCollView(PetView petSkin, int xn, int yn, int splitN, Action<PetView> callback) {
            var camp = petSkin.camp;
            var collideId = petSkin.conf.collideId;
            
            for (int n = -splitN; n <= splitN; n++) {
                for (int r = -splitN; r <= splitN; r++) {
                    
                    var sheepCell = system.gridMap.getCell(xn + n, yn + r);
                    if (sheepCell==null) {
                        continue;
                    }
                    sheepCell.forEachPet(camp,collideId, (p) => {
                        callback(p);
                        return false;
                    });
                    
                }
            }
        }

        public static bool forNearBlocksByAckView(PetView e, int t, int i, int o, Func<PetView, bool> callback) {
            // 寻找敌方阵营
            var enemyCamp = e.camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
             int n = 0;
             
             Func<int, int, bool> r = (blockX, blockY) => {
                 var sheepCell = system.gridMap.getCellSafe(blockX, blockY);
                  sheepCell.forEachPet(enemyCamp,callback);
                  return false;
             };

            for (int ring = 0; ring <= o; ring++) {
                if (ring != 0) {
                    Vector2Int topLeft = new Vector2Int(t - n, i + n);
                    Vector2Int topRight = new Vector2Int(t + n, i + n);
                    Vector2Int bottomRight = new Vector2Int(t + n, i - n);
                    Vector2Int bottomLeft = new Vector2Int(t - n, i - n);

                    if (system.Random01() < 0.5f) {
                        for (int x = topLeft.x; x < topRight.x; x++) {
                            if (r(x, topLeft.y)) return true;
                        }

                        for (int y = topRight.y; y > bottomRight.y; y--) {
                            if (r(topRight.x, y)) return true;
                        }

                        for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                            if (r(x, bottomRight.y)) return true;
                        }

                        for (int y = bottomLeft.y; y < topLeft.y; y++) {
                            if (r(bottomLeft.x, y)) return true;
                        }
                    }
                    else {
                        for (int x = topRight.x; x > topLeft.x; x--) {
                            if (r(x, topLeft.y)) return true;
                        }

                        for (int y = topLeft.y; y > bottomLeft.y; y--) {
                            if (r(bottomLeft.x, y)) return true;
                        }

                        for (int x = bottomLeft.x; x < bottomRight.x; x++) {
                            if (r(x, bottomRight.y)) return true;
                        }

                        for (int y = bottomRight.y; y < topRight.y; y++) {
                            if (r(topRight.x, y)) return true;
                        }
                    }
                }
                else if (r(t, i)) {
                    return true;
                }

                n += 1;
            }

            return false;
        }

        public static bool findFarBlocksByAckView(PetView petSkin, int xn, int yn, int findR, Func<PetView, bool> callback) {
            SheepCamp camp = petSkin.camp;
            camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
            
               Func<int, int, bool> n = (blockX, blockY) => {
                   
               var sheepCell = system.gridMap.getCellSafe(blockX, blockY);
                
                return sheepCell.petCounts[(int)camp] != 0;
            };

            Func<int, int, bool> a = (blockX, blockY) => {

                var sheepCell = system.gridMap.getCellSafe(blockX, blockY);
                
                return sheepCell.forEachPet(camp, callback);

            };

            for (int ring = findR; ring > 0; ring--) {
                Vector2Int topLeft = new Vector2Int(xn - ring, yn + ring);
                Vector2Int topRight = new Vector2Int(xn + ring, yn + ring);
                Vector2Int bottomRight = new Vector2Int(xn + ring, yn - ring);
                Vector2Int bottomLeft = new Vector2Int(xn - ring, yn - ring);
                HashSet<Vector2Int> c = new HashSet<Vector2Int>();

                for (int x = topLeft.x; x < topRight.x; x++) {
                    if (n(x, topLeft.y)) c.Add(new Vector2Int(x, topLeft.y));
                }

                for (int y = topRight.y; y > bottomRight.y; y--) {
                    if (n(topRight.x, y)) c.Add(new Vector2Int(topRight.x, y));
                }

                for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                    if (n(x, bottomRight.y)) c.Add(new Vector2Int(x, bottomRight.y));
                }

                for (int y = bottomLeft.y; y < topLeft.y; y++) {
                    if (n(bottomLeft.x, y)) c.Add(new Vector2Int(bottomLeft.x, y));
                }

                while (c.Count != 0) {
                    List<Vector2Int> points = new List<Vector2Int>();
                    foreach (Vector2Int point in c) {
                        points.Add(point);
                    }

                    int randomIndex = system.RandomInt(0, c.Count);
                    Vector2Int pointToCheck = points[randomIndex];
                    if (a(pointToCheck.x, pointToCheck.y)) {
                        return true;
                    }

                    c.Remove(pointToCheck);
                }
            }

            return n(xn, yn) && a(xn, yn);
            
        }

        public static bool findRandomBlocksByAckView(PetView e, int t, int i, int findR, Func<PetView, bool> callback) {
            SheepCamp camp = e.camp;
            camp = camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;
            var r = system.attackViews[(int)camp];
            var a = system.attackView1s[(int)camp];
            return findRandomBlocks(r, a, t, i, findR, callback);
        }
        
        public static bool findNearBlocksByAckView(PetView e, int xn, int yn, int o, Func<PetView, bool> callback) {
            // 寻找敌方阵营
            var enemyCamp = e.camp == SheepCamp.Red ? SheepCamp.Blue : SheepCamp.Red;

            Func<int, int, bool> forEachPetByCell = (blockX, blockY) => {
                var sheepCell = system.gridMap.getCellSafe(blockX, blockY);
                return sheepCell.forEachPet(enemyCamp,callback);
            };
            
            int n = 0;

            for (int ring = 0; ring <= o; ring++) {
                if (ring != 0) {
                    Vector2Int topLeft = new Vector2Int(xn - n, yn + n);
                    Vector2Int topRight = new Vector2Int(xn + n, yn + n);
                    Vector2Int bottomRight = new Vector2Int(xn + n, yn - n);
                    Vector2Int bottomLeft = new Vector2Int(xn - n, yn - n);

                    if (system.Random01() < 0.5f) {
                        for (int x = topLeft.x; x < topRight.x; x++) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topRight.y; y > bottomRight.y; y--) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomLeft.y; y < topLeft.y; y++) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }
                    }
                    else {
                        for (int x = topRight.x; x > topLeft.x; x--) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topLeft.y; y > bottomLeft.y; y--) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomLeft.x; x < bottomRight.x; x++) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomRight.y; y < topRight.y; y++) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }
                    }
                }
                else if (forEachPetByCell(xn, yn)) {
                    return true;
                }

                n += 1;
            }

            return false;
        }

        public static bool findNearBlocksByCollisionView(PetView e, int xn, int yn, int o, Func<PetView, bool> callback) {
            // 寻找乙方阵营 和 碰撞 id 相同的 
            var camp = e.camp;
            var collideId = e.conf.collideId;

            Func<int, int, bool> forEachPetByCell = (blockX, blockY) => {
                var sheepCell = system.gridMap.getCellSafe(blockX, blockY);
                return sheepCell.forEachPet(camp,collideId,callback);
            };
            
            int n = 0;

            for (int ring = 0; ring <= o; ring++) {
                if (ring != 0) {
                    Vector2Int topLeft = new Vector2Int(xn - n, yn + n);
                    Vector2Int topRight = new Vector2Int(xn + n, yn + n);
                    Vector2Int bottomRight = new Vector2Int(xn + n, yn - n);
                    Vector2Int bottomLeft = new Vector2Int(xn - n, yn - n);

                    if (system.Random01() < 0.5f) {
                        for (int x = topLeft.x; x < topRight.x; x++) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topRight.y; y > bottomRight.y; y--) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomLeft.y; y < topLeft.y; y++) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }
                    }
                    else {
                        for (int x = topRight.x; x > topLeft.x; x--) {
                            if (forEachPetByCell(x, topLeft.y)) {
                                return true;
                            }
                        }

                        for (int y = topLeft.y; y > bottomLeft.y; y--) {
                            if (forEachPetByCell(bottomLeft.x, y)) {
                                return true;
                            }
                        }

                        for (int x = bottomLeft.x; x < bottomRight.x; x++) {
                            if (forEachPetByCell(x, bottomRight.y)) {
                                return true;
                            }
                        }

                        for (int y = bottomRight.y; y < topRight.y; y++) {
                            if (forEachPetByCell(topRight.x, y)) {
                                return true;
                            }
                        }
                    }
                }
                else if (forEachPetByCell(xn, yn)) {
                    return true;
                }

                n += 1;
            }

            return false;
        }

        public static bool findRandomBlocks(IndexLen[] e, PetView[] t, int i, int s, int findR, Func<PetView, bool> callback) {
            Func<int, int, bool> n = (blockX, blockY) => {
                int blockIndex = Util.getIndexByXnYn(blockX, blockY);
                if (blockIndex < 0 || blockIndex >= SheepConfig.line_w * SheepConfig.line_w) {
                    return false;
                }

                var block = system.getBlockByIndex(e, blockIndex);
                return block.Len != 0;
            };

            Func<int, int, bool> a = (blockX, blockY) => {
                int blockIndex = Util.getIndexByXnYn(blockX, blockY);
                return system.findBlock(e, t, blockIndex, (Func<PetView, bool>)(petIndex => {
                    PetView petSkin = petIndex;
                    if (petSkin != null) {
                        bool result = callback(petSkin);
                        petSkin = null;
                        return result;
                    }

                    return false;
                }));
            };

            List<int> c = new List<int>();
            for (int ring = 0; ring <= findR; ring++) {
                c.Add(ring);
            }

            c.Sort((left, right) => system.Random01() < 0.5f ? -1 : 1);

            for (int ringIndex = 0; ringIndex <= findR; ringIndex++) {
                int ring = c[ringIndex];
                Vector2Int topLeft = new Vector2Int(i - ring, s + ring);
                Vector2Int topRight = new Vector2Int(i + ring, s + ring);
                Vector2Int bottomRight = new Vector2Int(i + ring, s - ring);
                Vector2Int bottomLeft = new Vector2Int(i - ring, s - ring);
                List<Vector2Int> h = new List<Vector2Int>();

                for (int x = topLeft.x; x < topRight.x; x++) {
                    if (n(x, topLeft.y)) h.Add(new Vector2Int(x, topLeft.y));
                }

                for (int y = topRight.y; y > bottomRight.y; y--) {
                    if (n(topRight.x, y)) h.Add(new Vector2Int(topRight.x, y));
                }

                for (int x = bottomRight.x; x > bottomLeft.x; x--) {
                    if (n(x, bottomRight.y)) h.Add(new Vector2Int(x, bottomRight.y));
                }

                for (int y = bottomLeft.y; y < topLeft.y; y++) {
                    if (n(bottomLeft.x, y)) h.Add(new Vector2Int(bottomLeft.x, y));
                }

                h.Sort((left, right) => system.Random01() < 0.5f ? -1 : 1);
                while (h.Count != 0) {
                    int lastIndex = h.Count - 1;
                    Vector2Int point = h[lastIndex];
                    h.RemoveAt(lastIndex);
                    if (a(point.x, point.y)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}