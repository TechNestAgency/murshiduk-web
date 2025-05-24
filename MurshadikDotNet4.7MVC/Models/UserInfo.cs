using MurshadikCP.Models.DB;
using MurshadikCP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Models
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public long labid { get; set; }
        public long marketid { get; set; }
        public string multiplemarketids { get; set; }
        public long region_id { get; set; }

        public List<roles_permission> roles_Permissions { get; set; }

        public bool canView(long page_id)
        {

            if (this.roles_Permissions.Any())
            {
                return this.roles_Permissions.Where(p => p.page_id == page_id).Select(p => p.can_view).FirstOrDefault();
            }

            return false;
        }

        public bool canUpdate(long page_id)
        {

            if (this.roles_Permissions.Any())
            {
                return this.roles_Permissions.Where(p => p.page_id == page_id).Select(p => p.can_update).FirstOrDefault();
            }

            return false;
        }

        public bool canDelete(long page_id)
        {
            if (this.roles_Permissions.Any())
            {
                return this.roles_Permissions.Where(p => p.page_id == page_id).Select(p => p.can_delete).FirstOrDefault();
            }

            return false;
        }

        public bool canInsert(long page_id)
        {

            if (this.roles_Permissions.Any())
            {
                return this.roles_Permissions.Where(p => p.page_id == page_id).Select(p => p.can_insert).FirstOrDefault();
            }

            return false;
        }
        public bool HasAdminAccess
        {
            get
            {
                return new long[] { (long)Role.SuperAdmin, (long)Role.Manager }.Contains(this.RoleId);
            }
        }
    }



    public class AppUser
    {
        private user user;
        public AppUser(long uid)
        {
            UserRepository ur = new UserRepository();
            this.user = ur.GetUserByID(uid) ?? null;
            this.UserId = uid;
            //this.Valid = 
        }
        
        public user dbUser
        {
            get
            {
                return this.user;
            }
        }
        public string Name { get { return this.dbUser != null ? this.dbUser.name : null; } }
        public string FullName {
            get {

                return string.Format("{0} {1}", this.Name, this.LastName).Trim();
            }
        }
        public string LastName { get { return this.dbUser != null ? this.dbUser.last_name : null; } }
        public long RoleId { get { return this.dbUser != null ? this.dbUser.role_id : 0; } }
        public string Valid { get; set; }
        public long UserId { get; set; }
       

        
        public bool IsAuthoizedAppUser
        {
            get
            {
                return this.RoleId == (long) UserType.Consultant || this.RoleId == (long) UserType.Farmer; 
            }
        }

    }
}