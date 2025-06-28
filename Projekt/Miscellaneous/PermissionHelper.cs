using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Provider;

namespace Projekt.Miscellaneous
{
    public static class PermissionHelper
    {
        public const int Blocked = 0;
        
        public const int CanSeeOwnProfile = 1;
        public const int CanSeeOwnSchedule = 1 << 1;
        public const int CanSeeOtherProfiles = 1 << 2;
        public const int CanSeeOtherSchedules = 1 << 3;
        public const int CanEditOwnProfile = 1 << 4;
        public const int CanEditOwnSchedule = 1 << 5;
        public const int CanEditOtherProfiles = 1 << 6;
        public const int CanEditOtherSchedules = 1 << 7;
        public const int CanCreateClasses = 1 << 8;
        public const int CanManageGroups = 1 << 9;
        public const int CanModifyData = 1 << 10; // wydziały, adresy, etc.
        public const int CanChangePermissions = 1 << 11;
        public const int CanManageUsers = 1 << 12; // dodawanie, usuwanie, edytowanie użytkowników

        public const int God = ~0;

        public static bool CheckPermissions(int has, params int[] shouldHave)
        {
            int shouldHaveCombined = 0;
            foreach(var current in shouldHave)
                shouldHaveCombined |= current;
           
            return (has & shouldHaveCombined) == shouldHaveCombined;
        }

        public static int CombinePermissions(params int[] permissions)
        {
            int combined = 0;
            foreach(var permission in permissions)
                combined |= permission;
            return combined;
        }
    }
}
