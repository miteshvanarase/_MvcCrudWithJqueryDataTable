using MvcTest.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcTest.Controllers
{
    public class TestController : Controller
    {
        SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ToString());
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Submit()// Get the Category view
        {
            return View();
        }
        [HttpPost]
        public ActionResult Submit(Category cg)//insert Data into the Category Master
        {
            if (ModelState.IsValid)
            {
                SqlCommand cmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Option", 1);
                cmd.Parameters.AddWithValue("@NAME", cg.CategoryName);

                sqlcon.Open();
                string res = cmd.ExecuteNonQuery().ToString();
                sqlcon.Close();

                return Json(cg, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0);
            }
        }
        [HttpPost]
        public ActionResult UpdateCategoryById(Category ct)//update the Category details by id
        {
            SqlCommand cmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Option", 5);
            cmd.Parameters.AddWithValue("@CategoryID", ct.CategoryId);
            cmd.Parameters.AddWithValue("@Name", ct.CategoryName);
            sqlcon.Open();
            string res = cmd.ExecuteNonQuery().ToString();
            sqlcon.Close();
            BindCategory();
            return Json(ct, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListOfCategory()// Display the list of category
        {
            DataTable dt = new DataTable();
            SqlCommand sqlcmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
            sqlcmd.Parameters.AddWithValue("@Option", 2);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
            da.Fill(dt);
            return View(dt);
        }
        [HttpPost]
        public ActionResult DeleteCategory(Category cg)// delete data from category master
        {
           
                SqlCommand cmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Option", 3);
                cmd.Parameters.AddWithValue("@CategoryID", cg.CategoryId);
                sqlcon.Open();
                string res = cmd.ExecuteNonQuery().ToString();
                sqlcon.Close();
                return Json(cg, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult BindCategory()//bind category into dropdownlist
        {
            DataSet ds = GetCategory();
            //ViewBag.countries = ds.Tables[0]; 
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                items.Add(new SelectListItem { Text = dr["Name"].ToString(), Value = dr["CategoryId"].ToString() });
            }
            ViewBag.category = items;
            return View();
        }
        public DataSet GetCategory()
        {
            SqlCommand sqlcmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
            sqlcmd.Parameters.AddWithValue("@Option", 2);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd);
            DataSet ds = new DataSet();
            sqlda.Fill(ds);
            return ds;
        }
        public ActionResult EditCategory(int CategoryId)//fill the details by id (edit view)
        {
            Category ct = new Category();
            SqlCommand sqlcmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
            sqlcmd.Parameters.AddWithValue("@Option", 4);
            sqlcmd.Parameters.AddWithValue("@CategoryID", CategoryId);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd);
            DataTable dt = new DataTable();
            sqlda.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ct.CategoryId = Convert.ToInt32(dt.Rows[0]["CategoryId"]);
                ct.CategoryName = Convert.ToString(dt.Rows[0]["Name"]);
                return View(ct);
            }
            return View();
        }
        public ActionResult InsertProduct()/// Get the Product view
        {
            BindCategory();
            return View();
        }
        [HttpPost]
        public ActionResult InsertProduct(Product pt)//Insert data into Product master
        {
            if (ModelState.IsValid)
            {
                SqlCommand cmd = new SqlCommand("USP_PRODUCT", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Option", 1);
                cmd.Parameters.AddWithValue("@CategoryID", pt.CategoryId);
                cmd.Parameters.AddWithValue("@ProductName", pt.ProductName);
                sqlcon.Open();
                string res = cmd.ExecuteNonQuery().ToString();
                sqlcon.Close();
                BindCategory();
                return Json(pt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0);
            }
        }
        
        public ActionResult ListOfProduct()//display the View of product
        {
            DataTable dt = new DataTable();
            SqlCommand sqlcmd = new SqlCommand("USP_PRODUCT", sqlcon);
            sqlcmd.Parameters.AddWithValue("@Option", 3);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
            da.Fill(dt);
            return View(dt);
        }
        [HttpPost]
        public ActionResult ProductList()//bind the list of product
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortcolumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortcolumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            int pagesize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalrecords = 0;
            DataTable dt = new DataTable();
            SqlCommand sqlcmd = new SqlCommand("USP_PRODUCT", sqlcon);
            sqlcmd.Parameters.AddWithValue("@Option", 3);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
            da.Fill(dt);
           // return View(dt);

            var v = (from c in dt.AsEnumerable()
                     select new
                     {
                         ProductId = c.Field<int>("ProductId"),
                         ProductName = c.Field<string>("ProductName"),
                         CategoryId = c.Field<int>("CategoryId"),
                         CategoryName = c.Field<string>("CategoryName")                        
                     });

         
            totalrecords = v.Count();
            var data = v.Skip(skip).Take(pagesize).ToList();
            return Json(new { draw = draw, recordsfiltered = totalrecords, recordtotal = totalrecords, data = data }, JsonRequestBehavior.AllowGet);
           
        }

        [HttpPost]
        public ActionResult CategoryList()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortcolumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortcolumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            int pagesize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int totalrecords = 0;

            DataTable dt = new DataTable();
            SqlCommand sqlcmd = new SqlCommand("USP_CATEGORY_CRUD", sqlcon);
            sqlcmd.Parameters.AddWithValue("@Option", 2);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
            da.Fill(dt);
            var v = (from c in dt.AsEnumerable()
                     select new
                     {
                         CategoryId = c.Field<int>("CategoryId"),
                         Name = c.Field<string>("Name")                       
                     });
            totalrecords = v.Count();
            var data = v.Skip(skip).Take(pagesize).ToList();
            return Json(new { draw = draw, recordsfiltered = totalrecords, recordtotal = totalrecords, data = data }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult EditProduct(int ProductId)//fill the details by id (edit view)
        {
            if (ModelState.IsValid)
            {
                Product pt = new Product();
                SqlCommand sqlcmd = new SqlCommand("USP_PRODUCT", sqlcon);
                sqlcmd.Parameters.AddWithValue("@Option", 5);
                sqlcmd.Parameters.AddWithValue("@ProductID", ProductId);

                sqlcmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter sqlda = new SqlDataAdapter(sqlcmd);
                DataTable dt = new DataTable();
                sqlda.Fill(dt);
                BindCategory();
                if (dt.Rows.Count > 0)
                {
                    pt.ProductId = Convert.ToInt32(dt.Rows[0]["ProductID"]);
                    pt.CategoryId = Convert.ToInt32(dt.Rows[0]["CategoryID_FK"]);
                    pt.ProductName = Convert.ToString(dt.Rows[0]["Name"]);
                    return View(pt);
                }
            }
            else
            {
                return Json(0);
            }
            return View();
        }
        public ActionResult UpdateProduct(Product pt)//update data into Product master
        {
            if (ModelState.IsValid)
            {
                SqlCommand cmd = new SqlCommand("USP_PRODUCT", sqlcon);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Option", 6);
                cmd.Parameters.AddWithValue("@CategoryID", pt.CategoryId);
                cmd.Parameters.AddWithValue("@ProductId", pt.ProductId);
                cmd.Parameters.AddWithValue("@ProductName", pt.ProductName);
                sqlcon.Open();
                string res = cmd.ExecuteNonQuery().ToString();
                sqlcon.Close();
                BindCategory();
                return Json(pt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0);
            }
        }

        public ActionResult DeleteProduct(Product pt)//delete data from product master
        {
            SqlCommand cmd = new SqlCommand("USP_PRODUCT", sqlcon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Option", 4);
            cmd.Parameters.AddWithValue("@ProductID", pt.ProductId);
            sqlcon.Open();
            string res = cmd.ExecuteNonQuery().ToString();
            sqlcon.Close();
            return Json(pt, JsonRequestBehavior.AllowGet);
        }



    }
}