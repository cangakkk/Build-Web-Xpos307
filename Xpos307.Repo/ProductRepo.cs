using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xpos307.DataModels;
using Xpos307.ViewModels;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Data;

namespace Xpos307.Repo
{
    public class ProductRepo
    {
        private readonly XPOS307Context db;
        private readonly VMResponse response=new VMResponse();
        private readonly int userId = 1;
        public ProductRepo(XPOS307Context _db)
        {
            db = _db;
        }
        
        public VMResponse GetAll()
        {
            try
            {
                List<VMTblProduct> dataView = (
                from p in db.TblProduct
                join v in db.TblVariant
                    on p.VariantId equals v.Id
                join c in db.TblCategory
                    on v.CategoryId equals c.Id
                where p.IsDelete == false
                select new VMTblProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Image = p.Image,

                    VariantId = p.VariantId,
                    VariantName = v.Name,

                    CategoryId=v.CategoryId,
                    CategoryName=c.Name,

                    IsDelete = p.IsDelete,
                    CreateBy = p.CreateBy,
                    CreateDate = p.CreateDate,
                    UpdateBy = p.UpdateBy,
                    UpdateDate = p.UpdateDate
                }).ToList();

                response.message = (dataView.Count > 0)
                    ? $"Product data Successfully fetched!"
                    : $"Product data is not available!";
                response.entity = dataView;
            }
            catch( Exception e)
            {
                response.Success= false;
                response.message= "Save Failed!"+e.Message;
            }

            return response;
        } 

        public VMResponse GetByID(int ProductID)
        {
            try
            {
                VMTblProduct DataView = (
                from p in db.TblProduct
                join v in db.TblVariant
                    on p.VariantId equals v.Id
                join c in db.TblCategory
                    on v.CategoryId equals c.Id
                where p.IsDelete == false && p.Id == ProductID
                select new VMTblProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Image = p.Image,

                    VariantId = p.VariantId,
                    VariantName = v.Name,

                    CategoryId = v.CategoryId,
                    CategoryName = c.Name,

                    IsDelete = p.IsDelete,
                    CreateBy = p.CreateBy,
                    CreateDate = p.CreateDate,
                    UpdateBy = p.UpdateBy,
                    UpdateDate = p.UpdateDate
                }).SingleOrDefault();
                response.message = (DataView != null)
                    ? $"Product data Successfully fetched!"
                    : $"Product data is not available!";
                response.entity = DataView;
            }
            catch( Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        }

        public VMResponse GetByName(string Name)
        {
            try
            {
                List<VMTblProduct> dataView = (
                from p in db.TblProduct
                join v in db.TblVariant
                    on p.VariantId equals v.Id
                join c in db.TblCategory
                    on v.CategoryId equals c.Id
                where p.IsDelete == false && (p.Name+v.Name+c.Name).Contains(Name)
                select new VMTblProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Stock = p.Stock,
                    Image = p.Image,

                    VariantId = p.VariantId,
                    VariantName = v.Name,

                    CategoryId = v.CategoryId,
                    CategoryName = c.Name,

                    IsDelete = p.IsDelete,
                    CreateBy = p.CreateBy,
                    CreateDate = p.CreateDate,
                    UpdateBy = p.UpdateBy,
                    UpdateDate = p.UpdateDate
                }).ToList();

                response.message = (dataView.Count > 0)
                    ? $"Product data Successfully fetched!"
                    : $"Product data is not available!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Save Failed!" + e.Message;
            }

            return response;
        }

        public VMResponse Edit(VMTblProduct dataView)
        {
            try
            {
                TblProduct DataModel = db.TblProduct.Find(dataView.Id);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {dataView.Id} is not available!";
                }
                else
                {
                    DataModel.Id = dataView.Id;
                    DataModel.Name = dataView.Name;
                    DataModel.Price= dataView.Price;
                    DataModel.Stock = dataView.Stock;

                    DataModel.VariantId= dataView.VariantId;
                    DataModel.Image = dataView.Image;
                  
                    DataModel.UpdateBy = dataView.UpdateBy;
                    DataModel.UpdateDate = DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = $"Data ID= {dataView.Id} Succesfully Update";
                    response.entity = dataView;
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Save Failed!" + e.Message;
            }
            return response;
        }

        public VMResponse Add(VMTblProduct dataView)
        {
            try
            {
                TblProduct dataModel = new TblProduct();

                dataModel.Name= dataView.Name;
                dataModel.Price= dataView.Price;
                dataModel.Stock = dataView.Stock;
                dataModel.VariantId = dataView.VariantId;
                dataModel.Image = dataView.Image;
               
                dataModel.CreateBy = dataView.CreateBy;
                dataModel.CreateDate = DateTime.Now;

                db.Add(dataModel);
                db.SaveChanges();

                response.message = "Data baru berhasil ditambahkan!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Data ditambahkan Error" + e.Message;
            }
            return response;
        }

        public VMResponse Delete(int orderId, int updateBy)
        {
            try
            {
                TblProduct DataModel = db.TblProduct.Find(orderId);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {DataModel.Id} is not available!";
                }
                else
                {
                    DataModel.IsDelete = true;
                    DataModel.UpdateBy = updateBy;
                    DataModel.UpdateDate = DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = "Data Succesfully Delete!";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Delete Error!" + e.Message;
            }
            return response;
        }
    }
}
