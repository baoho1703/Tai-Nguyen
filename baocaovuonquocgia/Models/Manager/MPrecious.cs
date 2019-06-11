using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models.Manager
{
    public class MPrecious
    {
        public class PreciousAdd
        {
            public string precious_name { get; set; }
            public DateTime? enddate { get; set; }
            public DateTime? startdate { get; set; }
            public DateTime? completedate { get; set; }
            public int id { get; set; }
            public DateTime? slowday { get; set; }
        }

        public ResSubmit ChinhSuaQuy(PreciousAdd preciousEdit)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.id == preciousEdit.id && a.status==false).FirstOrDefault();
            if (_Precious == null)
            {
                resSubmit = new ResSubmit(false, "Không có quý này");
            }
            if(resSubmit.success && tongcuclamnghiep.m_precious.Any(a=>a.id!=preciousEdit.id && a.precious_name == preciousEdit.precious_name))
            {
                resSubmit = new ResSubmit(false, "Tên này đã tồn tại");
            }
            if (resSubmit.success && !((preciousEdit.startdate < preciousEdit.enddate) && preciousEdit.completedate > preciousEdit.enddate))
            {
                resSubmit = new ResSubmit(false, "Ngày bắt đầu, kết thúc, hoàn thành nhập không hợp lệ");
            }
            if(resSubmit.success && !(preciousEdit.completedate > DateTime.Now))
            {
                resSubmit = new ResSubmit(false, "Ngày hoàn thành không phù hợp");
            }
            if (resSubmit.success && !(preciousEdit.slowday< preciousEdit.completedate && preciousEdit.slowday> preciousEdit.enddate))
            {
                resSubmit = new ResSubmit(false, "Ngày báo chậm không phù hợp");
            }
            if (resSubmit.success)
            {
                _Precious.completedate = preciousEdit.completedate;
                _Precious.enddate = preciousEdit.enddate;
                _Precious.precious_name = preciousEdit.precious_name;
                _Precious.startdate = preciousEdit.startdate;
                _Precious.slowday = preciousEdit.slowday;
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }
            }
            return resSubmit;
        }

        public ResSubmit ThemMoiQuy(PreciousAdd preciousAdd, AccountCookie accountCookie)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Thêm mới thành công");
            if (tongcuclamnghiep.m_precious.Any(a => a.precious_name == preciousAdd.precious_name))
            {
                resSubmit = new ResSubmit(false, "Tên đã tồn tại");
            }
            //if(resSubmit.success && tongcuclamnghiep.m_precious.Any(a => a.status == false))
            //{
            //    resSubmit = new ResSubmit(false, "Chưa kết thúc kỳ báo cáo trước");
            //}
            if(resSubmit.success && !( (preciousAdd.startdate< preciousAdd.enddate) && preciousAdd.completedate> preciousAdd.enddate))
            {
                resSubmit = new ResSubmit(false, "Ngày bắt đầu, kết thúc, hoàn thành nhập không hợp lệ");
            }
            if (resSubmit.success && !(preciousAdd.completedate > DateTime.Now))
            {
                resSubmit = new ResSubmit(false, "Ngày hoàn thành không phù hợp");
            }
            if (resSubmit.success && !(preciousAdd.slowday < preciousAdd.completedate && preciousAdd.slowday > preciousAdd.enddate))
            {
                resSubmit = new ResSubmit(false, "Ngày báo chậm không phù hợp");
            }
            if (resSubmit.success)
            {
                m_precious _Precious = new m_precious();
                _Precious.account_id = accountCookie.Id;
                _Precious.completedate = preciousAdd.completedate;
                _Precious.enddate = preciousAdd.enddate;
                _Precious.startdate = preciousAdd.startdate;
                _Precious.precious_name = preciousAdd.precious_name;
                _Precious.slowday = preciousAdd.slowday;
                _Precious.status = false;
                tongcuclamnghiep.m_precious.Add(_Precious);
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Thêm mới thất bại");
                }
                else
                {
                    tongcuclamnghiep.m_precious.Where(a => a.id != _Precious.id && a.status == false).ToList().All(a=> {
                        a.status = true;
                        return true;
                    });
                    tongcuclamnghiep.SaveChanges();
                }
            }
            return resSubmit;
        }

        public ResTableData GetPreciousTable(int start, int length, int draw, string search)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var dataAll = tongcuclamnghiep.m_precious.Where(a => (search.Length > 0 ? (a.precious_name.Contains(search)) : true));
            var data = dataAll.ToList().Skip((start)).Take(length).Select(a => new
            {
                a.id,
                a.precious_name,
                startdate = a.startdate.Value.ToString("dd/MM/yyyy"),
                enddate = a.enddate.Value.ToString("dd/MM/yyyy"),
                createdate = a.createdate.Value.ToString("dd/MM/yyyy"),
                completedate = a.completedate.Value.ToString("dd/MM/yyyy"),
                a.status
            });
            return new ResTableData()
            {
                data = data,
                draw = (draw + 1),
                recordsFiltered = dataAll.Count(),
                recordsTotal = dataAll.Count()
            };
        }
    }
}