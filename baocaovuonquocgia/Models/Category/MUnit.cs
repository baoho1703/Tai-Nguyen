using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models.Category
{
    public class MUnit
    {
        public class UnitAdd : m_unit
        {
            public string UnitVersionText { get; set; }
        }

        public ResSubmit ChinhSuaUnit(UnitAdd unitEdit)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.id == unitEdit.id).FirstOrDefault();
            if (_Unit == null)
            {
                resSubmit = new ResSubmit(false, "Không tồn tại đơn vị này");
            }
            if (resSubmit.success && !(unitEdit.unitname.Length >= 10 && unitEdit.unitname.Length <= 4000))
            {
                resSubmit = new ResSubmit(false, "Tên đơn vị không hợp lệ");
            }
            if(resSubmit.success && tongcuclamnghiep.m_unit.Any(a=>a.id!=unitEdit.id && a.unitname == unitEdit.unitname))
            {
                resSubmit = new ResSubmit(false, "Tên đơn vị đã tồn tại");
            }
            if (resSubmit.success)
            {
                _Unit.address = unitEdit.address;
                _Unit.status = unitEdit.status;
                _Unit.unitname = unitEdit.unitname;
                _Unit.dientich1 = (unitEdit.dientich1 != null ? unitEdit.dientich1 : 0);
                _Unit.dientich2 = (unitEdit.dientich2 != null ? unitEdit.dientich2 : 0);
                _Unit.ngansachnhannuoc1 = (unitEdit.ngansachnhannuoc1 != null ? unitEdit.ngansachnhannuoc1 : 0);
                _Unit.ngansachnhanuoc2 = (unitEdit.ngansachnhanuoc2 != null ? unitEdit.ngansachnhanuoc2 : 0);
                _Unit.ngaythanhlap = unitEdit.ngaythanhlap;
                _Unit.tongcanbo1 = (unitEdit.tongcanbo1 != null ? unitEdit.tongcanbo1 : 0);
                _Unit.tongcanbo2 = (unitEdit.tongcanbo2 != null ? unitEdit.tongcanbo2 : 0);
                tongcuclamnghiep.SaveChanges();
                int version = 1;
                var _UnitVS = tongcuclamnghiep.m_unitversion.Where(a => a.unit_id == _Unit.id && a.status == true).FirstOrDefault();
                if (_UnitVS != null)
                {
                    version = _UnitVS.version.Value + 1;
                }
                m_unitversion _Unitversion = new m_unitversion();
                _Unitversion.createdate = DateTime.Now;
                _Unitversion.datatext = unitEdit.UnitVersionText;
                _Unitversion.unit_id = _Unit.id;
                _Unitversion.status = true;
                _Unitversion.version = version;
                _Unitversion.dientich1 = _Unit.dientich1;
                _Unitversion.dientich2 = _Unit.dientich2;
                _Unitversion.ngansachnhannuoc1 = _Unit.ngansachnhannuoc1;
                _Unitversion.ngansachnhanuoc2 = _Unit.ngansachnhanuoc2;
                _Unitversion.ngaythanhlap = _Unit.ngaythanhlap;
                _Unitversion.tongcanbo1 = _Unit.tongcanbo1;
                _Unitversion.tongcanbo2 = _Unit.tongcanbo2;
                tongcuclamnghiep.m_unitversion.Add(_Unitversion);
                if (tongcuclamnghiep.SaveChanges() == 1)
                {
                    tongcuclamnghiep.m_unitversion.Where(a => a.unit_id == _Unit.id && a.status == true && a.id != _Unitversion.id).ToList().All(a => {
                        a.status = false;
                        return true;
                    });
                    tongcuclamnghiep.SaveChanges();
                }
            }
            return resSubmit;
        }

        public ResSubmit ThemMoiUnit(UnitAdd unitAdd)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Thêm mới thành công");
            if(!(unitAdd.unitname.Length>=10 && unitAdd.unitname.Length <= 4000))
            {
                resSubmit = new ResSubmit(false, "Tên đơn vị không hợp lệ");
            }
            if (resSubmit.success && tongcuclamnghiep.m_unit.Any(a => a.unitname == unitAdd.unitname))
            {
                resSubmit = new ResSubmit(false, "Tên đơn vị đã tồn tại");
            }
            if(resSubmit.success && (unitAdd.account_id != 0 && unitAdd.account_id != null) && !tongcuclamnghiep.m_account.Where(a=> a.role_id==2 && a.id== unitAdd.account_id && a.status==true).ToList().Any(a=> !tongcuclamnghiep.m_unit.Any(c => c.account_id == a.id)))
            {
                resSubmit = new ResSubmit(false, "Tài khoản  quản lý không hợp lệ");
            }
            if (resSubmit.success)
            {
                m_unit _Unit = new m_unit();
                _Unit.account_id = ((unitAdd.account_id != 0 && unitAdd.account_id != null) ? unitAdd.account_id : null);
                _Unit.address = unitAdd.address;
                _Unit.status = true;
                _Unit.unitname = unitAdd.unitname;

                _Unit.dientich1 = (unitAdd.dientich1!=null ? unitAdd.dientich1 : 0);
                _Unit.dientich2 = (unitAdd.dientich2 != null ? unitAdd.dientich2 : 0);
                _Unit.ngansachnhannuoc1 = (unitAdd.ngansachnhannuoc1 != null ? unitAdd.ngansachnhannuoc1 : 0);
                _Unit.ngansachnhanuoc2 = (unitAdd.ngansachnhanuoc2 != null ? unitAdd.ngansachnhanuoc2 : 0);
                _Unit.ngaythanhlap = unitAdd.ngaythanhlap;
                _Unit.tongcanbo1 = (unitAdd.tongcanbo1 != null ? unitAdd.tongcanbo1 : 0);
                _Unit.tongcanbo2 = (unitAdd.tongcanbo2 != null ? unitAdd.tongcanbo2 : 0);
                tongcuclamnghiep.m_unit.Add(_Unit);
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Thêm mới thất bại");
                }
                else
                {
                    m_unitversion _Unitversion = new m_unitversion();
                    _Unitversion.createdate = DateTime.Now;
                    _Unitversion.datatext = unitAdd.UnitVersionText;
                    _Unitversion.unit_id = _Unit.id;
                    _Unitversion.status = true;
                    _Unitversion.version = 1;
                    _Unitversion.dientich1 = _Unit.dientich1;
                    _Unitversion.dientich2 = _Unit.dientich2;
                    _Unitversion.ngansachnhannuoc1 = _Unit.ngansachnhannuoc1;
                    _Unitversion.ngansachnhanuoc2 = _Unit.ngansachnhanuoc2;
                    _Unitversion.ngaythanhlap = _Unit.ngaythanhlap;
                    _Unitversion.tongcanbo1 = _Unit.tongcanbo1;
                    _Unitversion.tongcanbo2 = _Unit.tongcanbo2;
                    tongcuclamnghiep.m_unitversion.Add(_Unitversion);
                    tongcuclamnghiep.SaveChanges();
                }
            }

            return resSubmit;
        }

        public ResTableData GetUnitTable(int start, int length, int draw, string search)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var dataAll = tongcuclamnghiep.m_unit.Where(a => (search.Length > 0 ? (a.unitname.Contains(search) || a.address.Contains(search)) : true));
            var data = dataAll.ToList().Skip((start)).Take(length).Select(a => new
            {
                a.id,
                a.unitname,
                createdate = a.createdate.Value.ToString("dd/MM/yyyy"),
                account = tongcuclamnghiep.m_account.Where(b => (b.id == a.account_id) || (b.role_id == 2 && b.status == true && !tongcuclamnghiep.m_unit.Any(c => c.account_id == b.id))).Select(b => new { b.id, b.fullname ,selected = (a.account_id!=null ? (bool?)(b.id == a.account_id) : false )}),
                a.status,
                a.address
            });
            return new ResTableData()
            {
                data = data,
                draw = (draw + 1),
                recordsFiltered = dataAll.Count(),
                recordsTotal = dataAll.Count()
            };
        }

        public ResSubmit CapNhatNguoiQuanLy(int UnitId, int AccountId, AccountCookie accountCookie)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Account = tongcuclamnghiep.m_account.Where(a => a.id == AccountId && a.status == true && !tongcuclamnghiep.m_unit.Any(b => b.account_id == a.id) && a.role_id == 2).FirstOrDefault();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.id == UnitId).FirstOrDefault();
            if (accountCookie.Role_Id != 1)
            {
                resSubmit = new ResSubmit(false, "Bạn không có quyền");
            }
            if (resSubmit.success && _Account == null && (AccountId != 0))
            {
                resSubmit = new ResSubmit(false, "Tài khoản không hợp lệ");
            }
            if(resSubmit.success && _Unit == null)
            {
                resSubmit = new ResSubmit(false, "Không tồn tại đơn vị này");
            }
            if (resSubmit.success)
            {
                if (AccountId == 0)
                {
                    _Unit.account_id = null;
                }
                else
                {
                    _Unit.account_id = AccountId;
                }
                if (tongcuclamnghiep.SaveChanges() != 1)
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }
            }
            return resSubmit;
        }
    }
}