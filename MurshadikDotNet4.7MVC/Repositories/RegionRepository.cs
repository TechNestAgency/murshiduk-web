using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class RegionRepository : GenericRepository<region>, IRegionRepository
    {
        public region GetRegionByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public IQueryable<region> GetAllRegions(int? Page_No, string Search_Data, string Filter_Value)
        {
            int No_Of_Page = (Page_No ?? 1);

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            var regions = from r in _db.regions
                          select r;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                regions = regions.Where(s => s.name.Contains(Search_Data)
                                       || s.name_ar.Contains(Search_Data));
            }

            return regions.OrderByDescending(x => x.id);
        }

        public void CreateRegion(region r)
        {
            Insert(r);
          
        }

        public void UpdateRegion(region r)
        {
            Update(r);
          
        }

        public void DeleteRegion(region r)
        {
            _db.regions.Remove(r);
            _db.SaveChanges();
            
        }

        public Data GetAllRegions()
        {
            Data d = new Data();

            var regions = _db.regions.Where(x => x.active == true).Select(p => new { p.id, p.name_ar, p.name }).ToList();
            d.status = true;
            d.message = "successfully fetch the records";
            d.data = regions;
            return d;
        }

        public Object GetRegionData()
        {
            var regions = _db.regions.Include("city").Where(x => x.active == true)
                        .ToList();

            SkillItem skills = new SkillItem()
            {
                items = _db.skills.Where(x => x.parent_skill == null).Select(p => new Skills()
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
            return new { regions, skills = skills.items };
        }

    }
}