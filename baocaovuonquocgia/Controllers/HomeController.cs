using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baocaovuonquocgia.Models;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Controllers
{
    public class HomeController : Controller
    {

        [CheckSession]
        public ActionResult Index()
        {
            return View();
        }

        [CheckSessionAjax]
        public JsonResult GetThongTinUnit()
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id).FirstOrDefault();
            if (_Unit != null)
            {
                var Vesion = tongcuclamnghiep.m_unitversion.Where(a => a.status == true && a.unit_id==_Unit.id).FirstOrDefault();
                if (Vesion != null)
                {
                    return Json(new
                    {
                        _Unit.address,
                        _Unit.dientich1,
                        _Unit.dientich2,
                        _Unit.ngansachnhannuoc1,
                        _Unit.ngansachnhanuoc2,
                        _Unit.ngaythanhlap,
                        _Unit.tongcanbo1,
                        _Unit.tongcanbo2,
                        _Unit.unitname,
                        Vesion = new
                        {
                            Vesion.datatext
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        _Unit.address,
                        _Unit.dientich1,
                        _Unit.dientich2,
                        _Unit.ngansachnhannuoc1,
                        _Unit.ngansachnhanuoc2,
                        _Unit.ngaythanhlap,
                        _Unit.tongcanbo1,
                        _Unit.tongcanbo2,
                        _Unit.unitname
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    _Unit.address,
                    _Unit.dientich1,
                    _Unit.dientich2,
                    _Unit.ngansachnhannuoc1,
                    _Unit.ngansachnhanuoc2,
                    _Unit.ngaythanhlap,
                    _Unit.tongcanbo1,
                    _Unit.tongcanbo2,
                    _Unit.unitname
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public class TienDo
        {
            public string QuyBaoCao { get; set; }
            public List<DonViTienDo> SoDonViDungTienDo { get; set; }
            public List<DonViTienDo> SoDonViChamTienDo { get; set; }
            public List<DonViTienDo> SoDonViKhongBaoCao { get; set; }
        }

        public class DonViTienDo
        {
            public int Id { get; set; }
            public string UnitName { get; set; }
        }

        [CheckSessionAjax]
        public JsonResult GetTienDo()
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            List<string> labels = new List<string>();
            List<DonViTienDo> DanhSachDonVi = new List<DonViTienDo>();
            List<int> DungTienDo = new List<int>();
            List<int> ChamTienDo = new List<int>();
            List<int> KhongBaoCao = new List<int>();

            List<TienDo> tienDos = new List<TienDo>();
            DanhSachDonVi = tongcuclamnghiep.m_unit.Where(a => a.status == true).Select(a => new DonViTienDo() { Id=a.id, UnitName=a.unitname }).ToList();
            tongcuclamnghiep.m_precious.Where(a=>a.enddate<DateTime.Now).ToList().ForEach(a=> {
                //labels.Add(a.precious_name);
                //int intDungTienDo = a.m_unit_precious.Where(b => b.completedate < a.slowday).Count();
                //DungTienDo.Add(intDungTienDo);
                //int intChamTienDo= a.m_unit_precious.Where(b => b.completedate >= a.slowday && b.completedate<=a.completedate).Count();
                //ChamTienDo.Add(intChamTienDo);
                //int intKhongBaoCao = a.m_unit_precious.Where(b => b.completedate == null).Count();
                //intKhongBaoCao += tongcuclamnghiep.m_unit.Where(b => b.status == true).ToList().Where(b => !a.m_unit_precious.ToList().Select(c => c.unit_id).Contains(b.id)).Count();
                //KhongBaoCao.Add(intKhongBaoCao);

                TienDo tienDo = new TienDo();
                tienDo.QuyBaoCao = a.precious_name;
                tienDo.SoDonViDungTienDo = a.m_unit_precious.Where(b => b.completedate < a.slowday).Select(b => new DonViTienDo() { Id=b.m_unit.id, UnitName= b.m_unit.unitname }).ToList();
                tienDo.SoDonViChamTienDo= a.m_unit_precious.Where(b => b.completedate >= a.slowday && b.completedate <= a.completedate).Select(b => new DonViTienDo() { Id = b.m_unit.id, UnitName = b.m_unit.unitname }).ToList();
                List<DonViTienDo> SoDonViKhongBaoCao = a.m_unit_precious.Where(b => b.completedate == null).Select(b => new DonViTienDo() { Id = b.m_unit.id, UnitName = b.m_unit.unitname }).ToList();
                tongcuclamnghiep.m_unit.Where(b => b.status == true).ToList().Where(b => !a.m_unit_precious.ToList().Select(c => c.unit_id).Contains(b.id)).ToList().ForEach(b=> {
                    SoDonViKhongBaoCao.Add(new DonViTienDo() { Id = b.id, UnitName = b.unitname });
                });
                tienDo.SoDonViKhongBaoCao = SoDonViKhongBaoCao;

                labels.Add(a.precious_name);
                DungTienDo.Add(tienDo.SoDonViDungTienDo.Count());
                ChamTienDo.Add(tienDo.SoDonViChamTienDo.Count());
                KhongBaoCao.Add(tienDo.SoDonViKhongBaoCao.Count());
                tienDos.Add(tienDo);
            });
            List<List<int>> series = new List<List<int>>() { DungTienDo, ChamTienDo, KhongBaoCao };
            int totalUnit = tongcuclamnghiep.m_unit.Where(a => a.status == true).Count();
            return Json(new { labels = labels , series= series , totalUnit= totalUnit, tienDos= tienDos , DanhSachDonVi = DanhSachDonVi }, JsonRequestBehavior.AllowGet);
        }


        [CheckSessionAjax]
        public JsonResult GetThongTinUnitByID(int IdUnit)
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.id == IdUnit).FirstOrDefault();
            if (accountCookie.Role_Id==1 && _Unit != null)
            {
                var Vesion = tongcuclamnghiep.m_unitversion.Where(a => a.status == true && a.unit_id == _Unit.id).FirstOrDefault();
                if (Vesion != null)
                {
                    return Json(new
                    {
                        _Unit.address,
                        _Unit.dientich1,
                        _Unit.dientich2,
                        _Unit.ngansachnhannuoc1,
                        _Unit.ngansachnhanuoc2,
                        _Unit.ngaythanhlap,
                        _Unit.tongcanbo1,
                        _Unit.tongcanbo2,
                        _Unit.unitname,
                        Vesion = new
                        {
                            Vesion.datatext
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        _Unit.address,
                        _Unit.dientich1,
                        _Unit.dientich2,
                        _Unit.ngansachnhannuoc1,
                        _Unit.ngansachnhanuoc2,
                        _Unit.ngaythanhlap,
                        _Unit.tongcanbo1,
                        _Unit.tongcanbo2,
                        _Unit.unitname
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new
                {
                    _Unit.address,
                    _Unit.dientich1,
                    _Unit.dientich2,
                    _Unit.ngansachnhannuoc1,
                    _Unit.ngansachnhanuoc2,
                    _Unit.ngaythanhlap,
                    _Unit.tongcanbo1,
                    _Unit.tongcanbo2,
                    _Unit.unitname
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSessionAjax]
        public JsonResult ChinhSuaUnit(Models.Category.MUnit.UnitAdd unitEdit)
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.id == unitEdit.id && a.account_id== accountCookie.Id).FirstOrDefault();
            if (_Unit == null)
            {
                resSubmit = new ResSubmit(false, "Không tồn tại đơn vị này");
            }
            if (resSubmit.success && !(unitEdit.unitname.Length >= 10 && unitEdit.unitname.Length <= 4000))
            {
                resSubmit = new ResSubmit(false, "Tên đơn vị không hợp lệ");
            }
            if (resSubmit.success && tongcuclamnghiep.m_unit.Any(a => a.id != unitEdit.id && a.unitname == unitEdit.unitname))
            {
                resSubmit = new ResSubmit(false, "Tên đơn vị đã tồn tại");
            }
            if (resSubmit.success)
            {
                HttpRequestBase request = HttpContext.Request;
                string UnitVersionText = request.Unvalidated.Form.Get("UnitVersionTextData");
                unitEdit.UnitVersionText = UnitVersionText;
                _Unit.address = unitEdit.address;
                _Unit.unitname = unitEdit.unitname;
                _Unit.dientich1 = unitEdit.dientich1;
                _Unit.dientich2 = unitEdit.dientich2;
                _Unit.ngansachnhannuoc1 = unitEdit.ngansachnhannuoc1;
                _Unit.ngansachnhanuoc2 = unitEdit.ngansachnhanuoc2;
                _Unit.ngaythanhlap = unitEdit.ngaythanhlap;
                _Unit.tongcanbo1 = unitEdit.tongcanbo1;
                _Unit.tongcanbo2 = unitEdit.tongcanbo2;
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
                _Unitversion.dientich1 = unitEdit.dientich1;
                _Unitversion.dientich2 = unitEdit.dientich2;
                _Unitversion.ngansachnhannuoc1 = unitEdit.ngansachnhannuoc1;
                _Unitversion.ngansachnhanuoc2 = unitEdit.ngansachnhanuoc2;
                _Unitversion.ngaythanhlap = unitEdit.ngaythanhlap;
                _Unitversion.tongcanbo1 = unitEdit.tongcanbo1;
                _Unitversion.tongcanbo2 = unitEdit.tongcanbo2;
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
            return Json(resSubmit, JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public ActionResult ThongTinCaNhan(ResetPass resetPass)
        {
            if(resetPass!=null && resetPass.MatKhauCu!=null && resetPass.MatKhauCu.Length>0 && resetPass.MatKhauMoi.Length>0 && resetPass.ReMatKhauMoi.Length > 0)
            {
                tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
                AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
                ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
                var _Account = tongcuclamnghiep.m_account.Where(a => a.id == accountCookie.Id && a.password == resetPass.MatKhauCu).FirstOrDefault();
                if (_Account == null)
                {
                    resSubmit = new ResSubmit(false, "Không có tài khoản này");
                }
                if (resSubmit.success && !(resetPass.MatKhauMoi == resetPass.ReMatKhauMoi && (resetPass.MatKhauMoi.Length >= 12 && resetPass.MatKhauMoi.Length <= 50)))
                {
                    resSubmit = new ResSubmit(false, "Mật khẩu mới không hợp lệ");
                }
                if (resSubmit.success)
                {
                    _Account.password = resetPass.MatKhauMoi;
                    if (tongcuclamnghiep.SaveChanges() != 1)
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                    else
                    {
                        string body = string.Format(@"<table width='100%' cellspacing='0' cellpadding='0' border='0' style='width:100%;min-width:100%'>
			<tbody>
            <tr>
				<td align='left' style='padding:0;font-size:20px;line-height:26px;letter-spacing:-0.32px;font-weight:normal;font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;color:#0078d6'>
					Tóm tắt tài khoản
				</td>
			</tr>
			<tr>
				<td class='m_-8498465890789338487useremail' align='left' style='padding:3px 0 0;font-size:14px;line-height:19px;letter-spacing:-0.16px;font-weight:normal;font-family:'Segoe UI','Segoe UI Regular',SUWR,Arial,sans-serif;color:#6e6e6e'>
					<b style='font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;font-weight:normal'>Tên Tài khoản:</b> <a href='#m_-8498465890789338487_' style='text-decoration:none;color:#737373'>{0}</a>
				</td>
			</tr>
            <tr>
				<td class='m_-8498465890789338487useremail' align='left' style='padding:3px 0 0;font-size:14px;line-height:19px;letter-spacing:-0.16px;font-weight:normal;font-family:'Segoe UI','Segoe UI Regular',SUWR,Arial,sans-serif;color:#6e6e6e'>
					<b style='font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;font-weight:normal'>Mật khẩu:</b> <a href='#m_-8498465890789338487_' style='text-decoration:none;color:#737373'>{1}</a>
				</td>
			</tr>
			<tr>
				<td align='left' style='padding:3px 0 0;font-size:14px;line-height:19px;letter-spacing:-0.16px;font-weight:normal;font-family:'Segoe UI Semibold','Segoe UI',SUWSB,Arial,sans-serif;color:#6e6e6e'>
					<a href='http://baocaolamnghiep.quantriwebhanoi.com' style='color:#6e6e6e;text-decoration:underline' target='_blank' ><strong style='font-weight:normal'>Tài khoản của Tôi</strong></a>
				</td>
			</tr>
		</tbody></table>", _Account.username, _Account.password);
                        FuncExtend.SendMail(_Account.email, body, "Thông tin tài khoản");
                    }
                }
                ModelState.AddModelError("", resSubmit.message);
                return View();
            }
            else
            {
                return View();
            }
            
        }
    }
}