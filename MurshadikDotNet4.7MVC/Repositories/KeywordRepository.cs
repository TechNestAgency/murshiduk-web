using MurshadikCP.Controllers;
using MurshadikCP.Models;
using MurshadikCP.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MurshadikCP.Repositories
{
    public class KeywordRepository : GenericRepository<keyword>, IKeywordRepository
    {
        public keyword GetKeywordByID(long id)
        {
            return table.Where(u => u.id == id).SingleOrDefault();
        }

        public Data GetKeyword()
        {
            Data d = new Data();
            var keywords = _db.keywords.Select(p => new { p.id, p.keyword1 }).ToList();

            d.status = true;
            d.message = "successfully fetch the records";
            d.data = keywords;
            return d;
        }

        public Data AddKeyword(string word)
        {
            Data d = new Data();

            keyword k = new keyword();
            k.created_at = DateTime.Now;
            k.keyword1 = word;
            Insert(k);
            Save();
            d.status = true;
            d.message = "successfully inserted!";
            d.data = new { id = k.id, name = k.keyword1 };
            return d;
        }

        public object AutoComplete(string word)
        {
            var k = GetAll().Where(x => x.keyword1.Contains(word)).Select(x => new { name = x.keyword1, id = x.id }).ToList();
            return k;
        }

        public object RandomKeyword()
        {
            var kws = GetAll().OrderByDescending(r => r.keyword_count).Take(10).Select(q => new { q.id, name = q.keyword1 });
            return kws;
        }
    }
}