using System;
using System.Collections.Generic;
using UnityEngine;

namespace rvb.scripts {
    
      
    public enum BuffID {
        GeneralOrder = 0,
        CardBuff = 1
    }
    
    public enum t {
        Single_Refresh = 0,
        Single_Extend = 1,
        Single_Independ = 2,
        Multi_Refresh = 3,
        Multi_Extend = 4,
        Multi_Independ = 5
    }

    public class i {
        public object id;
        public t type;
        public double duration;
        public double timer;
        public Action<i> onEndCall;
        public object arg;

        public i() {
            id = null;
            type = default;
            duration = default;
            timer = default;
            onEndCall = null;
            arg = null;
        }
    }

    public class BuffTimeAttacher {
        public Dictionary<object, i> attachingBuffMap;
        public List<i> independBuffs;
        public List<i> waitRemovedPacks;

        public BuffTimeAttacher() {
            attachingBuffMap = new Dictionary<object, i>();
            independBuffs = new List<i>();
            waitRemovedPacks = new List<i>();
        }

        public void addSingleRefresh(
            object n,
            double e,
            Action<i> d,
            Action<i> a) {
            i r;

            if (attachingBuffMap.TryGetValue(n, out r)) {
                if (r.type != t.Single_Refresh) {
                    Debug.LogError(
                        "Buff type mismatch: " + n +
                        " is not Single_Refresh"
                    );

                    return;
                }

                r.timer = r.duration = e;
                r.onEndCall = a;
                return;
            }

            i f = new i();

            f.id = n;
            f.type = t.Single_Refresh;
            f.duration = f.timer = e;
            f.onEndCall = a;

            if (d != null) {
                d(f);
            }

            attachingBuffMap.Add(n, f);
        }

        public void addSingleExtend(
            object n,
            double e,
            Action<i> d,
            Action<i> a,
            bool r = false) {
            i f;

            if (attachingBuffMap.TryGetValue(n, out f)) {
                if (f.type != t.Single_Extend) {
                    Debug.LogError(
                        "Buff type mismatch: " + n +
                        " is not Single_Extend"
                    );

                    return;
                }

                f.duration += e;
                f.timer += e;
                f.onEndCall = a;

                if (r) {
                    if (d != null) {
                        d(f);
                    }
                }

                return;
            }

            i l = new i();

            l.id = n;
            l.type = t.Single_Extend;
            l.duration = l.timer = e;
            l.onEndCall = a;

            if (d != null) {
                d(l);
            }

            attachingBuffMap.Add(n, l);
        }

        public void addIndependBuff(
            object n,
            double e,
            Action<i> d,
            Action<i> a) {
            i r = new i();

            r.id = n;
            r.type = t.Single_Independ;
            r.duration = r.timer = e;
            r.onEndCall = a;

            if (d != null) {
                d(r);
            }

            independBuffs.Add(r);
        }

        public void updateTimer(double n) {
            foreach (KeyValuePair<object, i> pair in attachingBuffMap) {
                i value = pair.Value;

                value.timer -= n;

                if (value.timer <= 0) {
                    if (value.onEndCall != null) {
                        value.onEndCall(value);
                    }

                    waitRemovedPacks.Add(value);
                }
            }

            foreach (i value in waitRemovedPacks) {
                attachingBuffMap.Remove(value.id);
            }

            waitRemovedPacks = new List<i>();

            for (int t = independBuffs.Count - 1; t >= 0; t--) {
                i value = independBuffs[t];

                value.timer -= n;

                if (value.timer <= 0) {
                    if (value.onEndCall != null) {
                        value.onEndCall(value);
                    }

                    independBuffs.RemoveAt(t);
                }
            }
        }

        public void clear(bool n = true) {
            if (n) {
                foreach (KeyValuePair<object, i> pair in attachingBuffMap) {
                    i value = pair.Value;

                    if (value.onEndCall != null) {
                        value.onEndCall(value);
                    }
                }

                foreach (i value in independBuffs) {
                    if (value.onEndCall != null) {
                        value.onEndCall(value);
                    }
                }
            }

            attachingBuffMap.Clear();
            independBuffs = new List<i>();
        }
    }
}