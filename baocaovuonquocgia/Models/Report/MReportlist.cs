using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models.Report
{
    public class MReportlist
    {
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
            public List<DataTongNumberDonVi> TongDuLieuInt { get; set; }
            public List<DataTongStringDonVi> DuLieuTextTong { get; set; }
            public double TongNumber { get { return (this.TongDuLieuInt!=null ? this.TongDuLieuInt.Sum(a => (a.Data != null ? a.Data.Value : 0)) : 0); } }

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

        public List<TieuDeBaoCao> GetDanhSachTieuDeHienThi(int Level, int IdHeader,int IdBaoCao)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            List<TieuDeBaoCao> tieuDeBaosNew = new List<TieuDeBaoCao>();
            
            if (IdHeader == 0)
            {
                tongcuclamnghiep.m_header.Where(a => a.reporttable_id == IdBaoCao && a.level == Level && a.header_id == null).OrderBy(a=>a.order).ToList().ForEach(a =>
                {
                    if (a.colspan > 1)
                    {
                        List<TieuDeBaoCao> tieuDeBaosNewTemp = GetDanhSachTieuDeHienThi((Level+1), a.id, IdBaoCao);
                        foreach (var item in tieuDeBaosNewTemp)
                        {
                            tieuDeBaosNew.Add(item);
                        }
                    }
                    else
                    {
                        tieuDeBaosNew.Add(new TieuDeBaoCao()
                        {
                            Id=a.id,
                            TieuDe=a.headername
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
        public List<BangBaoCao> GetDanhSachBaoCao(int IdDonVi,int IdPrecious)
        {
            List<BangBaoCao> bangBaoCaos = new List<BangBaoCao>();
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var _Precious = tongcuclamnghiep.m_precious.Where(a => a.id == IdPrecious).FirstOrDefault();
            var _Unit = tongcuclamnghiep.m_unit.Where(a => a.id == IdDonVi).FirstOrDefault();
            if(_Precious!=null && _Unit == null)
            {
                tongcuclamnghiep.m_reporttable.Where(a => a.status == true).OrderBy(a => a.order).ToList().ForEach(a =>
                {
                    BangBaoCao bangBaoCao = new BangBaoCao();
                    bangBaoCao.Loai = a.autorow == true ? 1 : 0;
                    bangBaoCao.SapXep = a.order.Value;
                    bangBaoCao.TieuDe = a.title;
                    bangBaoCao.Id = a.id;
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
                                    if (!(TieuDeCot.IsComponent == true || TieuDeCot.IsNo == true))
                                    {
                                        if (duLieuCot.TypeValue == 1)
                                        {
                                            List<DataTongStringDonVi> dataTongStringDonVis = new List<DataTongStringDonVi>();
                                            DataCell.m_unit_datareport.Where(d=>d.m_unit_precious.completedate!=null && d.m_unit_precious.precious_id==_Precious.id).ToList().ForEach(d => {
                                                dataTongStringDonVis.Add(new DataTongStringDonVi()
                                                {
                                                    Data = d.data_text,
                                                    IdUnit = d.m_unit_precious.unit_id.Value,
                                                    UnitName=d.m_unit_precious.m_unit.unitname
                                                });
                                            });
                                            duLieuCot.DuLieuTextTong = dataTongStringDonVis;
                                        }
                                        else
                                        {
                                            List<DataTongNumberDonVi> dataTongNumberDonVis = new List<DataTongNumberDonVi>();
                                            DataCell.m_unit_datareport.Where(d => d.m_unit_precious.completedate != null && d.m_unit_precious.precious_id == _Precious.id).ToList().ForEach(d => {
                                                dataTongNumberDonVis.Add(new DataTongNumberDonVi()
                                                {
                                                    Data = string.IsNullOrEmpty(d.data_text) ? 0 : double.Parse(d.data_text),
                                                    IdUnit = d.m_unit_precious.unit_id.Value,
                                                    UnitName = d.m_unit_precious.m_unit.unitname
                                                });
                                            });
                                            duLieuCot.TongDuLieuInt = dataTongNumberDonVis;
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
                        tongcuclamnghiep.m_unit_datereport_autorows.Where(b => b.reporttable_id == a.id && b.m_unit_precious.precious_id==_Precious.id && b.m_unit_precious.completedate!=null).GroupBy(b => b.orderrow).ToList().ForEach(b =>
                        {
                            DongBaoCao dongBaoCao = new DongBaoCao();
                            dongBaoCao.RowId = b.Key.Value;
                            List<DuLieuCot> duLieuCots = new List<DuLieuCot>();
                            bangBaoCao.DanhSachTieuDeCol.ToList().ForEach(c =>
                            {
                                DuLieuCot duLieuCot = new DuLieuCot();
                                duLieuCot.TieuDeCot = c;
                                List<DataTongStringDonVi> dataTongStringDonVis = new List<DataTongStringDonVi>();
                                b.Where(d => d.header_id == c.Id).ToList().ForEach(d => {
                                    dataTongStringDonVis.Add(new DataTongStringDonVi()
                                    {
                                        Data = d.datatext,
                                        IdUnit = d.m_unit_precious.unit_id.Value,
                                        UnitName = d.m_unit_precious.m_unit.unitname
                                    });
                                });
                                duLieuCot.DuLieuTextTong = dataTongStringDonVis;
                                duLieuCots.Add(duLieuCot);
                            });
                            dongBaoCao.DanhSachCot = duLieuCots;
                            dongBaoCaos.Add(dongBaoCao);
                        });
                    }
                    bangBaoCao.DanhSachDong = dongBaoCaos;
                    bangBaoCaos.Add(bangBaoCao);
                });
            }
            else
            {
                if(_Precious != null && _Unit != null)
                {
                    var _UnitPrecious = tongcuclamnghiep.m_unit_precious.Where(a => a.precious_id == _Precious.id && a.unit_id == _Unit.id && a.completedate!=null).FirstOrDefault();
                    tongcuclamnghiep.m_reporttable.Where(a => a.status == true).OrderBy(a => a.order).ToList().ForEach(a =>
                    {
                        BangBaoCao bangBaoCao = new BangBaoCao();
                        bangBaoCao.Loai = a.autorow == true ? 1 : 0;
                        bangBaoCao.SapXep = a.order.Value;
                        bangBaoCao.TieuDe = a.title;
                        bangBaoCao.Id = a.id;
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
                                            var DataCellUnit = DataCell.m_unit_datareport.Where(d => d.m_unit_precious.completedate != null && d.unit_precious_id == _UnitPrecious.id).FirstOrDefault();
                                            if (DataCellUnit != null)
                                            {
                                                duLieuCot.DuLieuText = DataCellUnit.data_text;
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
        
        public ResTableData GetPreciousTableAll(int start, int length, int draw, string search)
        {
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            var dataAll = tongcuclamnghiep.m_precious.Where(a => (search.Length > 0 ? (a.precious_name.Contains(search)) : true));
            var data = dataAll.ToList().Skip((start)).Take(length).Select(a => new
            {
                a.id,
                a.precious_name,
                createdate = a.createdate.Value.ToString("dd/MM/yyyy"),
                completedate = a.completedate.Value.ToString("dd/MM/yyyy"),
                a.status,
                startend = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}", a.startdate, a.enddate)
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