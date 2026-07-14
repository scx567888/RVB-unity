using System;
using System.Collections.Generic;

namespace rvb.scripts {
    public class SheepRoleRestraint {
        public int id;
        public float[] hitRate;

        public static SheepRoleRestraint getById(int id) {
            return SheepRoleRestraints.getById(id);
        }
    }

    public static class SheepRoleRestraints {
        public static readonly SheepRoleRestraint restraint_0 = new() {
            id = 0,
            hitRate = new[] { 1f, 8f, 0.02f, 0.02f, 0.5f, 0.5f, 0.5f, 0.08f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_1 = new() {
            id = 1,
            hitRate = new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_2 = new() {
            id = 2,
            hitRate = new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_3 = new() {
            id = 3,
            hitRate = new[] { 1f, 1f, 0.5f, 1f, 1f, 1f, 1.85f, 1f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_4 = new() {
            id = 4,
            hitRate = new[] { 1f, 1f, 0.4f, 2f, 1f, 4f, 1f, 1f, 1.5f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_5 = new() {
            id = 5,
            hitRate = new[] { 1f, 1f, 0.4f, 1f, 1f, 0.4f, 2f, 2f, 1.75f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_6 = new() {
            id = 6,
            hitRate = new[] { 1f, 1f, 0.15f, 0.4f, 1f, 0.8f, 1.2f, 0.25f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_7 = new() {
            id = 7,
            hitRate = new[] { 1f, 1f, 0.025f, 1f, 1f, 2f, 2f, 1f, 0.8f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_8 = new() {
            id = 8,
            hitRate = new[] { 1f, 1f, 0.8f, 0.2f, 1.8f, 1f, 1f, 0.2f, 0.8f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_9 = new() {
            id = 9,
            hitRate = new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_10 = new() {
            id = 10,
            hitRate = new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint restraint_11 = new() {
            id = 11,
            hitRate = new[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }
        };

        public static readonly SheepRoleRestraint[] LIST = {
            restraint_0,
            restraint_1,
            restraint_2,
            restraint_3,
            restraint_4,
            restraint_5,
            restraint_6,
            restraint_7,
            restraint_8,
            restraint_9,
            restraint_10,
            restraint_11,
        };

        private static readonly Dictionary<int, SheepRoleRestraint> MAP = buildMap(LIST);

        public static SheepRoleRestraint getById(int id) {
            return MAP[id];
        }

        private static Dictionary<int, SheepRoleRestraint> buildMap(SheepRoleRestraint[] list) {
            var map = new Dictionary<int, SheepRoleRestraint>();

            foreach (var e in list) {
                map.Add(e.id, e);
            }

            return map;
        }
    }
}