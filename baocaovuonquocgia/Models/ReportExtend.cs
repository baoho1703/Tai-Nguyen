using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using baocaovuonquocgia.App_Data;

namespace baocaovuonquocgia.Models
{
    public class ReportExtend
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

        public List<BangBaoCao> GetTableEmpty()
        {
            List<BangBaoCao> bangBaoCaos = new List<BangBaoCao>();
            tongcuclamnghiepEntities tongcuclamnghiep = new tongcuclamnghiepEntities();
            tongcuclamnghiep.m_reporttable.Where(a => a.status == true).OrderBy(a => a.order).ToList().ForEach(a =>
            {
                BangBaoCao bangBaoCao = new BangBaoCao();
                bangBaoCao.Id = a.id;
                bangBaoCao.Loai = a.autorow == true ? 1 : 0;
                bangBaoCao.SapXep = a.order.Value;
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
                        }
                        duLieuCots.Add(duLieuCot);
                    });
                    dongBaoCao.DanhSachCot = duLieuCots;
                    dongBaoCaos.Add(dongBaoCao);
                });
                bangBaoCao.DanhSachDong = dongBaoCaos;
                bangBaoCaos.Add(bangBaoCao);
            });

            return bangBaoCaos;
        }
    }
}