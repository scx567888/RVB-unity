using System;
using System.Collections.Generic;
using UnityEngine;

namespace rvb.scripts {
   public static class UtilAck
{
    public static SheepMgr system;

    public static void ackTar(PetView e, PetView t)
    {
        float i = e.conf.atk;

        if (e.curAtkBuff != 0)
        {
            i = Mathf.Floor(i * (1f + e.curAtkBuff / 100f));
        }

        if (Util.isCanAckByRole(e, t))
        {
            hurtByRole(e, t, i);
        }

        if (e.roleId != 0 && t.roleId != 0)
        {
            (int xn, int yn) block = Util.getXnYn(t.posX, t.posY);

            UtilFind.forfeachBlocksByAckView(e.camp, block.xn, block.yn, e.conf.splitN, target =>
            {
                if (!target.isDie && target.roleId != 0 && target.camp == t.camp && target.id != t.id && target.curHp > 0)
                {
                    float o = t.posX - target.posX;
                    float l = t.posY - target.posY;
                    if (Mathf.Sqrt(o * o + l * l) <= t.conf.collideR + target.conf.collideR + e.conf.spiltR)
                    {
                        hurtByRole(e, target, i);
                    }
                }
            });
        }
    }

    public static void ackMe(
        PetView e,
        float t = 1f,
        float i = 1f,
        int s = 10,
        float o = 0f,
        IList<SheepRoleType> l = null
    )
    {
        float n = i;
        n *= e.conf.atk;

        if (e.curAtkBuff != 0)
        {
            n = Mathf.Floor(n * (1f + e.curAtkBuff / 100f));
        }

        if (l == null)
        {
            l = new SheepRoleType[0];
        }

        (int xn, int yn) block = Util.getXnYn(e.posX, e.posY);
        UtilFind.forfeachBlocksByAckView(e.camp, block.xn, block.yn, s, target =>
        {
            if (!l.Contains(target.conf.roleType) && target.curHp > 0)
            {
                float targetX = e.posX - target.posX;
                float targetY = e.posY - target.posY;
                float distance = Mathf.Sqrt(targetX * targetX + targetY * targetY);
                if (distance <= e.conf.collideR + target.conf.collideR + e.conf.spiltR * t)
                {
                    hurtByRole(e, target, n);
                    if (o != 0f)
                    {
                        targetX /= distance;
                        targetY /= distance;
                        target.impulseX = -targetX * o;
                        target.impulseY = -targetY * o;
                    }
                }
            }
        });
    }

    public static void hitBackMe(PetView e, float t = 1f, int i = 10, float s = 0f)
    {
        (int xn, int yn) block = Util.getXnYn(e.posX, e.posY);
        UtilFind.forfeachBlocksByAckView(e.camp, block.xn, block.yn, i, target =>
        {
            if (target.curHp > 0)
            {
                float o = e.posX - target.posX;
                float l = e.posY - target.posY;
                float n = Mathf.Sqrt(o * o + l * l);
                if (n <= e.conf.collideR + target.conf.collideR + e.conf.spiltR * t && s != 0f)
                {
                    o /= n;
                    l /= n;
                    target.impulseX = -o * s;
                    target.impulseY = -l * s;
                }
            }
        });
    }

    public static void hurtByRole(PetView e, PetView t, float i)
    {
        float s = SheepRoleRestraint.getById(t.conf.roleType).hitRate[(int)e.conf.roleType];
        int damage = Mathf.Max(1, Mathf.FloorToInt(i * s));
        float o = t.subCurHp(damage);
        if (o > 0 && o <= damage)
        {
        }
    }

    public static void hurtByBullet(dynamic e, PetView t, float i)
    {
        float s = SheepRoleRestraint.getById(t.conf.roleType).hitRate[(int)e.conf.roleType];
        int damage = Mathf.Max(1, Mathf.FloorToInt(i * s));
        float o = t.subCurHp(damage);
        if (o > 0 && o <= damage)
        {
        }
    }

    public static bool isCanAckByBullet(dynamic e, PetView petSkin, int i)
    {
        bool s = !petSkin.isDie;
        if (!s)
        {
            return s;
        }

        SheepRoleState o = petSkin.state;
        if (
            petSkin.roleId != 0 &&
            (
                o == SheepRoleState.In ||
                o == SheepRoleState.Dead ||
                o == SheepRoleState.Merge ||
                o == SheepRoleState.Res ||
                o == SheepRoleState.Killer
            )
        )
        {
            return false;
        }

        bool l = petSkin.camp != e.camp;
        if (!l)
        {
            return l;
        }

        if (e.conf.atkShapeType == SheepBulletAtkShapeType.Ring)
        {
            float bulletX = e.x;
            float bulletY = e.y;
            float targetX = petSkin.posX - bulletX;
            float targetY = petSkin.posY - bulletY;
            float distanceSqr = targetX * targetX + targetY * targetY;
            float distance = Mathf.Sqrt(distanceSqr);
            return distance < e.conf.maxRadiuses[i] && distance > e.conf.minRadiuses[i];
        }

        {
            float bulletX = e.x;
            float bulletY = e.y;
            float targetX = petSkin.posX - bulletX;
            float targetY = petSkin.posY - bulletY;
            float distanceSqr = targetX * targetX + targetY * targetY;
            return Mathf.Sqrt(distanceSqr) < e.conf.atkR;
        }
    }
}
}