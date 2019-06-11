using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models.Report
{
    public class MReportnew
    {
        public class HeaderComponent
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public int TypeValue { get; set; }
            public string TextUnit { get; set; }
            public bool TextBold { get; set; }
            public string TextAlign { get; set; }
            public bool TextUpper { get; set; }
            public bool HeadBold { get; set; }
            public bool HeadUpper { get; set; }
            public string TextValue { get; set; }
            public int ColSpan { get; set; }
            public int RowSpan { get; set; }
            public int? HeaderId { get; set; }
            public int? Level { get; set; }
            public bool? Is_No { get; set; }
            public bool? Is_Component { get; set; }
            public bool Save { get; set; }
            public int OrderHeader { get; set; }
        }

        public class TableComponent
        {
            public int RowId { get; set; }
            public List<HeaderComponent> headers { get; set; }
            
        }

        public class TableReport
        {
            public int? IdBaoCao { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }
            public bool AutoRow { get; set; }
            public List<TableComponent> tableComponents { get; set; }
            public List<HeaderComponent> headers { get; set; }
        }

        public class BangBaoCao
        {
            public string TieuDe { get; set; }
            public int SapXep { get; set; }
            public int Loai { get; set; }
            public List<TieuDeBaoCao> DanhSachTieuDe { get; set; }
            public List<TieuDeBaoCao> DanhSachTieuDeCol { get; set; }
            public List<DongBaoCao> DanhSachDong { get; set; }
            public int Id { get; set; }
        }

        public class DongBaoCao
        {
            public int RowId { get; set; }
            public List<DuLieuCot> DanhSachCot { get; set; }
        }

        public class DuLieuCot
        {
            public TieuDeBaoCao TieuDeCot { get; set; }
            public string DuLieuText { get; set; }
            public int Id { get; set; }
            public int TypeValue { get; set; }
            public bool Save { get; set; }
            public List<DataTongNumberDonVi> TongDuLieuInt { get; set; }
            public List<DataTongStringDonVi> DuLieuTextTong { get; set; }
            public double TongNumber { get { return (this.TongDuLieuInt != null ? this.TongDuLieuInt.Sum(a => (a.Data != null ? a.Data.Value : 0)) : 0); } }

        }
        public class DataTongNumberDonVi
        {
            public int IdUnit { get; set; }
            public string UnitName { get; set; }
            public double? Data { get; set; }
        }
        public class DataTongStringDonVi
        {
            public int IdUnit { get; set; }
            public string UnitName { get; set; }
            public string Data { get; set; }
        }
        public class TieuDeBaoCao
        {
            public int Id { get; set; }
            public string TieuDe { get; set; }
            public int ColSpan { get; set; }
            public int RowSpan { get; set; }
            public int Level { get; set; }
            public int Order { get; set; }
            public bool IsNo { get; set; }
            public bool IsComponent { get; set; }
            public int? HeaderId { get; set; }
            public string TextAlignValue { get; set; }
            public bool TextUpperValue { get; set; }
            public bool TextBoldValue { get; set; }
            public bool Upper { get; set; }
            public bool Bold { get; set; }
        }

        public ResSubmit HoanThanhBaoCaoUnit(AccountCookie accountCookie)
        {
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id && a.status == true).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();
            if(_Unit==null || _Precious == null)
            {
                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
            }
            if (resSubmit.success)
            {
                var _UnitPrecious = tongcuclamnghiep.m_unit_precious.Where(a => a.precious_id == _Precious.id && a.unit_id == _Unit.id).FirstOrDefault();
                if (_UnitPrecious == null)
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }
                else
                {
                    if (_UnitPrecious.completedate != null)
                    {
                        resSubmit = new ResSubmit(false, "Bạn đã hoàn thành rồi");
                    }
                    else
                    {
                        _UnitPrecious.completedate = DateTime.Now;
                        if (tongcuclamnghiep.SaveChanges() != 1)
                        {
                            resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                        }
                    }
                }
            }
            return resSubmit;
        }

        public ResSubmit XoaDuLieuBaoCaoUnitAuto(int RowId, int IdBaoCao, AccountCookie accountCookie)
        {
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id && a.status == true).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();
            var _BaoCao = tongcuclamnghiep.m_reporttable.Where(a => a.id == IdBaoCao && a.status == true).FirstOrDefault();
            if(_Unit==null || _Precious == null || _BaoCao==null)
            {
                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
            }
            if (resSubmit.success)
            {
                var _UnitPrecious = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == _Precious.id && a.completedate==null).FirstOrDefault();
                if (_UnitPrecious == null)
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }
                if (resSubmit.success)
                {
                    var _RowData = tongcuclamnghiep.m_unit_datereport_autorows.Where(a => a.orderrow == RowId && a.unit_precious_id==_UnitPrecious.id && a.reporttable_id== _BaoCao.id).ToList();
                    if (_RowData.Count() > 0)
                    {
                        _RowData.All(a=> {
                            tongcuclamnghiep.m_unit_datereport_autorows.Remove(a);
                            return true;
                        });
                        if (tongcuclamnghiep.SaveChanges() <= 0)
                        {
                            resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                        }
                    }
                    else
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                }
            }
            return resSubmit;
        }


        public ResSubmit CapNhatDataComponentAutoReportUnit(int RowHeaderId, string TextValue, AccountCookie accountCookie)
        {
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id && a.status == true).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();

            
            if(_Unit==null || _Precious==null)
            {
                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
            }
            if (resSubmit.success)
            {
                var _UnitPrecious = tongcuclamnghiep.m_unit_precious.Where(a => a.precious_id == _Precious.id && a.unit_id == _Unit.id && a.completedate == null).FirstOrDefault();
                if (_UnitPrecious != null)
                {
                    var _UnitReportDataAuto = tongcuclamnghiep.m_unit_datereport_autorows.Where(a => a.id == RowHeaderId && a.unit_precious_id== _UnitPrecious.id).FirstOrDefault();
                    if (_UnitReportDataAuto != null)
                    {
                        _UnitReportDataAuto.datatext = TextValue;
                        _UnitReportDataAuto.editdate = DateTime.Now;
                        if (tongcuclamnghiep.SaveChanges() != 1)
                        {
                            resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                        }
                    }
                    else
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                }
                else
                {
                    resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                }

                
            }
            return resSubmit;
        }

        public ResSubmit ThemMoiThanhPhanAuto(int IdBaoCao, int RowId, AccountCookie accountCookie)
        {
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id && a.status == true).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();
            var _TableReport = tongcuclamnghiep.m_reporttable.Where(a => a.id == IdBaoCao && a.status == true && a.autorow==true).FirstOrDefault();
            if(_Unit==null || _Precious == null || _TableReport==null)
            {
                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
            }
            if (resSubmit.success)
            {
                int IdUnitBaoCao = 0;
                var _UnitBaoCao = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == _Precious.id).FirstOrDefault();
                if (_UnitBaoCao != null)
                {
                    if (_UnitBaoCao.completedate == null)
                    {
                        IdUnitBaoCao = _UnitBaoCao.id;
                    }
                    else
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                }
                else
                {
                    m_unit_precious _Unit_Precious = new m_unit_precious();
                    _Unit_Precious.createdate = DateTime.Now;
                    _Unit_Precious.precious_id = _Precious.id;
                    _Unit_Precious.unit_id = _Unit.id;
                    tongcuclamnghiep.m_unit_precious.Add(_Unit_Precious);
                    if (tongcuclamnghiep.SaveChanges() != 1)
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                    else
                    {
                        IdUnitBaoCao = _Unit_Precious.id;
                    }
                }
                if (resSubmit.success)
                {
                    if(!tongcuclamnghiep.m_unit_datereport_autorows.Any(a=>a.orderrow== RowId && a.reporttable_id== IdBaoCao && a.unit_precious_id== IdUnitBaoCao))
                    {
                        _TableReport.m_header.Where(a => a.status == true && a.colspan <= 1).ToList().ForEach(a =>
                        {
                            m_unit_datereport_autorows _Unit_Datereport_Autorows = new m_unit_datereport_autorows();
                            _Unit_Datereport_Autorows.createdate = DateTime.Now;
                            _Unit_Datereport_Autorows.datatext = "";
                            _Unit_Datereport_Autorows.editdate = DateTime.Now;
                            _Unit_Datereport_Autorows.header_id = a.id;
                            _Unit_Datereport_Autorows.orderrow = RowId;
                            _Unit_Datereport_Autorows.reporttable_id = IdBaoCao;
                            _Unit_Datereport_Autorows.unit_precious_id = IdUnitBaoCao;
                            tongcuclamnghiep.m_unit_datereport_autorows.Add(_Unit_Datereport_Autorows);
                        });
                        if (tongcuclamnghiep.SaveChanges() > 0)
                        {
                            List<TieuDeBaoCao> DanhSachTieuDeCol = GetDanhSachTieuDeHienThi(1, 0, IdBaoCao);
                            DongBaoCao dongBaoCao = new DongBaoCao();
                            dongBaoCao.RowId = RowId;
                            List<DuLieuCot> duLieuCots = new List<DuLieuCot>();
                            DanhSachTieuDeCol.ToList().ForEach(c =>
                            {
                                DuLieuCot duLieuCot = new DuLieuCot();
                                duLieuCot.TieuDeCot = c;

                                var DataCell = tongcuclamnghiep.m_unit_datereport_autorows.Where(d =>d.reporttable_id==IdBaoCao && d.orderrow==RowId && d.header_id == c.Id).FirstOrDefault();
                                if (DataCell != null)
                                {
                                    duLieuCot.Id = DataCell.id;
                                    duLieuCot.DuLieuText = DataCell.datatext;
                                }
                                duLieuCots.Add(duLieuCot);
                            });
                            dongBaoCao.DanhSachCot = duLieuCots;
                            resSubmit.extend = dongBaoCao;
                        }
                        else
                        {
                            resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                        }
                    }
                    else
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                }
            }
            return resSubmit;
        }



        public List<TieuDeBaoCao> GetDanhSachTieuDeHienThi(int Level, int IdHeader, int IdBaoCao)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            List<TieuDeBaoCao> tieuDeBaosNew = new List<TieuDeBaoCao>();

            if (IdHeader == 0)
            {
                tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao && a.level == Level && a.header_id == null).OrderBy(a => a.order).ToList().ForEach(a =>
                {
                    if (a.colspan > 1)
                    {
                        List<TieuDeBaoCao> tieuDeBaosNewTemp = GetDanhSachTieuDeHienThi((Level + 1), a.id, IdBaoCao);
                        foreach (var item in tieuDeBaosNewTemp)
                        {
                            tieuDeBaosNew.Add(item);
                        }
                    }
                    else
                    {
                        tieuDeBaosNew.Add(new TieuDeBaoCao()
                        {
                            Id = a.id,
                            TieuDe = a.headername
                        });
                    }
                });
            }
            else
            {
                tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao && a.level == Level && a.header_id == IdHeader).OrderBy(a => a.order).ToList().ForEach(a =>
                {
                    if (a.colspan > 1)
                    {
                        List<TieuDeBaoCao> tieuDeBaosNewTemp = GetDanhSachTieuDeHienThi((Level + 1), a.id, IdBaoCao);
                        foreach (var item in tieuDeBaosNewTemp)
                        {
                            tieuDeBaosNew.Add(item);
                        }
                    }
                    else
                    {
                        tieuDeBaosNew.Add(new TieuDeBaoCao()
                        {
                            Id = a.id,
                            TieuDe = a.headername
                        });
                    }
                });
            }

            return tieuDeBaosNew;
        }
        public List<BangBaoCao> GetDanhSachBaoCaoUnitNew(AccountCookie accountCookie)
        {
            List<BangBaoCao> bangBaoCaos = new List<BangBaoCao>();
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id).FirstOrDefault();
            if (_Precious != null && _Unit != null)
            {
                bool CheckView = true;
                var _UnitPrecious = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == _Precious.id).FirstOrDefault();
                if (_UnitPrecious != null)
                {
                    if (_UnitPrecious.completedate != null)
                    {
                        CheckView = false;
                    }
                }
                if (CheckView)
                {
                    int Precious_Id_Before = 0;
                    int UnitPrecious_Id = 0;
                    if (tongcuclamnghiep.m_precious.Any(a => a.status == false && a.id != _Precious.id))
                    {
                        Precious_Id_Before = tongcuclamnghiep.m_precious.Where(a => a.status == true && a.id != _Precious.id).OrderByDescending(a => a.completedate).Select(a => a.id).FirstOrDefault();
                    }
                    if (_UnitPrecious != null)
                    {
                        UnitPrecious_Id = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == Precious_Id_Before).Select(a => a.id).FirstOrDefault();
                    }
                    tongcuclamnghiep.m_reporttable.Where(a => a.status == true).OrderBy(a => a.order).ToList().ForEach(a =>
                    {
                        BangBaoCao bangBaoCao = new BangBaoCao();
                        bangBaoCao.Loai = a.autorow == true ? 1 : 0;
                        bangBaoCao.SapXep = a.order.Value;
                        bangBaoCao.Id = a.id;
                        bangBaoCao.TieuDe = a.title;
                        bangBaoCao.DanhSachTieuDeCol = GetDanhSachTieuDeHienThi(1, 0, a.id);

                        List<TieuDeBaoCao> tieuDeBaoCaos = new List<TieuDeBaoCao>();

                        a.m_header.OrderBy(b => b.level).ThenBy(b => b.order).ToList().ForEach(b => {
                            TieuDeBaoCao tieuDeBaoCao = new TieuDeBaoCao();
                            tieuDeBaoCao.Bold = b.is_bold.Value;
                            tieuDeBaoCao.ColSpan = b.colspan.Value;
                            tieuDeBaoCao.HeaderId = b.header_id;
                            tieuDeBaoCao.Id = b.id;
                            tieuDeBaoCao.IsComponent = b.is_component.Value;
                            tieuDeBaoCao.IsNo = b.is_no.Value;
                            tieuDeBaoCao.Level = b.level.Value;
                            tieuDeBaoCao.Order = b.order.Value;
                            tieuDeBaoCao.RowSpan = b.rowspan.Value;
                            tieuDeBaoCao.TextAlignValue = b.valuetextalign;
                            tieuDeBaoCao.TextBoldValue = b.is_valuebold.Value;
                            tieuDeBaoCao.TextUpperValue = b.is_valueupper.Value;
                            tieuDeBaoCao.TieuDe = b.headername;
                            tieuDeBaoCao.Upper = b.is_upper.Value;
                            tieuDeBaoCaos.Add(tieuDeBaoCao);

                        });
                        bangBaoCao.DanhSachTieuDe = tieuDeBaoCaos;
                        List<DongBaoCao> dongBaoCaos = new List<DongBaoCao>();
                        if (a.autorow == false)
                        {
                            a.m_component.OrderBy(b => b.orderrow).GroupBy(b => b.orderrow).ToList().ForEach(b =>
                            {
                                DongBaoCao dongBaoCao = new DongBaoCao();
                                dongBaoCao.RowId = b.Key.Value;
                                List<DuLieuCot> duLieuCots = new List<DuLieuCot>();
                                bangBaoCao.DanhSachTieuDeCol.ToList().ForEach(c =>
                                {
                                    DuLieuCot duLieuCot = new DuLieuCot();
                                    var TieuDeCot = tieuDeBaoCaos.Where(d => d.Id == c.Id).FirstOrDefault();
                                    duLieuCot.TieuDeCot = TieuDeCot;
                                    var DataCell = b.Where(d => d.header_id == TieuDeCot.Id).FirstOrDefault();
                                    if (DataCell != null)
                                    {
                                        duLieuCot.Id = DataCell.id;
                                        duLieuCot.DuLieuText = DataCell.componentname;
                                        duLieuCot.TypeValue = DataCell.typevalue.Value;
                                        if (!(TieuDeCot.IsComponent == true || TieuDeCot.IsNo == true) && _UnitPrecious != null)
                                        {
                                            var DataCellUnit = DataCell.m_unit_datareport.Where(d => d.unit_precious_id == _UnitPrecious.id).FirstOrDefault();
                                            if (DataCellUnit != null)
                                            {
                                                duLieuCot.DuLieuText = DataCellUnit.data_text;
                                                duLieuCot.Save = true;
                                            }
                                        }
                                        if (!(TieuDeCot.IsComponent == true || TieuDeCot.IsNo == true) && _UnitPrecious == null)
                                        {
                                            if (Precious_Id_Before > 0 && tongcuclamnghiep.m_unit_precious.Any(d=>d.precious_id== Precious_Id_Before && d.unit_id == _Unit.id))
                                            {
                                                int IdUnitPreciousBF = tongcuclamnghiep.m_unit_precious.Where(d => d.precious_id == Precious_Id_Before && d.unit_id == _Unit.id).Select(d=>d.id).FirstOrDefault();
                                                var DataCellUnit = tongcuclamnghiep.m_unit_datareport.Where(d => d.unit_precious_id == IdUnitPreciousBF && d.component_id == DataCell.id).FirstOrDefault();

                                                if (DataCellUnit != null)
                                                {
                                                    duLieuCot.DuLieuText = DataCellUnit.data_text;
                                                }
                                            }
                                        }
                                    }
                                    duLieuCots.Add(duLieuCot);
                                });
                                dongBaoCao.DanhSachCot = duLieuCots;
                                dongBaoCaos.Add(dongBaoCao);
                            });
                        }
                        else
                        {
                            if (_UnitPrecious != null)
                            {
                                tongcuclamnghiep.m_unit_datereport_autorows.Where(b => b.reporttable_id == a.id && b.unit_precious_id == _UnitPrecious.id).GroupBy(b => b.orderrow).ToList().ForEach(b =>
                                {
                                    DongBaoCao dongBaoCao = new DongBaoCao();
                                    dongBaoCao.RowId = b.Key.Value;
                                    List<DuLieuCot> duLieuCots = new List<DuLieuCot>();
                                    bangBaoCao.DanhSachTieuDeCol.ToList().ForEach(c =>
                                    {
                                        DuLieuCot duLieuCot = new DuLieuCot();
                                        duLieuCot.TieuDeCot = c;

                                        var DataCell = b.Where(d => d.header_id == c.Id).FirstOrDefault();
                                        if (DataCell != null)
                                        {
                                            duLieuCot.Id = DataCell.id;
                                            duLieuCot.DuLieuText = DataCell.datatext;
                                        }
                                        duLieuCots.Add(duLieuCot);
                                    });
                                    dongBaoCao.DanhSachCot = duLieuCots;
                                    dongBaoCaos.Add(dongBaoCao);
                                });
                            }

                        }
                        bangBaoCao.DanhSachDong = dongBaoCaos;
                        bangBaoCaos.Add(bangBaoCao);

                    });
                }
                
            }

            return bangBaoCaos;
        }

        public List<TableComponent> GetTableReportAutoRow(int IdBaoCao, AccountCookie accountCookie)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            List<TableComponent> tableComponents = new List<TableComponent>();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id && a.status == true).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a =>a.status==false).FirstOrDefault();
            var _ReportTable = tongcuclamnghiep.m_reporttable.Where(a => a.status == true && a.autorow == true && a.id == IdBaoCao).FirstOrDefault();
            if (!(_Unit == null || _Precious == null || _ReportTable==null))
            {
                var _UnitReport = tongcuclamnghiep.m_unit_precious.Where(a => a.precious_id == _Precious.id && a.unit_id == _Unit.id).FirstOrDefault();
                bool CheckView = true;
                if (_UnitReport == null)
                {
                    CheckView = false;
                }
                else
                {
                    if (_UnitReport.completedate != null)
                    {
                        CheckView = false;
                    }
                }
                if (CheckView == true)
                {
                    tongcuclamnghiep.m_unit_datereport_autorows.Where(a => a.reporttable_id == IdBaoCao && a.unit_precious_id == _UnitReport.id).GroupBy(a => a.orderrow).ToList().ForEach(a =>
                    {
                        TableComponent tableComponent = new TableComponent();
                        tableComponent.RowId = a.Key.Value;
                        List<HeaderComponent> headerComponents = new List<HeaderComponent>();
                        _ReportTable.m_header.OrderBy(b => b.level).OrderBy(b => b.order).ToList().ForEach(b =>
                        {
                            var datahead = a.Where(c => c.header_id == b.id).FirstOrDefault();
                            if (datahead != null)
                            {
                                HeaderComponent headerComponent = new HeaderComponent();
                                headerComponent.Id = datahead.id;
                                headerComponent.OrderHeader = b.order.Value;
                                headerComponent.TextAlign = b.valuetextalign;
                                headerComponent.TextBold = b.is_valuebold.Value;
                                headerComponent.TextUpper = b.is_valueupper.Value;
                                headerComponent.TextValue = datahead.datatext;
                                headerComponent.HeaderId = b.id;
                                headerComponents.Add(headerComponent);
                            }
                        });
                        tableComponent.headers = headerComponents;
                        tableComponents.Add(tableComponent);
                    });
                }
            }
            return tableComponents;
        }

        public ResSubmit CapNhatDataComponentReportUnit(int IdComponent,string TextValue, AccountCookie accountCookie )
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            ResSubmit resSubmit = new ResSubmit(true, "Cập nhật thành công");
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.account_id == accountCookie.Id && a.status == true).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();
            var _Component = tongcuclamnghiep.m_component.Where(a => a.id == IdComponent && a.status == true).FirstOrDefault();
            if(_Unit==null || _Precious==null || _Component == null)
            {
                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
            }
            if (resSubmit.success)
            {
                if(_Component.typevalue==2 || _Component.typevalue == 3)
                {
                    double Numb = 0;
                    if(!double.TryParse(TextValue,out Numb))
                    {
                        resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                    }
                }
                if (resSubmit.success)
                {
                    int IdUnitPrecious = 0;
                    var _UnitReport = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == _Precious.id).FirstOrDefault();
                    if (_UnitReport != null)
                    {
                        if (_UnitReport.completedate == null)
                        {
                            IdUnitPrecious = _UnitReport.id;
                        }
                        else
                        {
                            resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                        }
                    }
                    else
                    {
                        m_unit_precious _Unit_Precious = new m_unit_precious();
                        _Unit_Precious.precious_id = _Precious.id;
                        _Unit_Precious.unit_id = _Unit.id;
                        _Unit_Precious.createdate = DateTime.Now;
                        tongcuclamnghiep.m_unit_precious.Add(_Unit_Precious);
                        if (tongcuclamnghiep.SaveChanges() != 1)
                        {
                            resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                        }
                        else
                        {
                            IdUnitPrecious = _Unit_Precious.id;
                        }
                    }
                    if (resSubmit.success)
                    {
                        int IdData = 0;
                        var _UnitDataReport = tongcuclamnghiep.m_unit_datareport.Where(a => a.component_id == IdComponent && a.unit_precious_id == IdUnitPrecious).FirstOrDefault();
                        if (_UnitDataReport == null)
                        {
                            m_unit_datareport _Unit_DatareportNew = new m_unit_datareport();
                            _Unit_DatareportNew.component_id = IdComponent;
                            _Unit_DatareportNew.editlasttime = DateTime.Now;
                            _Unit_DatareportNew.createdate = DateTime.Now;
                            _Unit_DatareportNew.data_text = TextValue;
                            _Unit_DatareportNew.unit_precious_id = IdUnitPrecious;
                            tongcuclamnghiep.m_unit_datareport.Add(_Unit_DatareportNew);
                            if (tongcuclamnghiep.SaveChanges() != 1)
                            {
                                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                            }
                            else
                            {
                                IdData = _Unit_DatareportNew.id;
                            }
                        }
                        else
                        {
                            IdData = _UnitDataReport.id;
                            _UnitDataReport.data_text = TextValue;
                            _UnitDataReport.editlasttime = DateTime.Now;
                            if (tongcuclamnghiep.SaveChanges() != 1)
                            {
                                resSubmit = new ResSubmit(false, "Cập nhật thất bại");
                            }
                        }
                    }
                }
            }
            return resSubmit;
        }

        public List<TableReport> GetDataReport(AccountCookie accountCookie)
        {
            List<TableReport> tableReports = new List<TableReport>();
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.status == true && a.account_id == accountCookie.Id).FirstOrDefault();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.status == false).FirstOrDefault();
            if (_Precious != null && _Unit!=null)
            {
                bool CheckView = true;
                var _UnitPrecious = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == _Precious.id).FirstOrDefault();
                if (_UnitPrecious != null)
                {
                    if (_UnitPrecious.completedate != null)
                    {
                        CheckView = false;
                    }
                }
                if (CheckView == true)
                {
                    int Precious_Id_Before = 0;
                    int UnitPrecious_Id = 0;
                    if (tongcuclamnghiep.m_precious.Any(a => a.status == false && a.id != _Precious.id))
                    {
                        Precious_Id_Before = tongcuclamnghiep.m_precious.Where(a => a.status == true && a.id != _Precious.id).OrderByDescending(a => a.completedate).Select(a => a.id).FirstOrDefault();
                    }
                    if (_UnitPrecious != null)
                    {
                        UnitPrecious_Id = tongcuclamnghiep.m_unit_precious.Where(a => a.unit_id == _Unit.id && a.precious_id == Precious_Id_Before).Select(a => a.id).FirstOrDefault();
                    }
                    tongcuclamnghiep.m_reporttable.Where(a => a.status == true).OrderBy(a => a.order).ToList().ForEach(a =>
                    {
                        TableReport tableReport = new TableReport();
                        tableReport.IdBaoCao = a.id;
                        tableReport.Title = a.title;
                        tableReport.Order = a.order.Value;
                        tableReport.AutoRow = a.autorow.Value;
                        List<HeaderComponent> headers = new List<HeaderComponent>();
                        tongcuclamnghiep.m_header.Where(b => a.status == true && b.reporttable_id == a.id).OrderBy(b => b.order).ToList().ForEach(b =>
                        {
                            HeaderComponent header = new HeaderComponent();
                            header.Id = b.id;
                            header.Title = b.headername;
                            header.RowSpan = b.rowspan.Value;
                            header.ColSpan = b.colspan.Value;
                            header.HeaderId = b.header_id;
                            header.Level = b.level;
                            header.Is_No = b.is_no;
                            header.Is_Component = b.is_component;
                            header.OrderHeader = b.order.Value;
                            headers.Add(header);
                        });
                        tableReport.headers = headers.OrderBy(b => b.Level).ThenBy(b => b.OrderHeader).ToList();
                        List<TableComponent> tableComponents = new List<TableComponent>();
                        a.m_component.Where(b => b.status == true).GroupBy(b => b.orderrow).ToList().ForEach(b =>
                        {
                            TableComponent tableComponent = new TableComponent();
                            tableComponent.RowId = b.Key.Value;
                            List<HeaderComponent> headerComponents = new List<HeaderComponent>();
                            tableReport.headers.ForEach(c => {
                                var _CPM = tongcuclamnghiep.m_component.Where(d => d.header_id == c.Id && d.orderrow == b.Key).FirstOrDefault();
                                if (_CPM != null)
                                {
                                    HeaderComponent header = new HeaderComponent();
                                    header.Id = _CPM.id;
                                    header.OrderHeader = c.OrderHeader;
                                    header.HeaderId = _CPM.header_id;
                                    header.TextAlign = _CPM.m_header.valuetextalign;
                                    header.TextBold = _CPM.m_header.is_valuebold.Value;
                                    header.TextUpper = _CPM.m_header.is_valueupper.Value;
                                    header.TextValue = _CPM.componentname;
                                    header.Title = _CPM.m_header.headername;
                                    header.TypeValue = _CPM.typevalue.Value;
                                    header.Is_No = _CPM.m_header.is_no;
                                    if (_UnitPrecious == null)
                                    {
                                        header.Save = false;
                                        if (Precious_Id_Before > 0 && UnitPrecious_Id > 0)
                                        {
                                            var _dataUnitReportBefore = tongcuclamnghiep.m_unit_datareport.Where(d => d.component_id == _CPM.id && d.unit_precious_id == UnitPrecious_Id).FirstOrDefault();
                                            if (_dataUnitReportBefore == null)
                                            {
                                                header.TextUnit = "";
                                            }
                                            else
                                            {
                                                header.TextUnit = _dataUnitReportBefore.data_text;
                                            }
                                        }
                                        else
                                        {
                                            header.TextUnit = "";
                                        }
                                    }
                                    else
                                    {
                                        var _dataUnitReport = _CPM.m_unit_datareport.Where(d => d.unit_precious_id == _UnitPrecious.id).FirstOrDefault();
                                        if (_dataUnitReport == null)
                                        {
                                            header.Save = false;
                                            if (Precious_Id_Before > 0 && UnitPrecious_Id > 0)
                                            {
                                                var _dataUnitReportBefore = tongcuclamnghiep.m_unit_datareport.Where(d => d.component_id == _CPM.id && d.unit_precious_id == UnitPrecious_Id).FirstOrDefault();
                                                if (_dataUnitReportBefore == null)
                                                {
                                                    header.TextUnit = "";
                                                }
                                                else
                                                {
                                                    header.TextUnit = _dataUnitReportBefore.data_text;
                                                }
                                            }
                                            else
                                            {
                                                header.TextUnit = "";
                                            }
                                        }
                                        else
                                        {
                                            header.Save = true;
                                            header.TextUnit = _dataUnitReport.data_text;
                                        }
                                    }

                                    header.Is_Component = _CPM.m_header.is_component;
                                    headerComponents.Add(header);

                                }
                            });
                            tableComponent.headers = headerComponents;

                            tableComponents.Add(tableComponent);
                        });
                        tableReport.tableComponents = tableComponents;

                        tableReports.Add(tableReport);
                    });
                }
            }
            return tableReports;
        }
    }
}