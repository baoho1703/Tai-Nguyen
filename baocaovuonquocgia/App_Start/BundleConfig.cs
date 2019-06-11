using System.Web;
using System.Web.Optimization;

namespace baocaovuonquocgia
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/metisMenu.js",
                      "~/Scripts/jquery.slimscroll.min.js",
                      "~/Scripts/moment.js",
                      "~/Scripts/combodate.js",
                      "~/Scripts/common.js",
                      "~/Scripts/custom.js",
                      "~/Scripts/ExportExcel.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryHeader").Include(
                      "~/Scripts/custom.js",
                      "~/Scripts/jquery.min.js",
                      "~/Scripts/datatables.min.js"));
            //jqueryHeader
            bundles.Add(new StyleBundle("~/bundlescss/css").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/font-awesome.min.css",
                       "~/Content/css/hint.min.css",
                      "~/Content/css/datatables.min.css",
                      "~/Content/css/style.css",
                      "~/Content/css/metisMenu.css",
                      "~/Content/css/main.css"));
            bundles.Add(new StyleBundle("~/bundlescss/logincss").Include(
                      "~/Content/css/bootstrap.min.css",
                      "~/Content/css/font-awesome.min.css",
                      "~/Content/css/main.css"));
            bundles.Add(new ScriptBundle("~/bundles/Tree").Include(
                      "~/Scripts/Tree/jquery-ui.js",
                      "~/Scripts/Tree/jquery.fancytree.js",
                      "~/Scripts/Tree/jquery.fancytree.dnd.js",
                      "~/Scripts/Tree/jquery.fancytree.menu.js"));
            bundles.Add(new StyleBundle("~/bundlescss/Tree").Include(
                      "~/Scripts/Tree/ui.fancytree.css",
                      "~/Scripts/Tree/jquery-ui.css"));
        }
    }
}
