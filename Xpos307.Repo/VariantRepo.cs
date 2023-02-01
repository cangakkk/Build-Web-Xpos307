using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xpos307.DataModels;
using Xpos307.ViewModels;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using AutoMapper;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Xpos307.Repo
{
    public class VariantRepo
    {
        private readonly XPOS307Context db;
        private readonly VMResponse response= new VMResponse();
        private int userId = 1; 

        public VariantRepo(XPOS307Context _db)
        {
            this.db = _db;
        }

        public static IMapper GetMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap <TblVariant, VMTblVariant> ();
                cfg.CreateMap <VMTblVariant, TblVariant> ();
            });

            return config.CreateMapper();
        }

        public VMResponse GetAll()
        {
            try
            {
                List<VMTblVariant> dataView = (

                    from v in db.TblVariant
                    join c in db.TblCategory
                        on v.CategoryId equals c.Id
                    where v.IsDelete == false
                    select new VMTblVariant
                    {
                        Id = v.Id,
                        CategoryId = v.CategoryId,
                        Name = v.Name,
                        Description = v.Description,
                        CategoryName = c.Name,

                        IsDelete = v.IsDelete,
                        CreateBy = v.CreateBy,
                        CreateDate = v.CreateDate,
                        UpdateBy = v.UpdateBy,
                        UpdateDate = v.UpdateDate,

                    }).ToList();

                response.message = (dataView.Count > 0)
                    ? $"Variant data Successfully fetched!"
                    : $"Variant data is not available!";
                response.entity = dataView;
            }

            catch (Exception e)
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
                List<VMTblVariant> dataView = (

                    from v in db.TblVariant
                    join c in db.TblCategory
                        on v.CategoryId equals c.Id
                    where v.IsDelete == false &&
                        (v.Name+c.Name+v.Description+c.Description).Contains(Name)
                    select new VMTblVariant
                    {
                        Id = v.Id,
                        CategoryId = v.CategoryId,
                        Name = v.Name,
                        Description = v.Description,
                        CategoryName = c.Name,

                        IsDelete = v.IsDelete,
                        CreateBy = v.CreateBy,
                        CreateDate = v.CreateDate,
                        UpdateBy = v.UpdateBy,
                        UpdateDate = v.UpdateDate,

                    }).ToList();

                response.message = (dataView.Count > 0)
                    ? $"Variant data Successfully fetched!"
                    : $"Variant data is not available!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Error Get Data!" + e.Message;
            }
            return response;
        }

        public VMResponse GetById(int variantId)
        {
            try
            {
                VMTblVariant dataView = (
                    from v in db.TblVariant
                    join c in db.TblCategory on v.CategoryId equals c.Id
                    where v.IsDelete == false
                        && v.Id == variantId
                    select new VMTblVariant
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Description = v.Description,

                        CategoryId = v.CategoryId,
                        CategoryName = c.Name,

                        IsDelete = v.IsDelete,
                        CreateBy = v.CreateBy,
                        CreateDate = v.CreateDate,
                        UpdateBy = v.UpdateBy,
                        UpdateDate = v.UpdateDate
                    }
                ).FirstOrDefault();

                response.message = "Data Successfully Fetched!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Get Data Failed: " + e.Message;
            }

            return response;
        }

        public VMResponse GetByCategory(int categoryId)
        {
            try
            {
                List<VMTblVariant> dataView = (
                    from v in db.TblVariant
                    join c in db.TblCategory on v.CategoryId equals c.Id
                    where v.IsDelete == false
                        && v.CategoryId == categoryId
                    select new VMTblVariant
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Description = v.Description,

                        CategoryId = v.CategoryId,
                        CategoryName = c.Name,

                        IsDelete = v.IsDelete,
                        CreateBy = v.CreateBy,
                        CreateDate = v.CreateDate,
                        UpdateBy = v.UpdateBy,
                        UpdateDate = v.UpdateDate
                    }
                ).ToList();

                response.message = "Data Successfully Fetched!";
                response.entity = dataView;
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Get Data Failed: " + e.Message;
            }
            return response;
        }

        public VMResponse Edit(VMTblVariant dataView)
        {
            try
            {
                TblVariant DataModel = db.TblVariant.Find(dataView.Id);
                if (DataModel == null)
                {
                    response.Success = false;
                    response.message = $"Customer with ID = {dataView.Id} is not available!";
                }
                else
                {
                    DataModel.Id = dataView.Id;
                    DataModel.Name = dataView.Name;
                    DataModel.CategoryId= dataView.CategoryId;
                    DataModel.Description= dataView.Description;
                    
                    DataModel.UpdateBy = dataView.UpdateBy;
                    DataModel.UpdateDate = DateTime.Now;

                    db.Update(DataModel);
                    db.SaveChanges();

                    response.message = $"Data ID={dataView.Id} Succesfully Update";
                    response.entity = dataView;
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.message = "Update Failed!" + e.Message;
            }
            return response;
        }

        public VMResponse Add(VMTblVariant dataView)
        {
            try
            {
                TblVariant dataModel = new TblVariant();

                dataModel.Name = dataView.Name;
                dataModel.CategoryId = dataView.CategoryId;
                dataModel.Description= dataView.Description;

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

        public VMResponse Delete(int variantId, int updateBy)
        {
            try
            {
                TblVariant DataModel = db.TblVariant.Find(variantId);
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
