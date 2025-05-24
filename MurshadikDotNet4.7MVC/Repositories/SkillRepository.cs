using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class SkillRepository : GenericRepository<skill>, ISkillRepository
    {
        public skill GetSkillByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public object GetUserSkills(long id)
        {
            var query =
               from su in _db.skill_user
               join s in _db.skills on su.skill_id equals s.id
               where su.user_id == id
               select new { s.id, s.name, su.is_approved };
            return query.ToList();
        }

        public string GetAllSkillsByConsultant(long id)
        {
            string Allskills = "";
            var skills = _db.skill_user.Include("skill").Where(x => x.user_id == id).ToList().Select(p => p.skill.name);

            if (skills.Count() > 0)
            {
                Allskills = string.Join(",", skills);
            }
            return Allskills;
        }

        public string GetSkillsByConsultant(long id)
        {
            string Allskills = "";
            var skills = _db.fun_GetConsultantSkills(id).ToList();
            if (skills.Count() > 0)
            {
                Allskills = string.Join(",", skills);
            }
            return Allskills;
        }

        public object GetSkills()
        {
            SkillItem skills = new SkillItem()
            {
                items = _db.skills.Where(x => x.parent_skill == null).OrderBy(x => x.menu_sort).Select(p => new Skills()
                {
                    id = p.id,
                    name_ar = p.name,
                    menu_sort = p.menu_sort,
                    description = p.description,
                    icon = p.icon,
                    banner = p.banner,
                    text_color = p.text_color,
                    bg_color = p.bg_color
                }).ToList()

            };

            for (int i = 0; i < skills.items.Count(); i++)
            {
                if (_db.skills.Where(x => x.parent_skill == skills.items[i].id) != null)
                {
                    skills.items[i].children = new List<Skills>();
                    int p_id = Convert.ToInt16(skills.items[i].id);
                    skills.items[i].children.AddRange(_db.skills.Where(x => x.parent_skill == p_id).Select(q => new Skills()
                    {
                        id = q.id,
                        name_ar = q.name,
                        menu_sort = 0,
                        description = q.description,
                        icon = q.icon,
                        banner = q.banner,
                        text_color = q.text_color,
                        bg_color = q.bg_color
                    }).ToList());
                }
            }

            return skills.items;
        }

        public Data InsertUserSkill(long userid, long skill_id)
        {
            Data d = new Data();
            List<skill_user> list = new List<skill_user>();
            skill_user su = new skill_user();
            su.user_id = userid;
            su.skill_id = skill_id;
            su.created_at = DateTime.Now;
            _db.skill_user.Add(su);
            _db.SaveChanges();

            d.status = true;
            d.message = "successfully inserted!";
            d.data = null;
            return d;
        }

        public List<SkillUser> GetUserApprovedSkills(long uid)
        {
            List<skill_user> list = new List<skill_user>();
            List<SkillUser> su = _db.skill_user.Where(x => x.user_id == uid).Select(p => new SkillUser
            {
                skill_id = p.skill_id,
                name = p.skill.name,
                is_approved = p.is_approved
            }).ToList();

            return su;
        }
    }
}