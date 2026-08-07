using System;
using UnityEngine;

namespace rvb.scripts {
    public class SheepCtl {
        public ComMatch comMatch=new ComMatch();
        public ComUIAnim comUIAnim=new ();
        public CameraCtl cameraCtl=new ();
        public ComImages comImages=new ();
        public Action<int,int, SheepCamp> addFrameBlockCampCallback;
        public void addFrameBlockCamp(int blockIndex, SheepCamp bulletCamp) {
            
        }
        public Boss[] boss=new [] {
            new Boss(0),
            new Boss(0),
        };
        public static SheepCtl instance=new SheepCtl();

        public void addFrameBlockCamp(int gridX, int gridY, SheepCamp camp) {
            if (addFrameBlockCampCallback==null) {
                return;
            }
            addFrameBlockCampCallback(gridX, gridY, camp);
        }
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
        public CameraCtlCamera camera=new CameraCtlCamera();

        public void onShake(int shockBeginNumber) {
            
        }
    }

    public class CameraCtlCamera {
        public CameraCtlCameraNode node=new CameraCtlCameraNode();
    }

    public class CameraCtlCameraNode {
        public Vector3 eulerAngles=new Vector3();
    }

}