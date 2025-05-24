using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class CategoryRepository : GenericRepository<category>, ICategoryRepository
    {
        public category GetCategoryByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public List<category> GetCategoryList(int? Page_No, string Search_Data, string Filter_Value, long catId = 0)
        {
            
            int No_Of_Page = (Page_No ?? 1);

            List<category> catList = new List<category>();

            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }

            if (!String.IsNullOrEmpty(Search_Data))
            {
                catList = GetCategories().Where(x => x.name.Contains(Search_Data)).ToList();
            }
            else
            {
                catList = GetCategories().ToList();
            }

            return catList;
        }

        public List<category> GetCategoryByCatID(long catid)
        {
            List<category> finalcat = new List<category>();
            category _cat = _db.categories.Where(x => x.id == catid).FirstOrDefault();
            category cat_1 = new category();
            cat_1.id = _cat.id;
            cat_1.name = _cat.name;
            cat_1.active = _cat.active;
            finalcat.Add(cat_1);

            List<category> categories = _db.categories.Where(x => x.parent_category == catid).ToList();
            foreach (var item in categories)
            {
                category cat = new category();
                cat.id = item.id;
                cat.name = " - " + item.name;
                cat.active = item.active;
                finalcat.Add(cat);
                List<category> categories1 = _db.categories.Where(x => x.parent_category == item.id).ToList();
                foreach (var item1 in categories1)
                {
                    category cat1 = new category();
                    cat1.id = item1.id;
                    cat1.name = " - - " + item1.name;
                    cat1.active = item1.active;
                    finalcat.Add(cat1);
                    List<category> categories2 = _db.categories.Where(x => x.parent_category == item1.id).ToList();
                    foreach (var item2 in categories2)
                    {
                        category cat2 = new category();
                        cat2.id = item2.id;
                        cat2.name = " - - - " + item2.name;
                        cat2.active = item2.active;
                        finalcat.Add(cat2);
                        List<category> categories3 = _db.categories.Where(x => x.parent_category == item2.id).ToList();
                        foreach (var item3 in categories3)
                        {
                            category cat3 = new category();
                            cat3.id = item3.id;
                            cat3.name = " - - - - " + item3.name;
                            cat3.active = item3.active;
                            finalcat.Add(cat3);
                            List<category> categories4 = _db.categories.Where(x => x.parent_category == item3.id).ToList();
                            foreach (var item4 in categories4)
                            {
                                category cat4 = new category();
                                cat4.id = item4.id;
                                cat4.name = " - - - - - " + item4.name;
                                cat4.active = item4.active;
                                finalcat.Add(cat4);
                                List<category> categories5 = _db.categories.Where(x => x.parent_category == item4.id).ToList();
                                foreach (var item5 in categories5)
                                {
                                    category cat5 = new category();
                                    cat5.id = item5.id;
                                    cat5.name = " - - - - - - " + item5.name;
                                    cat5.active = item5.active;
                                    finalcat.Add(cat5);
                                }
                            }
                        }
                    }
                }
            }
            return finalcat;
        }

        // show all categories for drop down list
        // shows also first parent then child
        public List<category> GetCategories()
        {
            List<category> finalcat = new List<category>();
            List<category> categories = _db.categories.Where(x => x.parent_category == null).ToList();
            foreach (var item in categories)
            {
                category cat = new category();
                cat.id = item.id;
                cat.name = item.name;
                cat.active = item.active;
                finalcat.Add(cat);
                List<category> categories1 = _db.categories.Where(x => x.parent_category == item.id).ToList();
                foreach (var item1 in categories1)
                {
                    category cat1 = new category();
                    cat1.id = item1.id;
                    cat1.name = " - " + item1.name;
                    cat1.active = item1.active;
                    finalcat.Add(cat1);
                    List<category> categories2 = _db.categories.Where(x => x.parent_category == item1.id).ToList();
                    foreach (var item2 in categories2)
                    {
                        category cat2 = new category();
                        cat2.id = item2.id;
                        cat2.name = " - - " + item2.name;
                        cat2.active = item2.active;
                        finalcat.Add(cat2);
                        List<category> categories3 = _db.categories.Where(x => x.parent_category == item2.id).ToList();
                        foreach (var item3 in categories3)
                        {
                            category cat3 = new category();
                            cat3.id = item3.id;
                            cat3.name = " - - - " + item3.name;
                            cat3.active = item3.active;
                            finalcat.Add(cat3);
                            List<category> categories4 = _db.categories.Where(x => x.parent_category == item3.id).ToList();
                            foreach (var item4 in categories4)
                            {
                                category cat4 = new category();
                                cat4.id = item4.id;
                                cat4.name = " - - - - " + item4.name;
                                cat4.active = item4.active;
                                finalcat.Add(cat4);
                                List<category> categories5 = _db.categories.Where(x => x.parent_category == item4.id).ToList();
                                foreach (var item5 in categories5)
                                {
                                    category cat5 = new category();
                                    cat5.id = item5.id;
                                    cat5.name = " - - - - - " + item5.name;
                                    cat5.active = item5.active;
                                    finalcat.Add(cat5);
                                }
                            }
                        }
                    }
                }
            }
            return finalcat;
        }
        public object GetSubCategoryDetails(long category)
        {
            var data = _db.categories
                            .Where(cat => cat.parent_category == category && cat.active == true)
                            .Select(cat => new DashboardItems()
                            {
                                id = cat.id,
                                name_ar = cat.name,
                                desc_ar = cat.description,
                                icon = cat.icon,
                                bg_color = cat.bg_color,
                                text_color = cat.text_color,
                                banner_ar = cat.banner
                            })
                            .ToList();

            var news = _db.articles
                                .Where(art => art.category.id == category)
                                .Where(art => art.active == true)
                                .Select(art => new Article()
                                {
                                    id = art.id,
                                    title_ar = art.title,
                                    desc_ar = art.short_description,
                                    image = art.image,
                                    created_at = art.created_at
                                })
                                .ToList();


            return new { data, news };
        }
    }
}