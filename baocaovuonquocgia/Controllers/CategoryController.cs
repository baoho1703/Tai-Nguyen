using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baocaovuonquocgia.Models;

namespace baocaovuonquocgia.Controllers
{
    public class CategoryController : Controller
    {
        [CheckSession]
        // GET: Unit
        public ActionResult Unit(int? tab, int? IdChinhSua)
        {
            ControllerTab controllerTab = new ControllerTab();
            if (tab == null)
            {
                controllerTab.Tab = 1;
            }
            else
            {
                controllerTab.Tab = tab.Value;
                if (tab == 3 || tab == 4)
                {
                    controllerTab.IdChinhSua = IdChinhSua.Value;
                }
            }
            return View(controllerTab);
        }

        [CheckSession]
        // GET: Formreport
        public ActionResult Formreport(int? tab, int? IdChinhSua)
        {
            ControllerTab controllerTab = new ControllerTab();
            if (tab == null)
            {
                controllerTab.Tab = 1;
            }
            else
            {
                controllerTab.Tab = tab.Value;
                if (tab == 2 || tab == 3)
                {
                    controllerTab.IdChinhSua = IdChinhSua.Value;
                }
            }
            return View(controllerTab);
        }
        [CheckSession]
        // GET: FormreportUnit
        public ActionResult FormreportUnit()
        {
            return View();
        }

        [CheckSessionAjax]
        public ActionResult DemoTreeTable()
        {
            return View();
        }

        //[CheckSessionAjax]
        public ActionResult DemoDesignTable()
        {
            return View();
        }


        #region -- Unit --

        [CheckSessionAjax]
        public JsonResult GetUnitTable(int start, int length, int draw)
        {
            string search = Request["search[value]"];
            return Json(new baocaovuonquocgia.Models.Category.MUnit().GetUnitTable(start, length, draw, search), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult CapNhatNguoiQuanLy(int UnitId, int AccountId)
        {
            AccountCookie accountCookie = (AccountCookie)Session["AccountCookie"];
            return Json(new baocaovuonquocgia.Models.Category.MUnit().CapNhatNguoiQuanLy(UnitId, AccountId, accountCookie), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ThemMoiUnit(baocaovuonquocgia.Models.Category.MUnit.UnitAdd unitAdd)
        {
            HttpRequestBase request = HttpContext.Request;
            string UnitVersionText = request.Unvalidated.Form.Get("UnitVersionTextData");
            unitAdd.UnitVersionText = UnitVersionText;
            return Json(new baocaovuonquocgia.Models.Category.MUnit().ThemMoiUnit(unitAdd), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ChinhSuaUnit(baocaovuonquocgia.Models.Category.MUnit.UnitAdd unitEdit)
        {
            HttpRequestBase request = HttpContext.Request;
            string UnitVersionText = request.Unvalidated.Form.Get("UnitVersionTextData");
            unitEdit.UnitVersionText = UnitVersionText;
            return Json(new baocaovuonquocgia.Models.Category.MUnit().ChinhSuaUnit(unitEdit), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region -- Formreport --

        [CheckSessionAjax]
        public JsonResult CapNhatSapXepBaoCao(List<Models.Category.MFormreport.listBaoCao> listBaoCaos)
        {
            return Json(new Models.Category.MFormreport().CapNhatSapXepBaoCao(listBaoCaos), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetTreeBaoCao(int IdBaoCao, int IdHeaderCha, bool isFirst)
        {
            return Json(new Models.Category.MFormreport().GetTreeBaoCao(IdBaoCao, IdHeaderCha, isFirst), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult CapNhatHeader(Models.Category.MFormreport.HeaderAdd headeEdit)
        {
            return Json(new Models.Category.MFormreport().CapNhatHeader(headeEdit), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ThemMoiHeader(Models.Category.MFormreport.HeaderAdd headeAdd)
        {
            return Json(new Models.Category.MFormreport().ThemMoiHeader(headeAdd), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetTreeBaoCaoThanhPhan(int IdBaoCao, int IdHeaderCha, bool isFirst)
        {
            return Json(new Models.Category.MFormreport().GetTreeBaoCaoThanhPhan(IdBaoCao, IdHeaderCha, isFirst), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetTableReportHorizontal(int IdBaoCao)
        {
            return Json(new Models.Category.MFormreport().GetTableReportHorizontal(IdBaoCao), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult ThemMoiThanhPhan(int IdBaoCao, int RowId)
        {
            return Json(new Models.Category.MFormreport().ThemMoiThanhPhan(IdBaoCao, RowId), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetTableComponentReport(int IdBaoCao)
        {
            return Json(new Models.Category.MFormreport().GetTableComponentReport(IdBaoCao), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult CapNhatThanhPhanDataType(Models.Category.MFormreport.ComponentUpdate componentUpdate)
        {
            //return Json(componentUpdate, JsonRequestBehavior.AllowGet);
            return Json(new Models.Category.MFormreport().CapNhatThanhPhanDataType(componentUpdate), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult XoaComponent(int RowId, int IdBaoCao)
        {
            return Json(new Models.Category.MFormreport().XoaComponent(RowId, IdBaoCao), JsonRequestBehavior.AllowGet);
        }

        [CheckSessionAjax]
        public JsonResult GetTableEmpty()
        {
            return Json(new Models.ReportExtend().GetTableEmpty(), JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region -- DemoTreeTable --

        [CheckSessionAjax]
        public JsonResult GetBangBaoCao(int IdBaoCao)
        {
            return Json(new Models.Category.MDemoTreeTable().GetBangBaoCao(IdBaoCao), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}